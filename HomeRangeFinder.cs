/******************************************************************************
 * Change Date:   02/04/2008
 * Change By:     Bob Cummings
 * Description:   Since we started to sort the Eligible Sites the loop for picking 
 *                a site based on the random number had to be reversed to start at
 *                the top and work down in getHomeRangeCenter
 * ****************************************************************************
 * Change Date:   02/11/2008
 * Change By:     Bob Cummings
 * Description:   Made another change to Eligible Sites, so changed loop back to 
 *                checking from 0 on up.  Added logic to reset the index twice after 
 *                finding correct home site.
 * ****************************************************************************
 * Change Date:   Saturday, February 16, 2008 11:23:12 AM
 * Change By:     Bob Cummings
 * Description:   Added logic to check if the distance between the current 
 *                location and a eligible location was less then 1 to be 
 *                equal to one.                 
 ****************************************************************************/

using System;
using System.Linq;
using DesignByContract;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using log4net;
namespace SEARCH
{
   /// <summary>
   /// Summary description for HomeRangeFinder.
   /// </summary>
   public class HomeRangeFinder : IHomeRangeFinder
   {
		#region Non-Public Members (36) 

		#region Fields (12) 

      int fileNameIndex;
      const string homeRangePolygonFileName = "\\SuitablePolygons.shp";
      const string myAvailableAreaFileExtension = ".shp";
      const string myAvailableAreaFileName = @"\tempAvailable";
      const string myDissovleFileName = @"\tempDissolve.shp";
      protected IFeatureClass myAvailableAreas;
      private DataManipulator myDataManipulator;
      const string myGoodStepsPointFileName = @"\GoodSteps.shp";
      private MapManager myMapManager;
      protected System.Collections.ArrayList myPolygons;
      protected RandomNumbers rn = null;
      protected List<EligibleHomeSite> siteList;

		#endregion Fields 
		#region Constructors (1) 

      protected HomeRangeFinder()
      {
         this.rn = RandomNumbers.getInstance();
         this.myDataManipulator = new DataManipulator();
         this.fileNameIndex = 0;
         this.myMapManager = MapManager.GetUniqueInstance();
         this.siteList = new List<EligibleHomeSite>();

      }

		#endregion Constructors 
		#region Methods (23) 

       private ILog mLog = LogManager.GetLogger("hrTrigger");
       private ILog eLog = LogManager.GetLogger("Error");
       protected IPoint chooseHomeRangeCenter(List<EligibleHomeSite> inQualifiedSites, double inHomeRangeArea)
      {
         IPoint currPoint = new PointClass();
         double luckyNumber = rn.getUniformRandomNum();
         int count=0;
         mLog.Debug("inside choose chooseHomeRangeCenter in the HomeRangeFinde class");
         mLog.Debug("we have " + inQualifiedSites.Count.ToString() + " points that were qualified to work with");
         mLog.Debug("");
         {
            foreach (EligibleHomeSite ehs in inQualifiedSites)
            {
               currPoint.X = ehs.X;
               currPoint.Y = ehs.Y;
               count++;
               mLog.Debug("this sites rank is " + ehs.Rank.ToString());
               mLog.Debug("calling get area");
               
               //Multiply the minimum home range area by 1/10 so that animals can
               //select HR centers even if they haven't seen enough area around it yet
               //i.e. we keep all points in contiguous area of >= 1/10th HR size
               //Also used in setSuitablePolygons
               if (this.getArea(currPoint) >= (inHomeRangeArea*0.1))
               {
                 
                  mLog.Debug("had enough area we are out of here");
                   break;
               }
            }
         }
         if (count < inQualifiedSites.Count)
            return currPoint;
         else
            return null;
      }

      private void DeleteTooSmallPolygons(IFeatureClass fc)
      {
         IQueryFilter qf = new QueryFilterClass();
         qf.WhereClause = "Delete = 'true'";
         IFeatureCursor delCurr = fc.Update(qf, true);
         IFeature feat = delCurr.NextFeature();
         while (feat != null)
         {
            delCurr.DeleteFeature();
            feat = delCurr.NextFeature();
         }
         System.Runtime.InteropServices.Marshal.ReleaseComObject(delCurr);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(qf);
      }

