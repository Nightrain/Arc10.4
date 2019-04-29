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
using DesignByContract;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
namespace SEARCH
{
   /// <summary>
   /// Summary description for HomeRangeFinder.
   /// </summary>
   public class HomeRangeFinder : IHomeRangeFinder
   {

      #region Fields (4)


      private DataManipulator myDataManipulator;
      protected FileWriter.FileWriter fw;
      protected RandomNumbers rn = null;
      protected System.Collections.ArrayList myPolygons;
      IFeatureClass myAvailableAreas;
      string myAvailableAreaFileName;
      string myAvailableAreaFileExtension;
      int fileNameIndex;

      #endregion Fields

      #region Constructors (1)

      protected HomeRangeFinder()
      {
         this.buildLogger();
         rn = RandomNumbers.getInstance();
         this.myDataManipulator = new DataManipulator();
         myAvailableAreaFileName = @"\tempAvailable";
         myAvailableAreaFileExtension = ".shp";
         fileNameIndex = 0;

      }

      #endregion Constructors

      #region Private Methods (1)

      private void logValues(EligibleHomeSites inEhs)
      {
         EligibleHomeSite eh;
         for (int i = 0; i < inEhs.Count; i++)
         {
            eh = inEhs.getSite(i);
            fw.writeLine("site number " + i.ToString());
            fw.writeLine("site is eligible = " + eh.SuitableSite.ToString());
            fw.writeLine("X = " + eh.X.ToString() + " Y+ " + eh.Y.ToString());
            fw.writeLine("rank is " + eh.Rank.ToString());
         }
      }

      #endregion Private Methods

      #region Protected Methods (10)

      protected void buildLogger()
      {
         string s;
         StreamReader sr;
         bool foundPath = false;

         string st = System.Windows.Forms.Application.StartupPath;
         if (File.Exists(st + @"\logFile.dat"))
         {
            sr = new StreamReader(st + @"\logFile.dat");
            while (sr.Peek() > -1)
            {
               s = sr.ReadLine();
               if (s.IndexOf("HomeRangeLogPath") == 0)
               {
                  fw = FileWriter.FileWriter.getHomeRangeLogger(s.Substring(s.IndexOf(" ")));
                  foundPath = true;
                  break;
               }
            }
            sr.Close();

         }
         if (!foundPath)
         {
            fw = new FileWriter.EmptyFileWriter();
         }
      }

      protected IPoint chooseHomeRangeCenter(List<EligibleHomeSite> inQualifiedSites, double minHomeRangeArea)
      {
         IPoint currPoint = new PointClass();
         int count=0;
         fw.writeLine("inside choose chooseHomeRangeCenter in the HomeRangeFinde class");
         fw.writeLine("we have " + inQualifiedSites.Count.ToString() + " sites to work with");
         foreach (EligibleHomeSite ehs in inQualifiedSites)
         {
            currPoint.X = ehs.X;
            currPoint.Y = ehs.Y;
            count++;
            fw.writeLine("calling get area");
            if (this.getArea(currPoint) >= minHomeRangeArea)
            {
               break;
               fw.writeLine("had enough area we are out of here");
            }
         }
         if (count < inQualifiedSites.Count)
            return currPoint;
         else
            return null;
      }

      protected double getArea(IPoint inPoint)
      {
         double area = 0;
         IPolygon searchPoly = null;
         IRelationalOperator relOp = null;
         IFeatureCursor searchCurr = null;
         fw.writeLine("inside get area of the home range finder class for point X = " + inPoint.X.ToString() + " and Y = " + inPoint.Y.ToString());
         try
         {
            relOp = (IRelationalOperator)inPoint;

            searchCurr = myAvailableAreas.Search(null, true);
            IFeature feat = searchCurr.NextFeature();
            while (feat != null)
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
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(searchPoly);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(searchCurr);
         }
         fw.writeLine("leaving with an area of " + area.ToString());
         return area;

      }

      protected double getArea(IPolygon inPoly)
      {
         double area = 0;
         IArea areaGetter;
         try
         {
            fw.writeLine("inside get area");
            areaGetter = (IArea)inPoly;

            area = areaGetter.Area;
            fw.writeLine("total area is " + area.ToString());
            //area is in meters we are measuring in km so divide by 1000^2
            area = areaGetter.Area / (1000000);
            fw.writeLine("total area is " + area.ToString() + " kilometers");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
            area = 0;
         }
         return area;
      }

