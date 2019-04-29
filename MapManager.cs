//# define pat
/******************************************************************************
 * CHANGE LOG
 * ****************************************************************************
 * Author:        Bob Cummings
 * Date:          Thursday, October 11, 2007 6:59:50 PM
 * Description:   Changed the methods GetMoveModifiers,GetRiskModifier, and 
 *                GetFoodData to only update the values passed in if the animal
 *                had changed polygons on the relevant maps.  
 ******************************************************************************
 * Author:        Bob Cummings
 * Date:          Saturday, October 13, 20078 11:23:28 AM
 * Description:   Added GetInitialMapData(Animal inA) call.  Should help alleviate
 *                some of the pain with testing.
 * ****************************************************************************
 * Author:        Bob Cummings
 * Date:          Saturday, February 23, 2008
 * Description:   Added logic to GetMoveModifiers if see if George was sitting 
 *                on the fence between 2 polygons when looking for new move parameters. 
 *                If he is then he will just hold onto his previous values.
 * ****************************************************************************
 * Author:        Bob Cummings
 * Date:          Saturday, May 02, 2009
 * Descripton:    Added the DataManipulator class to simplify some data processeing
 *                using the new features in ArcGIS 9.3.
 * ****************************************************************************
 * Author:        Bob Cummings
 * Date:          Saturday, September 19, 2009
 * Descripton:    Changed from using an array for holding the Animal Map collection
 *                to using a List<t>
 * ***************************************************************************/

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using System.IO;
using DesignByContract;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Specialized;
using System.Diagnostics;
using log4net;


namespace SEARCH
{

   /// <summary>
   /// -The Singleton defines an Instance operation that lets clients access its unique instance. 
   /// -It may be responsible for creating its own unique instance.
   /// 
   /// 
   /// </summary>
   public class MapManager
   {
		#region Constructors (1) 

private MapManager()
      {
         //check on whether we are going to log or not
         // get the error messages ready         
         initializeErrMessages();
         myDataManipulator = new DataManipulator();
         wrkSpaceFactory = new ShapefileWorkspaceFactoryClass();
         this.myFoodMaps = new Maps("Food");
         this.myMoveMaps = new Maps("Move");
         this.myPredationMaps = new Maps("Predation");
         this.mySocialMaps = new Maps("Social");
         this.myDispersalMaps = new Maps("Dispersal");
         this.myAnimalMaps = new List<AnimalMap>();
         SocialIndex = 0;
         numHomeRanges = 0;

      }

		#endregion Constructors 

		#region Fields (26) 

      private string errFileName;
      private System.Collections.Specialized.StringCollection errMessages;
      private int errNumber;
      private IFeatureWorkspace featureWrkSpace;
      private DateTime mCurrTime;
      private string mOutMapPath;
      private List<AnimalMap> myAnimalMaps;
      private DataManipulator myDataManipulator;
      private Map myDispersalMap = null;
      public Maps myDispersalMaps = null;
      private Map myFoodMap = null;
      public Maps myFoodMaps = null;
      private Hashtable myHash;
      private Map myMoveMap = null;
      public Maps myMoveMaps = null;
      private Map myPredationMap = null;
      public Maps myPredationMaps = null;
      private Map mySocialMap = null;
      public Maps mySocialMaps = null;
      private int numHomeRanges;
      private int SocialIndex;
      private static MapManager uniqueInstance;
      private IWorkspace wrkSpace;
      private IWorkspaceFactory wrkSpaceFactory;
      private string _currStepPath;
      private ILog mLog = LogManager.GetLogger("mapManager");
      private ILog eLog = LogManager.GetLogger("Error");

		#endregion Fields 

		#region Properties (4) 

      public string CurrStepPath
      {
         get { return _currStepPath; }
         set { _currStepPath = value; }
      }

      public DateTime CurrTime
      {
         get { return mCurrTime; }
         set { mCurrTime = value; }
      }

      public string OutMapPath
      {
         get { return mOutMapPath; }
         set
         {
            mOutMapPath = value;

         }
      }

      public Map SocialMap
      {
         get { return mySocialMap; }
         set { mySocialMap = value; }
      }

		#endregion Properties 

		#region Methods (49) 

		#region Public Methods (37) 

      public void AddTimeSteps(int AnimalID, IPolygon inPoly1, IPolygon inPoly2, int timeStep, string sex)
      {
         DataManipulator myTimeStepDataManipulator = new DataManipulator();
         try
         {
            Process[] processlist = Process.GetProcesses();
            foreach(Process theprocess in processlist)
            {
                if (theprocess.ProcessName == "DataCentric")
                {
                    mLog.Debug("Process: " + theprocess.ProcessName + " ID: " + theprocess.Id.ToString());
                    mLog.Debug("          Working Set: " + theprocess.WorkingSet64.ToString() + " Virtual Memory Allocation: " + theprocess.VirtualMemorySize64.ToString());
                    mLog.Debug("          Memory Use: " + theprocess.PrivateMemorySize64.ToString() + " Virtual Memory Peak: " + theprocess.PeakVirtualMemorySize64.ToString());
                }
            }
            mLog.Debug("");
            mLog.Debug("----------------------------------------------------------------------");
            mLog.Debug("inside AddTime Steps for animal "+ AnimalID.ToString());
            mLog.Debug("and the time step is " + timeStep.ToString());
            string currMapPath = this.myAnimalMaps[AnimalID].FullFileName;
            string newMapPath = this.makeNewMapPath(currMapPath, timeStep.ToString(), AnimalID.ToString());
            string oldMapPath = this.makeNewMapPath(currMapPath, (timeStep - 1).ToString(), AnimalID.ToString());
            string clipPath = this.OutMapPath + "\\Clippy_" + AnimalID.ToString() + timeStep.ToString() + ".shp";
            string unionPath = this.OutMapPath + "\\Union_" + AnimalID.ToString() + timeStep.ToString() + ".shp";
            string timeStepPath = this.OutMapPath + "\\TimeStepPath_" + AnimalID.ToString() + timeStep.ToString() + ".shp";
            string dissolvePath = this.OutMapPath + "\\DissolvePath_" + AnimalID.ToString() + timeStep.ToString() + ".shp";


            mLog.Debug("the  current animal map is " + currMapPath);
            mLog.Debug("the  current clip map is " + clipPath);
            mLog.Debug("the  current union map is " + unionPath);
            mLog.Debug("the  current temp timeStep map is " + timeStepPath);
            mLog.Debug("the  current dissolve map is " + dissolvePath);
            mLog.Debug("calling MakeDissolvedTimeStep");
            myTimeStepDataManipulator.MakeDissolvedTimeStep(this._currStepPath, timeStepPath, inPoly1, inPoly2);

            mLog.Debug("back in AddTimeSteps now going to clip against the social map");
            myTimeStepDataManipulator.Clip(this.mySocialMap.FullFileName, timeStepPath, clipPath);
            
          
            mLog.Debug("back from Clipping now update the Current Animal Map");
            //this is to get the current occupied or not at this time because
            // it could change over time.
            if (timeStep == 0)
            {
               //if the first time through then we only need to add it to the map
               mLog.Debug("Calling Copy to Animal Map since it is the first time step");
               myTimeStepDataManipulator.CopyToAnotherlMap(currMapPath, clipPath);
               myTimeStepDataManipulator.Dissolve(currMapPath, dissolvePath, "SUITABILIT;OCCUP_MALE;OCCUP_FEMA;Delete");
            }
            else
            {
               mLog.Debug("Calling update the animal map");
               myTimeStepDataManipulator.UnionAnimalClipData(currMapPath, clipPath, unionPath);
               myTimeStepDataManipulator.Dissolve(unionPath, dissolvePath, "SUITABILIT;OCCUP_MALE;OCCUP_FEMA;Delete");

            }

            mLog.Debug("now make the new animal map");

            this.makeMapCopies(System.IO.Path.GetDirectoryName(dissolvePath), System.IO.Path.GetFileNameWithoutExtension(dissolvePath), System.IO.Path.GetDirectoryName(currMapPath), System.IO.Path.GetFileNameWithoutExtension(newMapPath));
            this.myAnimalMaps[AnimalID].FullFileName = newMapPath;
            mLog.Debug("now we need to move the dissovled back to the orginal map");
            mLog.Debug("now going to copy " + dissolvePath + " to " + currMapPath);
            mLog.Debug("time to remove those extra files");
            this.removeExtraFiles(clipPath);
            this.removeExtraFiles(unionPath);
            this.removeExtraFiles(timeStepPath);
            this.removeExtraFiles(dissolvePath);
            this.removeExtraFiles(oldMapPath);


         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            
         }

      }