      protected bool FindHomeRange(Animal inAnimal, string inFileName)
      {
         mLog.Debug("");
         mLog.Debug("inside FindHomeRange for animal number " + inAnimal.IdNum.ToString());
         bool foundHome = false;
         mLog.Debug("Going to remove the old files and site list if needed");
         try
         {
            this.RemoveOldFiles(System.IO.Path.GetDirectoryName(inFileName));
            this.siteList.Clear();

            mLog.Debug("now make the step map");
            // this will create a point map of all the steps in the animals memory.
            // It will be made in the Animal's home directory with the name Step.shp
            if (this.setSuitableSites(inAnimal, inFileName))
            {
               mLog.Debug("now make a map of suitable polygons from the animals memory");
               //now see if any of the areas visited are eligible and large enough
               //this will create a polygon map in the Animal's home directory with the name SuitablePolygons.shp
               if (this.setSuitablePolygons(inAnimal.HomeRangeCriteria.Area, inAnimal.Sex, inFileName))
               {
                  mLog.Debug("must have been some suitable polygons now see if any of the steps where inside the suitable polygons");
                  // Now see if any of 
                  List<EligibleHomeSite> suitableSites = this.getSuitableSteps(inFileName);
                  if (suitableSites != null)
                  {
                     mLog.Debug("must have been some so now pull those out of the animals memory");
                     var q = inAnimal.MyVisitedSites.MySites.Intersect(suitableSites, new SiteComparer());
                     //make sure there were some points
                     if (q.Count() > 0)
                     {
                        mLog.Debug("there were " + q.Count().ToString() + " sites that were in good polygons");
                        foundHome = true;
                        foreach (EligibleHomeSite i in q)
                        {
                           this.siteList.Add(i);
                        }
                     }
                  }

               }

            }
         }
         catch (Exception ex)
         {

            eLog.Debug(ex);
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif 
         }
        
         mLog.Debug("Leaving FindHomeRange for animal " + inAnimal.IdNum.ToString() + " with a value of " + foundHome.ToString());
         return foundHome;
         
      }

      protected double getArea(IPoint inPoint)
      {
         double area = 0;
         IPolygon searchPoly = null;
         IRelationalOperator relOp = null;
         IFeatureCursor searchCurr = null;
         IFeature feat = null;
         mLog.Debug("inside get area of the home range finder class for point X = " + inPoint.X.ToString() + " and Y = " + inPoint.Y.ToString());
         try
         {
            relOp = (IRelationalOperator)inPoint;

            searchCurr = this.myMapManager.SocialMap.mySelf.Search(null, true);
             
            while ((feat = searchCurr.NextFeature()) != null)
            {
               searchPoly = feat.Shape as IPolygon;
               if (relOp.Within(searchPoly))
               {
                  area = this.getArea(searchPoly);
                  break;
               }
            }
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(searchPoly);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(searchCurr);
         }
         mLog.Debug("leaving with an area of " + area.ToString());
         return area;

      }

      protected double getArea(IPolygon inPoly)
      {
         double area = 0;
         IArea areaGetter;
         try
         {
            mLog.Debug("inside get area");
            areaGetter = (IArea)inPoly;

            area = areaGetter.Area;
            mLog.Debug("total area is " + area.ToString());
            //area is in meters we are measuring in km so divide by 1000^2
            area = areaGetter.Area / (1000000);
            mLog.Debug("total area is " + area.ToString() + " kilometers");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
            area = 0;
         }
         return area;
      }