      protected IPoint getHomeRangeCenter(EligibleHomeSites inEhs)
      {
         double luckyNumber = rn.getUniformRandomNum();
         IPoint p = null;
         int i = 0;
         try
         {

            fw.writeLine("inside getHomeRangeCenter ");
            fw.writeLine("the roll of the dice is " + luckyNumber.ToString());
            logValues(inEhs);

            //should set up some sort of binary search but wtf over budget already.
            while (luckyNumber >= inEhs.getSite(i++).Rank)
            {
               fw.writeLine("current site rank is " + inEhs.getSite(i).Rank.ToString());
            }

            //since the index is auto incremented we are one past after the comparison
            //so we need to go back 1 to get the correct point
            //if it was the first one in the list then we can not go back any further
            if(i>0)
               i--;
            

            p = new PointClass();
            p.X = inEhs.getSite(i).X;
            p.Y = inEhs.getSite(i).Y;
            fw.writeLine("site we chose has a rank of " + inEhs.getSite(i).Rank.ToString());
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return p;
      }

      protected IPolygon getPolygon(IPoint inPoint)
      {
         bool found = false;
         IRelationalOperator relOp = (IRelationalOperator)inPoint;
         IFeature f = null;
         IPolygon searchPoly = null;
         fw.writeLine("inside getPolygon");
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
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

         if (found)
            return searchPoly;
         else
            return null;

      }

      protected IFeatureCursor getSuitablePolygons(IFeatureClass inFeatures)
      {

         IQueryFilter qf = new QueryFilterClass();
         qf.WhereClause = "Available = 1";
         return inFeatures.Search(qf, false);
      }

      protected void makeArrayOfPolygons(IFeatureCursor inFC)
      {
         fw.writeLine("inside makeArrayOfPolygons");
         myPolygons = new System.Collections.ArrayList();
         IFeature f;
         f = inFC.NextFeature();
         fw.writeLine("starting the loop");
         while (f != null)
         {
            myPolygons.Add(f);
            f = inFC.NextFeature();
         }
         fw.writeLine("done with the loop and have " + myPolygons.Count.ToString() + " polygons");
      }

      protected IPointList makePointList(EligibleHomeSites inEhs)
      {
         IPointList pl = null;
         fw.writeLine("inside make point list going to make " + inEhs.Count.ToString() + " points");
         try
         {
            pl = new IPointList();
            IPoint p = null;
            for (int i = 0; i < inEhs.Count - 1; i++)
            {
               p.X = inEhs.getSite(i).X;
               p.Y = inEhs.getSite(1).Y;
               pl.add(p);
            }
            fw.writeLine("actuall made " + pl.Count().ToString() + " points");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return pl;
      }

      protected void setDistance(Animal inA)
      {

         IPoint p = new PointClass();
         double lineLength = 0;
         try
         {

            IPolyline tempLine = new PolylineClass();
            fw.writeLine("inside getDistance in the HomeRangeFinderClass");
            fw.writeLine("setting the from point");
            tempLine.FromPoint = inA.Location;
            fw.writeLine("from point is X = " + inA.Location.X.ToString() + " Y = " + inA.Location.Y.ToString());
            fw.writeLine("now going to loop through and collect the distances.");
            for (int i = 0; i < inA.MySites.Count; i++)
            {
               if (inA.MySites.getSite(i).SuitableSite)
               {
                  p.X = inA.MySites.getSite(i).X;
                  p.Y = inA.MySites.getSite(i).Y;
                  tempLine.ToPoint = p;
                  //BC Saturday, February 16, 2008 moved value from tempLine.Length to lineLength because we can not modify
                  // tempLine.Length (readOnly property) and was worried about divide by zero issue.
                  lineLength = tempLine.Length;
                  fw.writeLine("to  point is X = " + tempLine.ToPoint.X.ToString() + " Y = " + tempLine.ToPoint.Y.ToString());
                  if (lineLength < 1) lineLength = 1;
                  inA.MySites.getSite(i).DistanceFromCurrLocation = lineLength;
                  fw.writeLine("the distance between them is " + lineLength);
               }

            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         fw.writeLine("leaving getDistance");
         return;
      }

      protected bool setSuitableSites(Animal inA, string inFileName)
      {
         bool result = false;
         this.fileNameIndex++;
         myAvailableAreas = this.myDataManipulator.GetSuitablePolygons(inFileName, inA.Sex, this.myAvailableAreaFileName + this.fileNameIndex.ToString() + this.myAvailableAreaFileExtension);
         if (myAvailableAreas != null)
            result = true;
         return result;
      }

//      protected int setSutitableSites(Animal inAnimal, IFeatureClass inAnmialMemoryMap)
//      {
//         IFeatureCursor fc = null;
//         int suitableSites = 0;
//         int currStep;
//         double polyArea;
//         IPolygon currPoly;
//         EligibleHomeSite ehs;
//         PointClass p = new PointClass();
//         try
//         {
//            Check.Require(inAnmialMemoryMap != null, "InAnimalMap is null in ClosetHomeRanage finder");
//            fw.writeLine("inside the getSuitablePoints in the BestFoodHomeRangeFinder for george number " + inAnimal.IdNum.ToString());
//            fw.writeLine("making the feature class and feature cursor");
//            //fc = this.getSuitablePolygons(inAnmialMemoryMap);
//            fw.writeLine("now calling make array of polygons");
//            makeArrayOfPolygons(fc);
//            fw.writeLine("now loop through the sites");
//            for (currStep = inAnimal.MySites.Count - 1; currStep >= 0; currStep--)
//            {
//               ehs = inAnimal.MySites.getSite(currStep);
//               p.X = ehs.X;
//               p.Y = ehs.Y;
//               fw.writeLine("current site is x = " + ehs.X.ToString() + " y = " + ehs.Y.ToString());
//               currPoly = this.getPolygon(p);
//               if (currPoly != null)//if null was not in a suitable polygon
//               {
//                  polyArea = this.getArea(currPoly);
//                  fw.writeLine("current polygon is " + polyArea.ToString());
//                  fw.writeLine("george needs " + inAnimal.HomeRangeArea.ToString());
//                  if (polyArea >= inAnimal.HomeRangeArea)
//                  {
//                     fw.writeLine("ok big enough area so add it to the list");
//                     fw.writeLine("X = " + p.X.ToString() + " Y + " + p.Y.ToString());
//                     ehs.SuitableSite = true;
//                     suitableSites++;
//                  }//end if large enough
//               }//end if not occupied
//            }//end for loop
//         }//end try
//         catch (System.Exception ex)
//         {
//#if (DEBUG)
//            System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//            FileWriter.FileWriter.WriteErrorFile(ex);
//         }
//         fw.writeLine("leaving setSutitableSites with " + suitableSites.ToString());
//         return suitableSites;
//      }

      #endregion Protected Methods


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