      public void addYearToMaps()
      {
         mySocialMaps.addYearToMaps();
         myFoodMaps.addYearToMaps();
         myPredationMaps.addYearToMaps();
         myMoveMaps.addYearToMaps();
         myDispersalMaps.addYearToMaps();

      }

      public bool BuildHomeRange(Animal inAnimal)
      {
         bool result = false;
         try
         {
            mLog.Debug("inside BuildHomeRange for George number " + inAnimal.IdNum.ToString());
            mLog.Debug("Building the new paths");
            string currSocialMapPath = this.SocialMap.FullFileName;
            string newUnionSocialMapPath = this.OutMapPath + "\\tempUnionSocial" + inAnimal.IdNum.ToString() + ".shp";
            string newTempPolyGonPath = this.OutMapPath + "\\tempPolyGon" + inAnimal.IdNum.ToString() + ".shp";
            string newTempSocialMapPath = this.OutMapPath + "\\tempSocial" + inAnimal.IdNum.ToString() + ".shp";
            string newSocialMapPath = this.OutMapPath + "\\NewSocialMap" + inAnimal.IdNum.ToString() + ".shp";
            string MulitToSinglePath = this.OutMapPath + "\\multiToSingle" + inAnimal.IdNum.ToString() + ".shp";


            mLog.Debug("Now making the home range builder");
            HomeRangeBuilder hrb = new HomeRangeBuilder();
            string NewHomeRangeFileName = hrb.BuildHomeRange(inAnimal, currSocialMapPath);
            //make sure a new polygon was built
            if (!NewHomeRangeFileName.Equals("No Home Found", StringComparison.CurrentCultureIgnoreCase))
            {
               result = true;
               mLog.Debug("new home range name is " + NewHomeRangeFileName);
               mLog.Debug("going to union it up and create " + newUnionSocialMapPath);
               this.myDataManipulator.UnionHomeRange(currSocialMapPath, NewHomeRangeFileName, newUnionSocialMapPath);
               mLog.Debug("Since the union tool can create a MultiPart we need to explode it using the Multi to Single");
               mLog.Debug("so the new map name will be " + MulitToSinglePath);
               mLog.Debug("now edit that map");
               this.EditNewHomeRangeUnion(newUnionSocialMapPath, inAnimal.Sex, inAnimal.IdNum.ToString());
               mLog.Debug("now call  myDataManipulator.CopyToAnother Map new map name is " + newTempSocialMapPath);
               this.myDataManipulator.CopyToAnotherlMap(newTempSocialMapPath, newUnionSocialMapPath);
               mLog.Debug("now call myDataManipulator.RemoveExtraFields");
               this.myDataManipulator.RemoveExtraFields(newTempSocialMapPath, "FID_availa; SUITABIL_1; OCCUP_MA_1; OCCUP_FE_1; Delete_1");
               mLog.Debug("now calling myDataManipulator.DissolveAndReturn to make " + newSocialMapPath);
               IFeatureClass newFC = this.myDataManipulator.DissolveAndReturn(newTempSocialMapPath, newSocialMapPath, "SUITABILIT;OCCUP_MALE;OCCUP_FEMA;Delete");
               mLog.Debug("Remove the old social map and assign the new one to me");
               this.SocialMap = null;
               this.SocialMap = new Map(newFC, newSocialMapPath);
               mLog.Debug("now blow away the temp maps for next time");
               this.removeExtraFiles(newUnionSocialMapPath);
               this.removeExtraFiles(newTempPolyGonPath);
               this.removeExtraFiles(newTempSocialMapPath);
               this.removeExtraFiles(MulitToSinglePath);
               if (System.IO.Path.GetDirectoryName(currSocialMapPath).Equals(this.OutMapPath))//do not want to blow away the orginal data just the temp maps
                  this.removeExtraFiles(currSocialMapPath);
               numHomeRanges++;
            }
            else
            {
               mLog.Debug("was not able to build a home range");
               result = false;
            }

         }
         catch (System.Exception ex)
         {
            result = false;
            eLog.Debug(ex);
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         return result;
      }

      public void changeMaps(DateTime now)
      {
         mySocialMaps.changeMap(now);
         myFoodMaps.changeMap(now);
         myPredationMaps.changeMap(now);
         myMoveMaps.changeMap(now);
         myDispersalMaps.changeMap(now);
      }

      public void changeMaps(DateTime now, AnimalManager am)
      {
         mLog.Debug("inside changeMaps (DateTime now,AnimalManager am)");
         mLog.Debug("DateTime = " + now.ToShortDateString() + " " + now.ToShortTimeString());

         try
         {
            myFoodMaps.changeMap(now);
            myPredationMaps.changeMap(now);
            myMoveMaps.changeMap(now);
            myDispersalMaps.changeMap(now, am);
            mySocialMaps.changeMap(now, am);
         }
         catch (Exception ex)
         {
            eLog.Debug(ex);
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            
         }
      }

//      public IFeatureClass getAnimalMap(int index)
//      {
//         AnimalMap am = null;
//         try
//         {
//            if (index >= 0 && index < this.myAnimalMaps.GetLength(0))
//            {
//               am = this.myAnimalMaps[index];
//            }

//         }
//         catch (System.Exception ex)
//         {
//#if DEBUG
//            System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//            FileWriter.FileWriter.WriteErrorFile(ex);
//         }
//         return am.mySelf;
//      }
      public string getAnimalMapName(int index)
      {
         string fileName = "";
         try
         {
            if (index >= 0 && index < this.myAnimalMaps.Count)
            {
               fileName = this.myAnimalMaps[index].FullFileName;
            }

         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
            fileName = "Not Found";
         }
         return fileName;

      }

      public double getArea(int index)
      {
         double area = 0;
         try
         {
            area = this.mySocialMap.getAvailableArea(index);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return area;
      }

     
      public crossOverInfo getCrossOverInfo(IPoint startPoint, IPoint endPoint)
      {
         mLog.Debug("inside get cross over info end point is null  = " + (null == endPoint).ToString());
         mLog.Debug("endpoint y = " + endPoint.Y.ToString());
         mLog.Debug("endpoint y = " + endPoint.X.ToString());

         return this.myMoveMap.getCrossOverInfo(startPoint, endPoint);
      }

      public crossOverInfo getCrossOverInfo(IPoint startPoint, IPoint endPoint, ref int currPolyIndex, ref int newPolyIndex)
      {
         mLog.Debug("inside map manger getCrossOverInfo");
         return this.myMoveMap.getCrossOverInfo(startPoint, endPoint, ref currPolyIndex, ref newPolyIndex);
      }

      public int getCurrMovePolygon(IPoint inPoint)
      {
         mLog.Debug("inside mapmanger calling map get current poly gon for x = " + inPoint.X.ToString() + " Y " + inPoint.Y.ToString());
         mLog.Debug("move map name is " + this.myMoveMap.FullFileName);
         return this.myMoveMap.getCurrentPolygon(inPoint);
      }

      public double getDistance(IPoint start, IPoint end)
      {
         IPolyline p = new PolylineClass();
         try
         {
            p.FromPoint = start;
            p.ToPoint = end;
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }

         return p.Length;
      }

      public string getErrMessage()
      {
         string errMessage;
         if (errNumber >= 0)
         {
            errMessage = "File Name: " + errFileName + System.Environment.NewLine + errMessages[errNumber];
            errNumber = -1;
         }
         else
         {
            errMessage = "no error";
         }
         return errMessage;

      }

      public void GetFoodData(IPoint location, ref int PolyIndex, ref double chance, ref double mean, ref double sd)
      {

         try
         {
            mLog.Debug("inside get food data for poly index " + PolyIndex.ToString());
            //check to see if we have moved out of the food index if so then adjust
            //Author: Bob Cummings moved all logic inside if statement.  No sense 
            //doing it unless the values changed.
            if (!myFoodMap.pointInPolygon(location, PolyIndex))
            {
               PolyIndex = myFoodMap.getCurrentPolygon(location);
            }
            //if the Poly Index is  less then 0 then george is sitting on the fence.
            //so use the old values and do not try to update.
            if (PolyIndex >= 0)
            {
               myHash = myFoodMap.getAllValuesForSinglePolygon(PolyIndex);
               chance = System.Convert.ToDouble(myHash["PROBCAP"]);
               mean = System.Convert.ToDouble(myHash["X_SIZE"]);
               sd = System.Convert.ToDouble(myHash["SD_SIZE"]);
            }

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public void GetInitalResidentAttributes(out InitialAnimalAttributes[] outAttributes)
      {
         outAttributes = null;
         try
         {
            this.mySocialMap.GetInitialResidentInformation(out outAttributes);
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }

      public void GetInitialAnimalAttributes(out InitialAnimalAttributes[] outAttributes)
      {
         outAttributes = null;
         try
         {
            this.myDispersalMap.GetInitialAnimalAttributes(out outAttributes);
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }

      }

      public void GetInitialMapData(Animal inA)
      {
         int PolyIndex = 0;
         mLog.Debug("inside get inital map data");
         mLog.Debug("getting move modifiers now");
         mLog.Debug("Animal number is " + inA.IdNum.ToString());
         mLog.Debug("The animals location is " + inA.getLocation_XY());


         PolyIndex = this.myMoveMap.getCurrentPolygon(inA.Location);
         if (PolyIndex >= 0)
         {
            this.myHash = this.myMoveMap.getAllValuesForSinglePolygon(PolyIndex);
            inA.MoveTurtosity = System.Convert.ToDouble(myHash["MVL"]);
            inA.MoveSpeed = System.Convert.ToDouble(myHash["MSL"]);
            inA.EnergyUsed = System.Convert.ToDouble(myHash["ENERGYUSED"]);
            inA.PerceptonModifier = System.Convert.ToDouble(myHash["PR_X"]);


         }
         else
         {
            inA.MoveTurtosity = 1;
            inA.MoveSpeed = 1;
            inA.EnergyUsed = 0;
            inA.PerceptonModifier = 1;
            while(PolyIndex < 0)
               PolyIndex = this.myMoveMap.getCurrentPolygon(inA.myMover.stepBack(inA));

         }

         inA.MoveIndex = PolyIndex;


         mLog.Debug("now getting initial food data ");
         PolyIndex = myFoodMap.getCurrentPolygon(inA.Location);
         myHash = myFoodMap.getAllValuesForSinglePolygon(PolyIndex);
         inA.CaptureFood = System.Convert.ToDouble(myHash["PROBCAP"]);
         inA.FoodMeanSize = System.Convert.ToDouble(myHash["X_SIZE"]);
         inA.FoodSD_Size = System.Convert.ToDouble(myHash["SD_SIZE"]);
         inA.FoodIndex = PolyIndex;

         mLog.Debug("now getting initial risk data");
         PolyIndex = myPredationMap.getCurrentPolygon(inA.Location);
         inA.PredationRisk = System.Convert.ToDouble(myPredationMap.getNamedValueForSinglePolygon(PolyIndex, "RISK"));
         inA.RiskIndex = PolyIndex;
      }

      public void GetMoveModifiers(IPoint inPoint, ref int PolyIndex, ref double MVL, ref double MSL,
         ref double PerceptionModifier, ref double EnergyUsed)
      {
         try
         {
            mLog.Debug("inside mapManager GetMoveModifiers checking to see if we moved out of the current polygon");
            mLog.Debug("move map name is " + this.myMoveMap.FullFileName + "\\" + this.myMoveMap.mySelf.AliasName);
            mLog.Debug("the PolyIndex is " + PolyIndex.ToString());
            mLog.Debug("my location is x=" + inPoint.X.ToString() + " and y=" + inPoint.Y.ToString());

            if (!this.myMoveMap.pointInPolygon(inPoint, PolyIndex))
            {
               mLog.Debug("we must have moved so get new polyIndex");
               mLog.Debug("old index is " + PolyIndex.ToString());
               mLog.Debug("old values are MSL = " + MSL.ToString() + " MVL + " + MVL.ToString() + " energyused is " + EnergyUsed.ToString() + " PerceptionModifier = " + PerceptionModifier.ToString());
               mLog.Debug("go to Map Log file to see new values.");
               PolyIndex = this.myMoveMap.getCurrentPolygon(inPoint);
               mLog.Debug("new move index is " + PolyIndex.ToString());

            }
            // Bob Cummings Saturday, February 23, 2008
            // If the PolyIndex is a negative value then George is on the fence between
            // two polygons so keep the orginal values.  Otherwise the program bombs.
            if (PolyIndex >= 0)
            {
               this.myHash = this.myMoveMap.getAllValuesForSinglePolygon(PolyIndex);
               MVL = System.Convert.ToDouble(myHash["MVL"]);
               MSL = System.Convert.ToDouble(myHash["MSL"]);
               EnergyUsed = System.Convert.ToDouble(myHash["ENERGYUSED"]);
               PerceptionModifier = System.Convert.ToDouble(myHash["PR_X"]);
            }
            else
            {
               MVL = 1;
               MSL = 1;
               EnergyUsed = 1;
               PerceptionModifier = 1;
            }

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }

      }

      public void getNumResidents(out int numMales, out int numFemales)
      {
         numMales = 0;
         numFemales = 0;
         this.mySocialMap.getNumResidents(out numMales, out numFemales);
      }

      public void GetRiskModifier(IPoint location, ref int PolyIndex, ref double risk)
      {

         try
         {
            mLog.Debug("incoming risk value is " + risk.ToString());
            mLog.Debug("inside GetRiskModifier checking to see if we moved out of the orginal polygon ");
            mLog.Debug("the poly index is " + PolyIndex.ToString());
            //check to see if we have moved out of the risk index if so then adjust
            //Author: Bob Cummings moved all logic inside if statement.  No sense 
            //doing it unless the values changed.
            if (!this.myPredationMap.pointInPolygon(location, PolyIndex))
            {
               mLog.Debug("must have changed now getting the new polygon");
               PolyIndex = myPredationMap.getCurrentPolygon(location);
            }
            //if the value is less then zero George is sitting on a fence between
            //two polygons.  So use old values.
            if (PolyIndex >= 0)
            {
               mLog.Debug("now getting the risk value");
               risk = System.Convert.ToDouble(myPredationMap.getNamedValueForSinglePolygon(PolyIndex, "RISK"));
               mLog.Debug("out going risk is " + risk.ToString());
            }

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }


      }

      public void getSocialIndex(IPoint inPoint, ref int inPolygonIndex)
      {

         try
         {
            mLog.Debug("");
            mLog.Debug("inside getSocialIndex for location x = " + inPoint.X.ToString() + " y = " + inPoint.Y.ToString());
            mLog.Debug("mySocialMap name is " + this.mySocialMap.mySelf.AliasName);
            if (!mySocialMap.pointInPolygon(inPoint, inPolygonIndex))
            {
               int tempPolyIndex = inPolygonIndex;
               inPolygonIndex = mySocialMap.getCurrentPolygon(inPoint);
               //sometimes the point is on a border between polygons in that case the 
               //return value is -1, so just use the last value
               if (inPolygonIndex < 0)
                  inPolygonIndex = tempPolyIndex;
            }


         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         mLog.Debug("leaving getSocialIndex with an index of " + inPolygonIndex.ToString());
         mLog.Debug("");

      }

      /********************************************************************************
       *  Function name   : validateNFieldPolylMap
       *  Description     : Will check to make sure it is a valid polygon shape file and 
       *                    has the required number of fields
       *  Return type     : bool 
       *  Argument        : string [] inMapFileNames all the file names of the map we need 
       *  Argument        : int NumFields the number of field the maps are supposed to have
       * *******************************************************************************/
      public IGeometryDef getSpatialInfo()
      {
         IGeometryDef geoDef = null;
         try
         {
            IField f;
            int fieldIndex = 0;
            IFeatureCursor fc;
            fc = this.mySocialMap.mySelf.Search(null, false);
            fieldIndex = fc.FindField("SHAPE");
            f = fc.Fields.get_Field(fieldIndex);
            geoDef = f.GeometryDef;
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
         return geoDef;
      }

      public static MapManager GetUniqueInstance()
      {

         if (uniqueInstance == null)
         {
            uniqueInstance = new MapManager();
         }
         return uniqueInstance;
      }

      public bool isOccupied(IPoint inPoint)
      {
         bool occupied = true;
         int polyIndex = this.mySocialMap.getCurrentPolygon(inPoint);
         string s = this.mySocialMap.getNamedValueForSinglePolygon(polyIndex, "OCCUP_MALE").ToString();

         if (s.Equals("NONE", StringComparison.CurrentCultureIgnoreCase))
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(polyIndex, "OCCUP_FEMA").ToString();

            if (s.Equals("NONE", StringComparison.CurrentCultureIgnoreCase))
            {
               occupied = false;
            }
         }
         return occupied;
      }

      public bool isOccupied(int inPolyIndex, string sex)
      {
         ILog aLog = LogManager.GetLogger("animalLog"); 
         bool occupied = true;
         string s;
         aLog.Debug("inside isOccupied checking for " + sex);
         if (sex == "Male")
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(inPolyIndex, "OCCUP_MALE").ToString();

         }
         else
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(inPolyIndex, "OCCUP_FEMA").ToString();
         }

         aLog.Debug("that returns " + s);
         s = s.ToUpper();
         if (s == "NONE")
         {
            occupied = false;
         }
         return occupied;
      }

      public bool isPointOnMoveMap(IPoint inPoint)
      {
         return this.myMoveMap.isPointOnMap(inPoint);
      }

      //Bob Cummings Sunday, February 24, 2008
      //added for checking if site is suitable for site home range trigger.
      public bool isSuitable(int inPolyIndex)
      {
          ILog aLog = LogManager.GetLogger("animalLog");
         bool isSuitable = false;
         string s;
         aLog.Debug("inside isSuitable checking for suitablility for index" + inPolyIndex.ToString());
         try
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(inPolyIndex, "SUITABILIT").ToString();
            if (s == "Suitable")
               isSuitable = true;
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         return isSuitable;
      }

      /********************************************************************************
      *   Function name   : loadMap
       *   Description     : loads the maps that need to be loaded for this part of the
       *                     simulation 
       *   Return type     : void 
       *   Argument        : string inMapType is the type of map "Social,Food" etc
       *   Argument        : string fileName is the actual name of the shape file to open
       * ********************************************************************************/
      public void loadOneMap(string inMapType, string fileName, string inPath)
      {
         try
         {
            mLog.Debug("inside load map for " + inMapType);
            //sometimes we are loading from an XML file and so we have to check that the
            //map is actually available
            String fullName = inPath + "\\" + fileName + ".shp";
            if (File.Exists(fullName))
            {
               mLog.Debug("the file we want to load is " + inPath + " " + fileName);
               //the file name is the fully qualifed path and name
               //all we use is the name of the shape file itself
               this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(inPath, 0);
               featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
               fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
               switch (inMapType)
               {
                  case "Social":



                     mLog.Debug("inside case under Social loading the map");
                     if (mySocialMap == null)
                     {
                         //total hack where we create a temp copy of the initial social map 
                         //so that we don't modify the original one
                         //we also delete any old temp_copy in that folder if present.
                         string copyName = "temp_copy";
                         fullName = inPath + "\\" + copyName + ".shp";
                         if (System.IO.File.Exists(fullName))
                         {
                             String tempPath = inPath + "\\" + copyName;
                             System.IO.File.Delete(fullName);
                             System.IO.File.Delete(tempPath + ".prj");
                             System.IO.File.Delete(tempPath + ".dbf");
                             System.IO.File.Delete(tempPath + ".sbn");
                             System.IO.File.Delete(tempPath + ".sbx");
                             System.IO.File.Delete(tempPath + ".shx");
                             System.IO.File.Delete(tempPath + ".xml");
                         }
                        makeMapCopies(inPath, fileName, inPath, copyName);
                        fileName = copyName;

                        mySocialMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName), fullName);
                     }
                     else
                     {
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(this.wrkSpace);
                        if (!Directory.Exists(mOutMapPath))
                           Directory.CreateDirectory(mOutMapPath);
                        IFeatureClass fc = myDataManipulator.GetFeatureClass(inPath, fileName);
                        mySocialMap = new Map(fc);
                     }
                     mySocialMap.TypeOfMap = "Social";
                     mySocialMap.Path = inPath;
                     mySocialMap.FullFileName = fullName;
                     break;
                  case "Food":
                     mLog.Debug("inside case under Food loading the map");
                     if (myFoodMap != null)
                        mLog.Debug("old map name is " + myFoodMap.mySelf.AliasName);
                     myFoodMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName));
                     mLog.Debug("new map name is " + myFoodMap.mySelf.AliasName);
                     myFoodMap.TypeOfMap = "Food";
                     myFoodMap.Path = inPath;
                     break;
                  case "Predation":
                     mLog.Debug("inside case under Predation loading the map");

                     myPredationMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName));
                     myPredationMap.TypeOfMap = "Risk";
                     myPredationMap.Path = inPath;
                     break;
                  case "Dispersal":
                     mLog.Debug("inside case under Dispersal loading the map");
                     myDispersalMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName));
                     myDispersalMap.TypeOfMap = "Dispersal";
                     myDispersalMap.Path = inPath;
                     break;
                  case "Move":
                     mLog.Debug("inside case under Move loading the map");
                     myMoveMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName));
                     myMoveMap.TypeOfMap = "Move";
                     myMoveMap.Path = inPath;
                     break;
                  default:
                     mLog.Debug("bombed with invalid name ");
                     System.Windows.Forms.MessageBox.Show("Not a valid map type" + inMapType);
                     break;
               }// end switch on map name
            }
            else
            {
               throw new SystemException(fullName + " Not found");
            }

         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif


         }

      }

      public void loadXMLTriggers(string mapType, XPathNodeIterator inIterator)
      {
         try
         {
            switch (mapType)
            {
               case "Social":
                  mySocialMaps.loadXMLTriggers(inIterator);
                  break;
               case "Food":
                  myFoodMaps.loadXMLTriggers(inIterator);
                  break;
               case "Predation":
                  myPredationMaps.loadXMLTriggers(inIterator);
                  break;
               case "Move":
                  myMoveMaps.loadXMLTriggers(inIterator);
                  break;
               case "Dispersal":
                  myDispersalMaps.loadXMLTriggers(inIterator);
                  break;
               default:
                  throw new ArgumentException(mapType + " is an invalid mapType in loadXMLTriggers in MapManagerClass");

            }

         }
         catch (ArgumentException ae)
         {
            // System.Windows.Forms.MessageBox.Show(ae.Message);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }

      }

      public bool MakeCurrStepMap(string path)
      {
         this._currStepPath = path + "\\currStep.shp";
         this.myDataManipulator.CreateEmptyFeatureClass(this._currStepPath, "polygon");
         return true;
      }

      public bool makeNewAnimalMaps(int numAnimals)
      {
         bool success = true;
         int i = 0;
         try
         {
            IGeometryDef geoDef = this.getSpatialInfo();
            mLog.Debug("inside make new animal map for " + numAnimals.ToString() + " number of animals");
            for (i = 0; i < numAnimals && success; i++)
            {
               myAnimalMaps.Add( new AnimalMap(i.ToString(), mOutMapPath, geoDef));
               //set reference to social map so we can add those fields on the makeMap call
               myAnimalMaps[i].MySocialMap = this.mySocialMap;

            }
            mLog.Debug("leaving make new animal maps");
         }
         catch (System.Exception ex)
         {
            success = false;
            eLog.Debug(ex);
         }
         if (success == false)
         {
            this.errNumber = (int)ERR.DIRECTORY_ALREADY_IN_USE;
            this.errFileName = mOutMapPath + @"\" + i.ToString();
         }
         return success;
      }

      public void makeNewDisperserAnimalMaps(int numberToAdd)
      {
         mLog.Debug("inside make new disperer animal maps");
         mLog.Debug("currently there are " + myAnimalMaps.Count.ToString());
         mLog.Debug("we are going to add " + numberToAdd.ToString());
         try
         {
            int totalNumMaps = myAnimalMaps.Count + numberToAdd;
            IGeometryDef geoDef = this.getSpatialInfo();
            mLog.Debug("starting loop");
            for (int i = myAnimalMaps.Count; i < totalNumMaps; i++)
            {
               myAnimalMaps.Add(new AnimalMap(i.ToString(), mOutMapPath, geoDef));
               myAnimalMaps[i].MySocialMap = this.mySocialMap;
            }
            mLog.Debug("now out of here");
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
     }

      public bool makeNewResidentAnimalMaps(int numResidents)
      {
         bool success = true;
         string resPath = mOutMapPath + "\\Resident";
         mLog.Debug("inside makeNewResidentAnimalMaps");
         mLog.Debug("currently there are " + myAnimalMaps.Count.ToString());
         mLog.Debug("we are going to add " + numResidents.ToString());
         mLog.Debug("we are going to build them at " + resPath);
         mLog.Debug("lets see if there is directory yet");

         try
         {

            if (!Directory.Exists(resPath)) Directory.CreateDirectory(resPath);
            int totalNumMaps = myAnimalMaps.Count + numResidents;
            IGeometryDef geoDef = this.getSpatialInfo();
            mLog.Debug("starting loop");
            for (int i = myAnimalMaps.Count; i < totalNumMaps; i++)
            {
               myAnimalMaps.Add(new AnimalMap(i.ToString(), resPath, geoDef));
               myAnimalMaps[i].MySocialMap = this.mySocialMap;
            }
            mLog.Debug("now out of here");
         }
         catch (System.Exception ex)
         {
            success = false;
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         return success;
      }
     

      public bool removeExtraFiles(string FullFilePath)
      {
         string[] fileNames;
         string currDir = System.IO.Path.GetDirectoryName(FullFilePath);
         string fileName = System.IO.Path.GetFileNameWithoutExtension(FullFilePath);
         
         bool success = true;
         try
         {
            mLog.Debug("inside remove extra files for " + FullFilePath);
            fileNames = Directory.GetFiles(currDir);
            for (int i = 0; i < fileNames.Length; i++)
            {
               string delFileName = System.IO.Path.GetFileNameWithoutExtension(fileNames[i]);
               
               if (delFileName.Equals(fileName,StringComparison.CurrentCultureIgnoreCase) || delFileName.EndsWith(".shp"))
               {
#if ! pat         
                  mLog.Debug("now going to delete " + delFileName);
                  File.Delete(fileNames[i]);
#endif
               }
            }

         }
         catch (System.Exception ex)
         {
            success = false;
            eLog.Debug(ex);
         }


         return success;


      }

      public static void RemoveFiles(string FullFilePath)
      {
          ILog eLog = LogManager.GetLogger("Error");
         string[] fileNames;
         string currDir = System.IO.Path.GetDirectoryName(FullFilePath);
         string fileName = System.IO.Path.GetFileNameWithoutExtension(FullFilePath);
         string compareFileName;
         bool success = true;
         try
         {
            fileNames = Directory.GetFiles(currDir);
            for (int i = 0; i < fileNames.Length; i++)
            {
               compareFileName = System.IO.Path.GetFileNameWithoutExtension(fileNames[i]);
               if (compareFileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
               {
                  File.Delete(fileNames[i]);
               }
            }
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
      }

      public void setUpNewYearsMaps(DateTime now, AnimalManager am)
      {
         mLog.Debug("inside setUpNewYearsMaps (DateTime now,AnimalManager am)");
         mLog.Debug("DateTime = " + now.ToShortDateString() + " " + now.ToShortTimeString());
         mySocialMaps.changeMap(now, am);
         myFoodMaps.changeMap(now);
         myPredationMaps.changeMap(now);
         myMoveMaps.changeMap(now);
         myDispersalMaps.changeMap(now, am);
      }

      public bool validateMap(string inMapName, string inMapDir)
      {
         bool success;
         mLog.Debug("inside validate Map for Map Manager ");
         mLog.Debug("the map name is " + inMapName);
         mLog.Debug("the dir is " + inMapDir);
         Check.Require(inMapName.Length > 0, "empty string for map type");
         Check.Require(inMapDir.Length > 0, "empty string for map dir");
         string[] fileNames;
         string[] fieldNames;
         string[] fileInfo;
         string mapType;
         success = false;
         if (Directory.Exists(inMapDir))
         {
            mLog.Debug("ok the directory exsits so now open the workspace");
            this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(inMapDir, 0);

            mLog.Debug("the directory exists now get all the file names in the dir");
            fileNames = Directory.GetFiles(inMapDir, "*.shp");
            mLog.Debug("there were " + fileNames.Length.ToString() + " files found");
            if (fileNames.GetLength(0) > 0)
            {
               switch (inMapName)
               {
                  case "Social":
                     mLog.Debug("inside case under Social");
                     fieldNames = new string[4];
                     fieldNames[0] = "Suitabilit";
                     fieldNames[1] = "OCCUP_MALE";
                     fieldNames[2] = "OCCUP_FEMA";
                     fieldNames[3] = "Delete";
                     success = validateNFieldPolylMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Social";
                     }
                     break;
                  case "Food":
                     mLog.Debug("inside case under Food");
                     fieldNames = new string[3];
                     fieldNames[0] = "PROBCAP";
                     fieldNames[1] = "X_SIZE";
                     fieldNames[2] = "SD_SIZE";
                     success = validateNFieldPolylMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Food";
                     }
                     break;
                  case "Predation":
                     mLog.Debug("inside case under Predation");
                     fieldNames = new string[1];
                     fieldNames[0] = "RISK";
                     success = validateNFieldPolylMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Predation";
                     }
                     break;
                  case "Dispersal":
                     mLog.Debug("inside case under Dispersal");
                     fieldNames = new string[3];
                     fieldNames[0] = "RELEASESIT";
                     fieldNames[1] = "MALES";
                     fieldNames[2] = "FEMS";
                     success = validateNFieldPointMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Dispersal";
                     }

                     break;
                  case "Move":
                     mLog.Debug("inside case under Move");
                     fieldNames = new string[5];
                     fieldNames[0] = "MVL";
                     fieldNames[1] = "MSL";
                     fieldNames[2] = "ENERGYUSED";
                     fieldNames[3] = "CROSSING";
                     fieldNames[4] = "PR_X";
                     success = validateNFieldPolylMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Move";
                     }
                     break;
                  default:
                     mLog.Debug("bombed with invalid name ");
                     System.Windows.Forms.MessageBox.Show("Not a valid map type" + inMapName);
                     break;
               }// end switch on map name

            }
            else // no files in dir
            {
               mLog.Debug("did not find any shape files");
               this.errNumber = (int)ERR.NO_FILES_FOUND_IN_DIRECTORY;
               this.errFileName = inMapDir;
            }
         }
         else//directory does not exsit
         {
            this.errNumber = (int)ERR.NO_DIRECTORY_FOUND;
            this.errFileName = inMapDir;
            mLog.Debug("directory does not exist");
            success = false;
         }

         mLog.Debug("leaving validateMap with a value of " + success);
         return success;
      }

      /********************************************************************************
       *  Function name   : validateMap
       *  Description     : the traffic cop for validating the different types of imput
       *                    maps we need to run the simulation
       *  Return type     : bool 
       *  Argument        : string inMapName  name of map we are validating
       *  Argument        : string inMapDir   directory where we can find the maps
       * *******************************************************************************/
      public bool validateMap(string inmapType, ref string inFilename, string inPath)
      {
         bool success;
         string mapType = inmapType;
         mLog.Debug("inside validate Map for Map Manager ");
         mLog.Debug("the map name is " + inmapType);
         mLog.Debug("the dir is " + inPath);
         success = false;
         if (Directory.Exists(inPath))
         {
            mLog.Debug("ok the directory exsits so now open the workspace");
            this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(inPath, 0);
            string[] fileNames;
            string[] fieldNames;

            mLog.Debug("the directory exists now get all the file names in the dir");
            fileNames = Directory.GetFiles(inPath, "*.shp");
            mLog.Debug("there were " + fileNames.Length.ToString() + " files found");
            if (fileNames.GetLength(0) > 0)
            {
               switch (inmapType)
               {
                  case "Social":
                     mLog.Debug("inside case under Social");
                     fieldNames = new string[3];
                     fieldNames[0] = "Suitabilit";
                     fieldNames[1] = "OCCUP_MALE";
                     fieldNames[2] = "OCCUP_FEMA";
                     break;
                  case "Food":
                     mLog.Debug("inside case under Food");
                     fieldNames = new string[3];
                     fieldNames[0] = "PROBCAP";
                     fieldNames[1] = "X_SIZE";
                     fieldNames[2] = "SD_SIZE";
                     break;
                  case "Predation":
                     mLog.Debug("inside case under Predation");
                     fieldNames = new string[1];
                     fieldNames[0] = "RISK";
                     break;
                  case "Dispersal":
                     mLog.Debug("inside case under Dispersal");
                     fieldNames = new string[3];
                     fieldNames[0] = "RELEASESIT";
                     fieldNames[1] = "MALES";
                     fieldNames[2] = "FEMS";
                     break;
                  case "Move":
                     mLog.Debug("inside case under Move");
                     fieldNames = new string[5];
                     fieldNames[0] = "MVL";
                     fieldNames[1] = "MSL";
                     fieldNames[2] = "ENERGYUSED";
                     fieldNames[3] = "CROSSING";
                     fieldNames[4] = "PR_X";
                     break;
                  default:
                     mLog.Debug("bombed with invalid name ");
                     System.Windows.Forms.MessageBox.Show("Not a valid map type" + inmapType);
                     return false;
               }// end switch on map name
               success = validateNFieldPolylMap(fileNames, fieldNames);
               if (success)
               {
                  inFilename = fileNames[0];
               }
               if (fileNames.Length > 1)
               {
                  mLog.Debug("More than one map in directory.  Used first (alphabetically) one.");
               }

            }
            else // no files in dir
            {
               mLog.Debug("did not find any shape files");
               this.errNumber = (int)ERR.NO_FILES_FOUND_IN_DIRECTORY;
               this.errFileName = inPath;
            }
         }
         else//directory does not exsit
         {
            this.errNumber = (int)ERR.NO_DIRECTORY_FOUND;
            this.errFileName = inPath;
            mLog.Debug("directory does not exist");
            success = false;
         }

         mLog.Debug("leaving validateMap with a value of " + success);
         return success;
      }

      public void writeXMLTriggers(ref XmlTextWriter xw)
      {
         mySocialMaps.writeXMLTriggers(ref xw);
         myFoodMaps.writeXMLTriggers(ref xw);
         myPredationMaps.writeXMLTriggers(ref xw);
         myMoveMaps.writeXMLTriggers(ref xw);
         myDispersalMaps.writeXMLTriggers(ref xw);
      }

		#endregion Public Methods 
		#region Private Methods (12) 

      private void AddFirstMemoryPoly(int AnimalID, IPolygon inPoly1, IPolygon inPoly2)
      {
         try
         {
            this.myAnimalMaps[AnimalID].addPolygon(inPoly1);
            this.myAnimalMaps[AnimalID].addPolygon(inPoly2);

         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
      }

      private void addNewPoly(Map inMap, IPolygon inPoly)
      {
         IFeature feature = null;
         IFields fields;
         int index = 0;


         //now add the one we want
         try
         {
            feature = inMap.mySelf.CreateFeature();
            fields = feature.Fields;
            index = fields.FindField("CurrTime");
            if (index >= 0)
               feature.set_Value(index, 1);
            index = fields.FindField("OCCUP_MALE");

            feature.Shape = inPoly;
            feature.Store();
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }


      }


      private DateTime createStartTime(string time, string typeOfTime)
      {
         string[] s;
         DateTime dt = DateTime.Today;
         try
         {
            switch (typeOfTime)
            {
               case "daily":
                  s = time.Split('/');
                  int day = System.Convert.ToInt32(s[1]);
                  int month = System.Convert.ToInt32(s[0]);
                  int year = System.Convert.ToInt32(s[2]);
                  dt = new DateTime(year, month, day);
                  break;
               case "hourly":
                  dt = DateTime.Today;
                  //the time is in the format of 7:32 PM
                  //so split it up and deal with it you wancker
                  s = time.Split(':');
                  dt = dt.AddHours(System.Convert.ToInt32(s[0]));
                  dt = dt.AddMinutes(System.Convert.ToInt32(s[1].Substring(0, 2)));
                  if (time.IndexOf("PM") > 0)
                     dt = dt.AddHours(12);
                  break;
               case "yearly":
                  dt = DateTime.Today;
                  dt = dt.AddYears(System.Convert.ToInt32(time) - dt.Year);
                  break;
               case "static":
                  dt = DateTime.Today;
                  dt = dt.AddYears(System.Convert.ToInt32(time) - dt.Year);
                  break;
               default:
                  throw new SystemException("Bad type of time in Map Manager createStartTime");

            }

         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }

         return dt;
      }

      private void EditNewHomeRangeUnion(string inUnionSocialMapPath, string sex, string inAnimalID)
      {
         int sexIndex;
         IFeatureCursor updateCurr;
         IFeature feat;
         IQueryFilter qf = new QueryFilterClass();
         qf.WhereClause = "FID_dissol >= 0";

         IFeatureClass fc = myDataManipulator.GetFeatureClass(inUnionSocialMapPath);
         if (sex.Equals("male", StringComparison.CurrentCultureIgnoreCase))
            sexIndex = fc.FindField("OCCUP_MALE");
         else
            sexIndex = fc.FindField("OCCUP_FEMA");

         updateCurr = fc.Update(qf, true);
         while ((feat = updateCurr.NextFeature()) != null)
         {
            feat.set_Value(sexIndex, inAnimalID);
            feat.Store();
         }
         updateCurr.Flush();
         System.Runtime.InteropServices.Marshal.ReleaseComObject(qf);

      }

      private void initializeErrMessages()
      {
         errMessages = new System.Collections.Specialized.StringCollection();
         this.errMessages.Add("Wrong type of shape file");
         this.errMessages.Add("Required field not found for this shape file.");
         this.errMessages.Add("No files found in directory");
         this.errMessages.Add("That directory is all ready in use");
         this.errMessages.Add("That directory is not available");
         errNumber = -1;

      }

      private void makeMapCopies(string orgPath, string orgFileName, string newPath, string newFileName)
      {
         string[] fileNames;
         string extension;
         try
         {
            if (!Directory.Exists(newPath))
               Directory.CreateDirectory(newPath);
            fileNames = Directory.GetFiles(orgPath, orgFileName + "*");
            for (int i = 0; i < fileNames.Length; i++)
            {
               extension = fileNames[i].Substring(fileNames[i].Length - 4, 4);
               File.Copy(fileNames[i], newPath + "\\" + newFileName + extension, true);
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      private string makeNewMapPath(string oldMapPath, string timeStep, string inAnimalID)
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(System.IO.Path.GetDirectoryName(oldMapPath) + "\\");
         sb.Append(inAnimalID + timeStep + ".shp");
         return sb.ToString();

      }

      private void removeAnimalMap(string oldMapPath)
      {

         try
         {
            mLog.Debug("");
            mLog.Debug("Inside removeAnimalMap " + oldMapPath);
            string currDir = System.IO.Path.GetDirectoryName(oldMapPath);
            string oldFileName = System.IO.Path.GetFileNameWithoutExtension(oldMapPath);
            string pattern = oldFileName + "*";
            string[] files = Directory.GetFiles(currDir, pattern);
            for (int i = 0; i < files.Length; i++)
            {
               if (oldFileName.Equals(System.IO.Path.GetFileNameWithoutExtension(files[i])))
               {
                  File.Delete(files[i]);
               }
            }
         }

         catch (Exception)
         {
            mLog.Debug("Exectional Fail");
            throw;
         }

      }

      private void removeTimeStepMaps(string AnimalID)
      {
         string[] fileNames;
         try
         {
            //use the animal id because each animal has its own sub folder based
            //on its name.
            fileNames = Directory.GetFiles(mOutMapPath + "\\" + AnimalID, "TimeStep" + "*");
            for (int i = 0; i < fileNames.Length; i++)
            {
#if ! pat
               File.Delete(fileNames[i]);
#endif
            }

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      /********************************************************************************
       *  Function name   : validateNFieldPointMap
       *  Description     : Will check to make sure it is a valid point shape file and 
       *                    has the required number of fields
       *  Return type     : bool 
       *  Argument        : string [] inMapFileNames the names of the map files to validate
       *  Argument        : int NumFields  the number of fields the maps should have
       * *******************************************************************************/
      private bool validateNFieldPointMap(string[] inMapFileNames, string[] inFieldNames)
      {
         int i;
         int found = -1;
         bool result;
         string fileName;
         IFeatureClass myShapefile = null;


         Check.Require(inFieldNames.Length > 0, "No field names to check in validateNFieldPointMap");
         Check.Require(inMapFileNames.Length > 0, "No files to check in validateNFieldPointMap");

         result = true;//assume we are good to go
         mLog.Debug("inside validateNFieldPointMap going to look for " + inFieldNames.Length.ToString() + " fields");
         mLog.Debug("set the feature workspace");
         featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
         for (i = 0; i < inMapFileNames.GetLength(0); i++)
         {
            fileName = System.IO.Path.GetFileNameWithoutExtension(inMapFileNames[i]);
            mLog.Debug("found " + inMapFileNames[i]);

            //create the feature_class
            myShapefile = this.featureWrkSpace.OpenFeatureClass(fileName);
            mLog.Debug("now check for correct type of shape file");
            if (myShapefile.ShapeType.ToString() == "esriGeometryPoint")
            {

               mLog.Debug("must be a point now check for the required field names");
               for (int j = 0; j < inFieldNames.Length; j++)
               {
                  mLog.Debug("looking for " + inFieldNames[j]);
                  found = myShapefile.Fields.FindField(inFieldNames[j]);
                  mLog.Debug("the result from the find field is " + found.ToString());
                  if (found < 0)
                  {
                     this.errNumber = (int)ERR.CAN_NOT_FIND_REQUIRED_FIELD;
                     mLog.Debug("did not find " + inFieldNames[j] + "  so set result = false and bail from loop");
                     this.errFileName = fileName;
                     result = false;
                     break;
                  }
               }//end checking for correct field names
            }
            else
            {
               mLog.Debug("wrong type of shape file so set result to false and bail out of here");
               result = false;
               this.errNumber = (int)ERR.WRONG_TYPE_OF_SHAPE_FILE;
               this.errFileName = fileName;
            }//end checking to see if it is a point file 
         }//end loop
         mLog.Debug("leaving validateNFieldPolylMap with a value of " + result.ToString());
         return result;
      }

      private bool validateNFieldPolylMap(string[] inMapFileNames, string[] inFieldNames)
      {
         int i=0;
         int found = -1;
         bool result;
         string fileName;
         IFeatureClass myShapefile = null;
         result = true;//assume we are good to go

         Check.Require(inFieldNames.Length > 0, "No field names to check in ValidatePolyMap");
         Check.Require(inMapFileNames.Length > 0, "No files to check in validateNFieldPolylMap");

         try
         {
            
            mLog.Debug("inside validateNFieldPolylMap going to look for " + inFieldNames.Length.ToString() + " fields");
            mLog.Debug("set the feature workspace");
            featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
            for (i = 0; i < inMapFileNames.GetLength(0); i++)
            {
               fileName = System.IO.Path.GetFileNameWithoutExtension(inMapFileNames[i]);
               mLog.Debug("found " + inMapFileNames[i]);

               //create the feature_class
               myShapefile = this.featureWrkSpace.OpenFeatureClass(fileName);
               mLog.Debug("now check for correct type of shape file");
               if (myShapefile.ShapeType.ToString() == "esriGeometryPolygon")
               {

                  mLog.Debug("must be a polygon now check for the required field names");
                  for (int j = 0; j < inFieldNames.Length; j++)
                  {
                     mLog.Debug("looking for " + inFieldNames[j]);
                     found = myShapefile.Fields.FindField(inFieldNames[j]);
                     mLog.Debug("the result from the find field is " + found.ToString());
                     if (found < 0)
                     {
                        this.errNumber = (int)ERR.CAN_NOT_FIND_REQUIRED_FIELD;
                        mLog.Debug("did not find " + inFieldNames[j] + "  so set result = false and bail from loop");
                        this.errFileName = fileName;
                        result = false;
                        break;

                     }//end checking for correct field names
                  }
               }
               else
               {
                  mLog.Debug("wrong type of shape file so set result to false and bail out of here");
                  result = false;
                  this.errNumber = (int)ERR.WRONG_TYPE_OF_SHAPE_FILE;
                  this.errFileName = fileName;
               }//end checking to see if it is a polygon file 
            }//end loop
         }
         catch (Exception ex)
         {

            eLog.Debug(ex);
            eLog.Debug("File Name was " + inMapFileNames[i]);
         }
         mLog.Debug("leaving validateNFieldPolylMap with a value of " + result.ToString());
         return result;
      }

		#endregion Private Methods 

		#endregion Methods 
      private enum ERR
      {
         WRONG_TYPE_OF_SHAPE_FILE,
         CAN_NOT_FIND_REQUIRED_FIELD,
         NO_FILES_FOUND_IN_DIRECTORY,
         DIRECTORY_ALREADY_IN_USE,
         NO_DIRECTORY_FOUND
      }
   }
}