      protected IPoint getHomeRangeCenter()
      {
         double luckyNumber = rn.getUniformRandomNum();
         IPoint p = null;
         int i = 0;
         try
         {

            mLog.Debug("inside getHomeRangeCenter ");
            mLog.Debug("the roll of the dice is " + luckyNumber.ToString());
            //logValues(inEhs);

            //should set up some sort of binary search but wtf over budget already.
            while (luckyNumber >= this.siteList[i++].Rank)
            {
               mLog.Debug("current site rank is " + this.siteList[i].Rank.ToString());
            }

            //since the index is auto incremented we are one past after the comparison
            //so we need to go back 1 to get the correct point
            //if it was the first one in the list then we can not go back any further
            if (i > 0)
               i--;


            p = new PointClass();
            p.X = this.siteList[i].X;
            p.Y = this.siteList[i].Y;
            mLog.Debug("site we chose has a rank of " + this.siteList[i].Rank.ToString());
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return p;
      }

      protected IPoint getHomeRangeCenter(EligibleHomeSites inEhs)
      {
         double luckyNumber = rn.getUniformRandomNum();
         IPoint p = null;
         int i = 0;
         try
         {

            mLog.Debug("inside getHomeRangeCenter ");
            mLog.Debug("the roll of the dice is " + luckyNumber.ToString());
            logValues(inEhs);

            //should set up some sort of binary search but wtf over budget already.
            while (luckyNumber >= inEhs.getSite(i++).Rank)
            {
               mLog.Debug("current site rank is " + inEhs.getSite(i).Rank.ToString());
            }

            //since the index is auto incremented we are one past after the comparison
            //so we need to go back 1 to get the correct point
            //if it was the first one in the list then we can not go back any further
            if(i>0)
               i--;
            

            p = new PointClass();
            p.X = inEhs.getSite(i).X;
            p.Y = inEhs.getSite(i).Y;
            mLog.Debug("site we chose has a rank of " + inEhs.getSite(i).Rank.ToString());
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return p;
      }

      private List<EligibleHomeSite> GetListOfPoints(IFeatureClass fc)
      {
         mLog.Debug("inside GetListOfPoints for " + fc.AliasName);
         List<EligibleHomeSite> outSites = new List<EligibleHomeSite>();
         IFeatureCursor search = fc.Search(null, false);
         IFeature feat = search.NextFeature();
         while (feat != null)
         {
            IPoint p = feat.ShapeCopy as IPoint;
            EligibleHomeSite ehs = new EligibleHomeSite(p);
            outSites.Add(ehs);
            feat = search.NextFeature();
         }

         System.Runtime.InteropServices.Marshal.ReleaseComObject(search);
         mLog.Debug("we have " + outSites.Count.ToString() + " number of points that are good");
         return outSites;
      }

      protected IPolygon getPolygon(IPoint inPoint)
      {
         bool found = false;
         IRelationalOperator relOp = (IRelationalOperator)inPoint;
         IFeature f = null;
         IPolygon searchPoly = null;
         mLog.Debug("inside getPolygon");
         try
         {
            for (int i = 0; i < this.myPolygons.Count && !found; i++)
            {
               f = (IFeature)myPolygons[i];
               searchPoly = (IPolygon)f.ShapeCopy;
               if (relOp.Within(searchPoly))
               {
                  found = true;
               }
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }

         if (found)
            return searchPoly;
         else
            return null;

      }

      private List<int> GetPolyIndexes(string inFileName)
      {
         int fieldIndex;
         int currValue;
         List<int> outList = new List<int>();
         IFeatureClass fc = this.myDataManipulator.GetFeatureClass(inFileName);
         IFeatureCursor curr = fc.Search(null, false);
         IFeature feat = curr.NextFeature();
         fieldIndex = feat.Fields.FindFieldByAliasName("Id");
         while (feat != null)
         {
            currValue = System.Convert.ToInt16(feat.get_Value(fieldIndex));
            if (!outList.Contains(currValue))
            {
               outList.Add(currValue);
            }
            feat = curr.NextFeature();
         }

         System.Runtime.InteropServices.Marshal.ReleaseComObject(fc);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(curr);
         return outList;

      }

      protected List<EligibleHomeSite> getSuitableSteps(string inAnimalMemoryMap)
      {
         mLog.Debug("inside getSuitableSteps");
         string tempPolyFileName = System.IO.Path.GetDirectoryName(inAnimalMemoryMap) + homeRangePolygonFileName;
         mLog.Debug("Polygon file name is " + tempPolyFileName);
         string tempPointFileName = System.IO.Path.GetDirectoryName(inAnimalMemoryMap) + "\\Step.shp";
         mLog.Debug("point file name is " + tempPointFileName);
         string outFileName = System.IO.Path.GetDirectoryName(inAnimalMemoryMap) +   myGoodStepsPointFileName;
         mLog.Debug("out file name is " + outFileName);
         mLog.Debug("calling intersect myDataManipulator.IntersectFeatures");
         
         IFeatureClass fc = myDataManipulator.IntersectFeatures(tempPolyFileName + " ; " + tempPointFileName, outFileName, "ERROR 000953");
         if (fc != null)
         {
            mLog.Debug("now get the list of points");
            return (this.GetListOfPoints(fc));
         }
         else
            return null;
      }

      private void logValues(EligibleHomeSites inEhs)
      {
         EligibleHomeSite eh;
         for (int i = 0; i < inEhs.SiteCount; i++)
         {
            eh = inEhs.getSite(i);
            mLog.Debug("site number " + i.ToString());
            mLog.Debug("site is eligible = " + eh.SuitableSite.ToString());
            mLog.Debug("X = " + eh.X.ToString() + " Y+ " + eh.Y.ToString());
            mLog.Debug("rank is " + eh.Rank.ToString());
         }
      }

      protected void makeArrayOfPolygons(IFeatureCursor inFC)
      {
         mLog.Debug("inside makeArrayOfPolygons");
         myPolygons = new System.Collections.ArrayList();
         IFeature f;
         f = inFC.NextFeature();
         mLog.Debug("starting the loop");
         while (f != null)
         {
            myPolygons.Add(f);
            f = inFC.NextFeature();
         }
         mLog.Debug("done with the loop and have " + myPolygons.Count.ToString() + " polygons");
      }

      protected IPointList makePointList(EligibleHomeSites inEhs)
      {
         IPointList pl = null;
         mLog.Debug("inside make point list going to make " + inEhs.SiteCount.ToString() + " points");
         try
         {
            pl = new IPointList();
            IPoint p = null;
            for (int i = 0; i < inEhs.SiteCount - 1; i++)
            {
               p.X = inEhs.getSite(i).X;
               p.Y = inEhs.getSite(1).Y;
               pl.add(p);
            }
            mLog.Debug("actuall made " + pl.Count().ToString() + " points");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return pl;
      }

      private void MarkPollygonsTooSmall(double minAreaNeeded, IFeatureClass fc)
      {
         IFeatureCursor curr = null;
         IFeature feat = null;
         IPolygon currPoly;

         curr = fc.Update(null, false);
         feat = curr.NextFeature();
         int index = feat.Fields.FindFieldByAliasName("Delete");
         while (feat != null)
         {
            currPoly = feat.ShapeCopy as IPolygon;
            if (this.getArea(currPoly) < minAreaNeeded)
            {
               feat.set_Value(index, "true");
               feat.Store();
               curr.UpdateFeature(feat);
            }
            feat = curr.NextFeature();
         }
         curr.Flush();
      }

      private void RemoveOldFiles(string Path)
      {
         MapManager.RemoveFiles(Path + "\\Step.shp");
         //MapManager.RemoveFiles(Path + homeRangePolygonFileName);
         MapManager.RemoveFiles(Path + myGoodStepsPointFileName);
      }

      private void resetRank()
      {
         int i = 0;
         int j = 1;

         for (i = 0, j = 1; j < this.siteList.Count; i++, j++)
         {
            
               this.siteList[j].Rank = this.siteList[i].Rank + this.siteList[j].Rank;
            
         }
         //sometimes it will only be .9999999999999987 and the random number could
         //conviebly be .9999999999989 so eliminate any chance.
         this.siteList[this.siteList.Count - 1].Rank = 1.0;

      }

      protected void setDistance(IPoint currLocation)
      {
         IPoint p = new PointClass();
         double lineLength = 0;
         try
         {

            IPolyline tempLine = new PolylineClass();
            mLog.Debug("inside getDistance in the HomeRangeFinderClass");
            mLog.Debug("setting the from point");
            tempLine.FromPoint = currLocation;
            mLog.Debug("from point is X = " + currLocation.X.ToString() + " Y = " + currLocation.Y.ToString());
            mLog.Debug("now going to loop through and collect the distances.");
            for (int i = 0; i < this.siteList.Count; i++)
            {
               p.X = siteList[i].X;
               p.Y = siteList[i].Y;
               tempLine.ToPoint = p;
               //BC Saturday, February 16, 2008 moved value from tempLine.Length to lineLength because we can not modify
               // tempLine.Length (readOnly property) and was worried about divide by zero issue.
               lineLength = tempLine.Length;
               mLog.Debug("to  point is X = " + tempLine.ToPoint.X.ToString() + " Y = " + tempLine.ToPoint.Y.ToString());
               if (lineLength < 1) lineLength = 1;
               siteList[i].DistanceFromCurrLocation = lineLength;
               mLog.Debug("the distance between them is " + lineLength);
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         mLog.Debug("leaving getDistance");
         return;
      }

      protected void setDistance(Animal inA)
      {

         IPoint p = new PointClass();
         double lineLength = 0;
         try
         {

            IPolyline tempLine = new PolylineClass();
            mLog.Debug("inside getDistance in the HomeRangeFinderClass");
            mLog.Debug("setting the from point");
            tempLine.FromPoint = inA.Location;
            mLog.Debug("from point is X = " + inA.Location.X.ToString() + " Y = " + inA.Location.Y.ToString());
            mLog.Debug("now going to loop through and collect the distances.");
            for (int i = 0; i < this.siteList.Count; i++)
            {
             
              {
                  p.X = this.siteList[i].X;
                  p.Y = this.siteList[i].Y;
                  tempLine.ToPoint = p;
                  //BC Saturday, February 16, 2008 moved value from tempLine.Length to lineLength because we can not modify
                  // tempLine.Length (readOnly property) and was worried about divide by zero issue.
                  lineLength = tempLine.Length;
                  mLog.Debug("to  point is X = " + tempLine.ToPoint.X.ToString() + " Y = " + tempLine.ToPoint.Y.ToString());
                  if (lineLength < 1) lineLength = 1;
                  this.siteList[i].DistanceFromCurrLocation = lineLength;
                  mLog.Debug("the distance between them is " + lineLength);
               }
            }

            EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Dist;
            this.siteList.Sort();
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         mLog.Debug("leaving getDistance");
         return;
      }

      protected void SetRanges(double inValue)
      {
         double d = 0;
         try
         {
            mLog.Debug("now setting the ranges based on the total rankings = " + inValue.ToString());
            mLog.Debug("starting the loop");
            foreach (EligibleHomeSite ehs in this.siteList)
            {

               mLog.Debug(ehs.X.ToString() + ehs.Y.ToString() + " is eligble site raw rank is " + ehs.Rank.ToString());
               ehs.Rank = ehs.Rank / inValue;
               mLog.Debug("after adjusting rank is " + ehs.Rank.ToString());
               d += ehs.Rank;

            }


            mLog.Debug("total rank is " + d.ToString());
            this.sortByRank();
            this.resetRank();


         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      protected bool setSuitablePolygons(double minAreaNeededOverall, string inAnimalSex, string inAnimalMemoryMap)
      {
         
         //Another place where the 1/10th rule is used (also in HomeRangeFinder) 
         //i.e. polygon must be more than 1/10th min HR size to be kept
         double minAreaNeeded = minAreaNeededOverall * 0.1;
         bool result = true;
         try
         {
            mLog.Debug("inside setSuitablePolygons");
            string tempFileName = System.IO.Path.GetDirectoryName(inAnimalMemoryMap) + homeRangePolygonFileName;
            string tempFileName2 = System.IO.Path.GetDirectoryName(inAnimalMemoryMap) + myDissovleFileName;
            IFeatureClass fc = myDataManipulator.GetSuitablePolygons(inAnimalMemoryMap, inAnimalSex, tempFileName);
            IFeatureClass fc2 = myDataManipulator.DissolveBySexAndReturn(fc, tempFileName2, inAnimalSex);
            //Could put a multipart-to-singlepart execution here if you wanted

            // if there are not any suitable polygons the feature class will be null 
            mLog.Debug("ok check if there were any polygons that were suitable");
            if (fc2 != null)
            {
               //fw.writeLine("must have been, now add the delete field so we can measure the area and delete the small ones");
               //this.myDataManipulator.AddField("TEXT", "Delete", null, tempFileName);
               mLog.Debug("now check each one to see if it is bigger then " + minAreaNeeded.ToString());
               this.MarkPollygonsTooSmall(minAreaNeeded, fc2);
               mLog.Debug("now call delete too small");
               this.DeleteTooSmallPolygons(fc2);
               mLog.Debug("now double check to see if there were any left");
               //if there are no rows then there are not any suitable areas yet
               mLog.Debug("after delete to small there are " + this.myDataManipulator.GetRowCount(fc2).ToString());
               if (this.myDataManipulator.GetRowCount(fc2) <= 0)
                  result = false;
            }
            else
            {
               mLog.Debug("evidently no suitable ones found");
               result = false;
            }

         }
         catch (Exception ex)
         {
            eLog.Debug(ex);
            result = false;
         }
         mLog.Debug("leaving setSuitablePolygons with a value of " + result.ToString());
         return result;

      

      }

      protected bool setSuitableSites(Animal inA, string inFileName)
      {
         bool result;

         try
         {
            mLog.Debug("inside setSuitableSites for animal number " + inA.IdNum.ToString());
            mLog.Debug("inFileName is " + inFileName);
            mLog.Debug("calling datamanipulator create step map");
            //this will create a point map in the animals home dir with a name of steps.shp
            result = this.myDataManipulator.CreateStepMap(inFileName, inA.MyVisitedSites.getPoints());
         }
         catch (Exception ex)
         {
            eLog.Debug(ex);
            result = false;
         }
         return result;
         
         
      }

      private void sortByRank()
      {
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Rank;
         this.siteList.Sort();

      }

		#endregion Methods 

		#endregion Non-Public Members 


      #region IHomeRangeFinder Members

      public virtual bool setHomeRangeCenter(Animal inAnimal, ESRI.ArcGIS.Geodatabase.IFeatureClass inAnmialMemoryMap)
      {
         // TODO:  Add HomeRangeFinder.setHomeRangeCenter implementation
         return false;
      }
      public virtual bool setHomeRangeCenter(Animal inA, string inFileName)
      {
         return false;
      }
      #endregion
   }
}
