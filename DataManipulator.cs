using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geoprocessing;
using log4net;

namespace SEARCH
{
   class DataManipulator
   {
		#region Constructors (2) 

      public DataManipulator(string fileName)
      {
         myProcessor = new Geoprocessor();
        
      }

      public DataManipulator()
      {
         myProcessor = new Geoprocessor();
         myProcessor.LogHistory = true;
         myProcessor.TemporaryMapLayers = true;
         myProcessor.SetEnvironmentValue("Extent", "MAXOF");
         
         
         tempLayer1 = "\\layer1";
         tempLayer2 = "\\layer2";
         selectLayer = "\\select_lyr";
         pointLayer = "\\point_lyr";
         dissolveLayer = "\\dissolve_lyr";
         sb = new StringBuilder();
      }

      //~DataManipulator()
      //{
      //    Console.WriteLine("Data manipulator finalized");
      //}

		#endregion Constructors 

		#region Fields (9) 

      Geoprocessor myProcessor;
      ITopologicalOperator myTopOperator;
      StringBuilder sb;
      string pointLayer;
      string selectLayer;
      string tempLayer1;
      string tempLayer2;
      string dissolveLayer;
      static int index;

		#endregion Fields 

		#region Methods (50) 

		#region Public Methods (29) 

//end of buildLogger
      public void AddField(string dataType, string fieldName, object value, string layerToAddFieldTo)
      {

         AddField af = new AddField();
         af.in_table = layerToAddFieldTo;
         af.field_name = fieldName;
         af.field_type = dataType;
         this.RunProcess(af, null);
      }

      public IFeatureClass AddHomeRangePolyGon(string outFileName, IPolygon inHomeRange)
      {
         mLog.Debug("inside AddHomeRangePolyGon calling CreateEmptyFeatureClass with a file name of " + outFileName);
         IFeatureClass fc = this.CreateEmptyFeatureClass(outFileName, "polygon");
         mLog.Debug("that created a feature class lets see if it is null = " + (fc == null).ToString());
         mLog.Debug("now going to call AddPolygon for the empty feature class");
         this.AddPolyGon(fc, inHomeRange);
         return fc;

      }

      public void CleanUnionResults(string UnionPath)
      {
         IFeatureClass fc;
         IQueryFilter qf;
         int SUITABILIT;
         int OCCUP_MALE;
         int OCCUP_FEMA;
         int SUITABIL_1;
         int OCCUP_MA_1;
         int OCCUP_FE_1;
         string suitValue;
         string occMale;
         string occFemale;

         GetFeatureClassFromFileName(UnionPath, out fc, out qf);
         IField field = fc.Fields.get_Field(2);
         qf.WhereClause = field.AliasName + " = -1";
         IFeatureCursor curr = fc.Update(qf, false);
         SUITABILIT = curr.FindField("SUITABILIT");
         OCCUP_MALE = curr.FindField("OCCUP_MALE");
         OCCUP_FEMA = curr.FindField("OCCUP_FEMA");
         SUITABIL_1 = curr.FindField("SUITABIL_1");
         OCCUP_MA_1 = curr.FindField("OCCUP_MA_1");
         OCCUP_FE_1 = curr.FindField("OCCUP_FE_1");

         IFeature feat = curr.NextFeature();
         while (feat != null)
         {
            suitValue = feat.get_Value(SUITABIL_1).ToString();
            occMale = feat.get_Value(OCCUP_MA_1).ToString();
            occFemale = feat.get_Value(OCCUP_FE_1).ToString();

            feat.set_Value(SUITABILIT, suitValue);
            feat.set_Value(OCCUP_MALE, occMale);
            feat.set_Value(OCCUP_FEMA, occFemale);

            feat.Store();
            feat = curr.NextFeature();
         }
         curr.Flush();
         System.Runtime.InteropServices.Marshal.ReleaseComObject(fc);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(qf);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(curr);
      }

      public void Clip(string inFileNameClipFrom, string inFileNameClipFeature, string outFileName)
      {
          this.MakeLayer(inFileNameClipFrom, "clipFrom");
          this.MakeLayer(inFileNameClipFeature, "clipFeature");
          this.ClipFeatures("clipFrom", "clipFeature", outFileName);
      }

     
      public void CopyToAnotherlMap(string NewMapPath, string OldMapPath)
      {
         this.MakeLayer(OldMapPath, this.tempLayer1);
         this.CopyFeaturesToFeatureClass(this.tempLayer1, NewMapPath);
      }

      public IFeatureClass CreateEmptyFeatureClass(string inFileName, string featureType)
      {
         IFeatureClass fc = null;
         try
         {
            mLog.Debug("inside CreateEmptyFeatureClass the file to make is " + inFileName);
            mLog.Debug("it will be a " + featureType);
            string path;
            string fileName;
            this.GetPathAndFileName(inFileName, out path, out fileName);
            CreateFeatureclass cf = new CreateFeatureclass();
            cf.out_path = path;
            cf.out_name = fileName;
            cf.geometry_type = featureType.ToUpper();
            fc = this.RunProcessGetFeatureClass(cf, null);
         }
         catch (System.Runtime.InteropServices.COMException ex)
         {
            //System.Windows.Forms.MessageBox.Show(ex.Message);
            eLog.Debug(ex);
         }
         mLog.Debug("Leaving CreateEmptyFeatureClass");
         return fc;
      }

      public bool CreateStepMap(string inFilePath, List<IPoint> inSteps)
      {
         bool didCreateMap = true;
         mLog.Debug("inside CreateStepMap we have " + inSteps.Count.ToString() + " steps to create");
         string path;
         string fileName;
         this.GetPathAndFileName(inFilePath, out path, out fileName);
         mLog.Debug("going to create the empty feature class");
         IFeatureClass fc = this.CreateEmptyFeatureClass(path + "\\Step.shp", "point");
         if (fc != null)
         {
            mLog.Debug("that seemed to work at least the feature class is not null");
            mLog.Debug("so now add the points");
            this.AddPointsToEmptyFeatureClass(fc, inSteps);
            mLog.Debug("leaving CreateStepMap");
         }
         else
         {
            mLog.Debug("fc must have been null");
            didCreateMap = false;
         }
         mLog.Debug("leaving CreateStepMap with a value of " + didCreateMap.ToString());
         return didCreateMap;
      }

      public void DeleteAllFeatures(string inFileName)
      {
         IFeatureClass fc;
         IQueryFilter qf;
         GetFeatureClassFromFileName(inFileName, out fc, out qf);
         IFeatureCursor curr = fc.Update(null, false);
         IFeature feat = curr.NextFeature();
         while (feat != null)
         {
            feat.Delete();
            feat = curr.NextFeature();
         }
         curr.Flush();

         System.Runtime.InteropServices.Marshal.ReleaseComObject(curr);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(fc);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(qf);

      }

      public void DeleteAllFeatures2(string inFileName)
      {
         this.MakeLayer(inFileName, this.tempLayer1);
         Delete d = new Delete();
         d.in_data = this.tempLayer1;
         this.RunProcess(d, null);


      }

      public void Dissolve(string inFile, string outFile, string FieldNames)
      {
         this.MakeLayer(inFile, this.tempLayer1);
         this.DissolveFeatures(this.tempLayer1, outFile, FieldNames);
      }

      public IFeatureClass DissolveAndReturn(string inFile, string outFile, string FieldNames)
      {
         this.MakeLayer(inFile, this.tempLayer1);
         this.DissolveFeatures(this.tempLayer1, outFile, FieldNames);
         string path;
         string fileName;
         this.GetPathAndFileName(outFile, out path, out fileName);
         return this.GetFeatureClass(path, fileName);
      }

      public IFeatureClass DissolveBySexAndReturn(IFeatureClass inFE, string outFile, string sex)
      {
         IFeatureClass fc = null;
         mLog.Debug("inside DissolveBySexAndReturn for a " + sex);
         mLog.Debug("the out put will be " + outFile);
         string FieldNames = this.buildSexBasedDissolveClause(sex);
         mLog.Debug("the dissolve clause is " + FieldNames);
         if (this.DissolveFeatures(inFE, outFile, FieldNames))
         {
            mLog.Debug("dissolve was successfull");
            if (this.GetRowCount(outFile) > 0)
            {
               string path;
               string fileName;
               this.GetPathAndFileName(outFile, out path, out fileName);
               if (File.Exists(outFile))
               {
                  fc = GetFeatureClass(path, fileName);
               }
            }
         }
         else
         {
            mLog.Debug("dissovle failed");
         }
         return fc;

      }

      public IFeatureClass GetFeatureClass(string inFileName)
      {     string path = string.Empty;
      string fileName = string.Empty;
         IFeatureClass ifc = null;
         try
         {
           
            this.GetPathAndFileName(inFileName, out path, out fileName);
            IWorkspaceFactory wrkSpaceFactory = new ShapefileWorkspaceFactory();
            IFeatureWorkspace featureWorkspace = null;
            featureWorkspace = (IFeatureWorkspace)wrkSpaceFactory.OpenFromFile(path, 0);
            ifc = featureWorkspace.OpenFeatureClass(fileName);
         }
         catch (COMException COMEx)
         {
            eLog.Debug(COMEx);
            eLog.Debug("Orginal File Name was " + inFileName);
            eLog.Debug("so the path is  " + path);
            eLog.Debug("just the file name is" + fileName);
            //System.Windows.Forms.MessageBox.Show(COMEx.GetBaseException().ToString(), "COM Error: " + COMEx.ErrorCode.ToString());
         }

         catch (System.Exception ex)
         {
            eLog.Debug(ex);
            //System.Windows.Forms.MessageBox.Show(ex.Source + " ");//+ ex.InnerException.ToString());
         }

         return ifc;
      }

      public IFeatureClass GetFeatureClass(string path, string fileName)
      {
         IFeatureClass ifc = null;
         try
         {

            IWorkspaceFactory wrkSpaceFactory = new ShapefileWorkspaceFactory();
            IFeatureWorkspace featureWorkspace = null;
            featureWorkspace = (IFeatureWorkspace)wrkSpaceFactory.OpenFromFile(path, 0);
            ifc = featureWorkspace.OpenFeatureClass(fileName);
         }
         catch (COMException COMEx)
         {
            eLog.Debug(COMEx);
            //System.Windows.Forms.MessageBox.Show(COMEx.GetBaseException().ToString(), "COM Error: " + COMEx.ErrorCode.ToString());
         }

         catch (System.Exception ex)
         {
            eLog.Debug(ex);
            //System.Windows.Forms.MessageBox.Show(ex.Source + " ");//+ ex.InnerException.ToString());
         }

         return ifc;
      }

      public IFeatureClass GetNewlyAddedToSocialMapPolygons(string inFileName, string outFileName)
      {
         string path;
         string fileName;
         this.GetPathAndFileName(outFileName, out path, out fileName);
         this.MakeLayer(inFileName, this.selectLayer);
         string sqlWhereClause = "FID_availa >= 0";
         this.SelectByValue(this.selectLayer, sqlWhereClause);
         this.CopyFeaturesToFeatureClass(this.selectLayer, outFileName);
         return this.GetFeatureClass(path, fileName);

      }

      public int GetRowCount(string inFileName)
      {
         GetCount gc = new GetCount();
         this.MakeLayer(inFileName, this.tempLayer1);
         gc.in_rows = this.tempLayer1;
         this.RunProcess(gc, null);
         return gc.row_count;
      }

      public int GetRowCount(IFeatureClass inFC)
      {
         GetCount gc = new GetCount();
         gc.in_rows = inFC;
         this.RunProcess(gc, null);
         return gc.row_count;
      }

      public IFeatureClass GetSuitablePolygons(string inFileName, string sex)
      {
         mLog.Debug("inside GetSuitablePolygons from " + inFileName + " for a " + sex);
         string sqlWhereClause = this.buildSexBasedWhereClause(sex);
         mLog.Debug("my where clause is " + sqlWhereClause);
         this.MakeLayer(inFileName, this.selectLayer);
         return this.SelectByValue(this.selectLayer, sqlWhereClause);
      }

      public IFeatureClass GetSuitablePolygons(string inFileName, string sex, string outFileName)
      {
         IFeatureClass fc = null;
         string sqlWhereClause = this.buildSexBasedWhereClause(sex);
         this.MakeLayer(inFileName, this.selectLayer);
         this.SelectByValue(this.selectLayer, sqlWhereClause);
         if (this.CopyFeaturesToFeatureClass(this.selectLayer, outFileName))
         {
            fc = this.GetFeatureClass(outFileName);
         }

         return fc;
      }

      public IFeatureClass IntersectFeatures(string inFeaturesNames, string outFeatureName)
      {
         mLog.Debug("inisde IntersectFeatures setting the features");
         mLog.Debug("inFeaturs are " + inFeaturesNames);
         mLog.Debug("out feature is " + outFeatureName);
         Intersect i = new Intersect();
         i.in_features = inFeaturesNames;
         i.out_feature_class = outFeatureName;
         return this.RunProcessGetFeatureClass(i, null);
      }

      public IFeatureClass IntersectFeatures(string inFeaturesNames, string outFeatureName, string ignoreMessage)
      {
         Intersect i = new Intersect();
         i.in_features = inFeaturesNames;
         i.out_feature_class = outFeatureName;
         return this.RunProcessGetFeatureClass(i, null, ignoreMessage);
      }

      public void JoinLayers(string layerName1, string layerName2)
      {

         SpatialJoin sj = new SpatialJoin();
         sj.target_features = layerName1;
         sj.join_features = layerName2;
         sj.out_feature_class = @"C:\map\stepmaps.shp";
         sj.match_option = "IS_WITHIN";
         this.RunProcess(sj, null);
         //return fc;
      }

      public bool MakeTimeStep(string inFileName, string outFileName, IPoint from, IPoint to)
      {
         IFeatureClass fc = null;
         IPolyline line = null;
         string path;
         string fileName;
         bool result = true;

         fc = this.CreateEmptyFeatureClass(inFileName, "POLYLINE");
         line = this.MakePolyLine(from, to);
         this.addGeometry(fc, line as IGeometry);

         ESRI.ArcGIS.AnalysisTools.Buffer buf = new ESRI.ArcGIS.AnalysisTools.Buffer();
         buf.buffer_distance_or_field = 100;
         buf.line_end_type = "ROUND";
         buf.in_features = fc;
         buf.out_feature_class = outFileName;
         this.RunProcess(buf, null);

         return result;

      }
      public bool MakeDissolvedTimeStep(string inFullFilePath, string dissovlePath, IPolygon inPoly1, IPolygon inPoly2)
      {
         IFeatureClass fc = null;
         string path;
         string fileName;
         bool result = true;
         try
         {
            mLog.Debug("inside MakeDissolvedTimeStep");
            mLog.Debug("file name is " + inFullFilePath);
            mLog.Debug("dissolvePath is " + dissovlePath);
            mLog.Debug("clean up the current step map");
            this.DeleteAllFeatures(inFullFilePath);
            GetPathAndFileName(inFullFilePath, out path, out fileName);
            fc = this.GetFeatureClass(path, fileName);
            this.AddPolyGon(fc, inPoly1);
            this.AddPolyGon(fc, inPoly2);
            this.MakeLayer(inFullFilePath, this.tempLayer1);
            mLog.Debug("Calling check lock");
            this.DissolveFeatures(this.tempLayer1, dissovlePath, "Id");


         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
            result = false;
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(fc);
         }
         return result;
      }

      public void makeHomeRangeSelectionMap(string stepMapName, string animalMemoryMapName)
      {
         this.MakeLayer(stepMapName, this.tempLayer1);
         this.MakeLayer(animalMemoryMapName, this.tempLayer2);
         string path = System.IO.Path.GetDirectoryName(animalMemoryMapName);
         SpatialJoin sj = new SpatialJoin();
         sj.target_features = this.tempLayer1;
         sj.join_features = this.tempLayer2;
         sj.match_option = "IS_WITHIN";
         sj.out_feature_class = path + "\\HomeSteps.shp";
         this.RunProcess(sj, null);

      }

      public IPolyline MakePolyLine(IPoint from, IPoint to)
      {
         IPolyline line = new PolylineClass();
         line.FromPoint = from;
         line.ToPoint = to;
         return line;
      }

      public void MultiToSinglePart(string inFileName, string outFileName)
      {
         MultipartToSinglepart ms = new MultipartToSinglepart();
         ms.in_features = inFileName;
         ms.out_feature_class = outFileName;
         this.RunProcess(ms, null);
      }

      public void RemoveExtraFields(string inFullFilePath, string ListOfFields)
      {
         DeleteField d = new DeleteField();
         this.MakeLayer(inFullFilePath, this.tempLayer1);
         d.in_table = this.tempLayer1;
         d.drop_field = ListOfFields;
         this.RunProcess(d, null);

      }

      public IFeatureClass SetSuitableSteps(string inPointFileName, List<IPoint> inPoints, string inMemoryMap)
      {

         IFeatureClass fc = null;
         fc = this.CreateEmptyFeatureClass(inPointFileName, "point");
         this.AddPointsToEmptyFeatureClass(fc, inPoints);
         this.MakeLayer(inPointFileName, this.tempLayer1);
         this.MakeLayer(inMemoryMap, this.tempLayer2);
         this.JoinLayers(this.tempLayer1, this.tempLayer2);
         return fc;
      }

      public bool UnionAnimalClipData(string inAnimalPath, string inClipPath, string outPutFileName)
      {
         bool result = true;

         try
         {
            mLog.Debug("");
            mLog.Debug("inside UnionAnimalClipData for " + inAnimalPath);
            this.MakeLayer(inAnimalPath, this.tempLayer1);
            this.MakeLayer(inClipPath, this.tempLayer2);
            //this.UnionFeatures(this.tempLayer1 + "; " + this.tempLayer2, outPutFileName);
            this.UnionFeatures(this.tempLayer2 + "; " + this.tempLayer1, outPutFileName);
            this.CleanUnionResults(outPutFileName);
         }
         catch (System.Exception ex)
         {

            eLog.Debug(ex);
            result = false;
         }
         return result;
      }

      public void UnionHomeRange(string inTempHomeRangePath, string inSocialMapPath, string outPutFileName)
      {
         this.MakeLayer(inTempHomeRangePath, this.tempLayer1);
         this.MakeLayer(inSocialMapPath, this.tempLayer2);
         this.UnionFeatures(this.tempLayer1 + "; " + this.tempLayer2, outPutFileName);
      }

		#endregion Public Methods 
		#region Private Methods (21) 

      private log4net.ILog mLog = LogManager.GetLogger("dataLogger");
      private log4net.ILog eLog = LogManager.GetLogger("Error");
       
       private void AddPointsToEmptyFeatureClass(IFeatureClass inFC, List<IPoint> inPoints)
      {
         mLog.Debug("inside AddPointsToEmptyFeatureClass");
         IFeatureBuffer featureBuffer = inFC.CreateFeatureBuffer();
         IFeatureCursor insertCursor = inFC.Insert(true);
         foreach (IPoint p in inPoints)
         {
            featureBuffer.Shape = p;
            insertCursor.InsertFeature(featureBuffer);
         }
         insertCursor.Flush();
         System.Runtime.InteropServices.Marshal.ReleaseComObject(featureBuffer);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
         mLog.Debug("leaving AddPointsToEmptyFeatureClass");
      }

      private void AddPolyGon(IFeatureClass inFC, IPolygon inPoly)
      {
         IFeature feature;
         feature = inFC.CreateFeature();
         feature.Shape = inPoly;
         feature.Store();
         System.Runtime.InteropServices.Marshal.ReleaseComObject(feature);

      }

      private void addGeometry(IFeatureClass inFC, IGeometry inGeo)
      {
         IFeature feature;
         feature = inFC.CreateFeature();
         feature.Shape = inGeo;
         feature.Store();
         System.Runtime.InteropServices.Marshal.ReleaseComObject(feature);

      }


      private string buildSexBasedDissolveClause(string inSex)
      {
         sb.Remove(0, sb.Length);
         sb.Append("SUITABILIT;");
         if (inSex.Equals("male", StringComparison.CurrentCultureIgnoreCase))
            sb.Append("OCCUP_MALE;Delete");
         else
            sb.Append("OCCUP_FEMA;Delete");
         return sb.ToString();

      }

      private string buildSexBasedWhereClause(string inSex)
      {
         sb.Remove(0, sb.Length);
         sb.Append("SUITABILIT = 'Suitable' ");
         if (inSex.Equals("male", StringComparison.CurrentCultureIgnoreCase))
            sb.Append("And OCCUP_MALE = 'none'");
         else
            sb.Append("And OCCUP_FEMA = 'none'");
         return sb.ToString();
      }

      private void ClipFeatures(string clipFromLayer, string clipFeatureLayer, string outFeatureClassName)
      {
         int numTrys = 0;
         ESRI.ArcGIS.AnalysisTools.Clip c = new ESRI.ArcGIS.AnalysisTools.Clip();
         c.in_features = clipFromLayer;
         c.clip_features = clipFeatureLayer;
         c.cluster_tolerance = "0.001 meters";
         c.out_feature_class = outFeatureClassName;
         while (numTrys<=10)
         {
             if (numTrys == 10)
             {
                 //System.Windows.Forms.MessageBox.Show("Tried clip 10 times and failed");
             }
             else
             {
                 if (RunProcess(c, null))
                 {
                     mLog.Debug("Passed clip features after " + (numTrys + 1).ToString() + " trys");
                     numTrys = 999;
                 }
                 else
                 {
                     numTrys++;
                     mLog.Debug("****************************Failed dissolve " + numTrys + " times");
                     mLog.Debug("Attempting repair geometry of clipFromLayer");
                     RepairGeometry r = new RepairGeometry();
                     r.in_features = clipFromLayer;
                     RunProcess(r, null);
                     mLog.Debug("Attempting repair geometry of clipFeatureLayer");
                     RepairGeometry r2 = new RepairGeometry();
                     r2.in_features = clipFeatureLayer;
                     RunProcess(r2, null);
                     mLog.Debug("Reseting geoprocessor");
                     myProcessor = null;
                     myProcessor = new Geoprocessor();
                 }
             }
         }
      }

      private bool CopyFeaturesToFeatureClass(string inLayer, string RecievingFeatureClass)
      {

         CopyFeatures cf = new CopyFeatures();
         cf.in_features = inLayer;
         cf.out_feature_class = RecievingFeatureClass;
         return RunProcess(cf, null);
      }

      private void DissolveFeatures(string layerName, string outName, string fieldName)
      {
         int numTrys = 0;
         Dissolve d = new Dissolve();
         d.in_features = layerName;
         d.out_feature_class = outName;
         d.dissolve_field = fieldName as object;
         d.multi_part = "SINGLE_PART";
         while (numTrys <= 10)
         {
             if (numTrys == 10)
             {
                 //System.Windows.Forms.MessageBox.Show("Tried dissolve 10 times and failed");
             }
             else
             {
                 if (RunProcess(d, null))
                 {
                     mLog.Debug("Passed dissolve features after " + (numTrys + 1).ToString() + " trys");
                     numTrys=999;
                 }
                 else
                 {
                     numTrys++;
                     mLog.Debug("****************************Failed dissolve " + numTrys + " times");
                     mLog.Debug("Attempting repair geometry");
                     RepairGeometry r = new RepairGeometry();
                     r.in_features = layerName;
                     RunProcess(r, null);
                     mLog.Debug("Reseting geoprocessor");
                     myProcessor = null;
                     myProcessor = new Geoprocessor();
                 }
             }
         }
      }

      private bool DissolveFeatures(IFeatureClass inFC, string outName, string fieldName)
      {
         
         bool didDissolve = true;
         mLog.Debug("inside Dissolve Features");
         Dissolve d = new Dissolve();
         d.in_features = inFC;
         d.out_feature_class = outName;
         d.dissolve_field = fieldName as object;
         d.multi_part = "SINGLE_PART";
         didDissolve = RunProcess(d, null);
         mLog.Debug("didDissolve = " + didDissolve.ToString());
         return didDissolve;
      }

      private void GetFeatureClassFromFileName(string inFileName, out IFeatureClass fc, out IQueryFilter qf)
      {
          fc = null;
          qf = null;
         try 
	{	        
		MakeFeatureLayer makefeaturelayer = new MakeFeatureLayer();
         makefeaturelayer.in_features = inFileName;
         makefeaturelayer.out_layer = "tempLayer";
         IGeoProcessorResult result = (IGeoProcessorResult)myProcessor.Execute(makefeaturelayer, null);
         IGPUtilities util = new GPUtilitiesClass();
         util.DecodeFeatureLayer(result.GetOutput(0), out fc, out qf);
	}
	catch (Exception ex)
	{
		
        //System.Windows.Forms.MessageBox.Show(ex.ToString());
	}
      }

      private void GetPathAndFileName(string inFullFilePath, out string path, out string fileName)
      {
         path = System.IO.Path.GetDirectoryName(inFullFilePath);
         fileName = System.IO.Path.GetFileName(inFullFilePath);

      }

      private void MakeLayer(string inFileName, string outLayerName)
      {
         MakeFeatureLayer makefeaturelayer = new MakeFeatureLayer();
         makefeaturelayer.in_features = inFileName;
         makefeaturelayer.out_layer = outLayerName;
         this.RunProcess(makefeaturelayer, null);
      }

      private void RemoveAllPolygons(ref IFeatureClass inFeatureClass)
      {
         IFeatureCursor tmpCur;
         IFeature tmpFeature;
         tmpCur = inFeatureClass.Update(null, false);
         tmpFeature = tmpCur.NextFeature();

         while (tmpFeature != null)
         {
            tmpFeature.Delete();
            tmpFeature = tmpCur.NextFeature();
         }
         tmpCur.Flush();
         int j = inFeatureClass.FeatureCount(null);




      }

      // Function for returning the tool messages.
      private bool ReturnMessages(Geoprocessor gp)
      {
         bool noErrors = true;

         try
         {
            if (gp.MessageCount > 0)
            {
               for (int Count = 0; Count <= gp.MessageCount - 1; Count++)
               {
                  string s = gp.GetMessage(Count);
                  if (s.Contains("ERROR"))// || s.Contains("WARNING 000117"))
                  {
                     if (s.Contains("Virmem low memory"))
                        //System.Windows.Forms.MessageBox.Show(s);
                     noErrors = false;
                  }
                  mLog.Debug(s);
               }
            }
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
         return noErrors;

      }

      /// <summary>
      /// Function for returning the tool messages.
      /// </summary>
      /// <param name="gp">The Geoprocessor to get the Messages from</param>
      /// <param name="MessageToIgnore">Messages that we do not want to log as errors</param>
      private void ReturnMessages(Geoprocessor gp, string MessageToIgnore)
      {
         bool hasError = false;
         try
         {
            if (gp.MessageCount > 0)
            {
               for (int Count = 0; Count <= gp.MessageCount - 1; Count++)
               {
                  string s = gp.GetMessage(Count);
                  if (s.Contains("ERROR") && !s.Contains(MessageToIgnore))
                  {
                     if (s.Contains("Virmem low memory"))
                        //System.Windows.Forms.MessageBox.Show(s);

                     hasError = true;

                  }
                  mLog.Debug(s);
                  if (hasError)
                  {
                     eLog.Debug(s);
                     //hasError = false;
#if DEBUG
                     System.Windows.Forms.MessageBox.Show("Error in DataManipulator");
#endif
                  }

               }
            }
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }

      }

      private bool RunProcess(IGPProcess inProcess, ITrackCancel inCancel)
      {
         bool wasSuccessful = false;
         //try
         //{
            string toolbox = inProcess.ToolboxName;
            mLog.Debug("inside run process");
            mLog.Debug("the process I want to run is " + inProcess.ToolName);
            mLog.Debug("the tool box is " + toolbox);
            myProcessor.OverwriteOutput = true;
            myProcessor.Execute(inProcess, null);
            wasSuccessful = ReturnMessages(myProcessor);
#if DEBUG
            //if (!wasSuccessful)
             //  System.Windows.Forms.MessageBox.Show("Data error");
#endif

         //}
         //catch (Exception ex)
         //{
         //   wasSuccessful = false;
         //   Console.WriteLine(ex.Message);
         //   ReturnMessages(myProcessor);
         //}
         return wasSuccessful;
      }

      private IFeatureClass RunProcessGetFeatureClass(IGPProcess inProcess, ITrackCancel inCancel)
      {
         IFeatureClass fc = null;
         IQueryFilter qf = null;
         try
         {
            string toolbox = inProcess.ToolboxName;
            mLog.Debug("inside run process");
            mLog.Debug("the process I want to run is " + inProcess.ToolName);
            mLog.Debug("the tool box is " + toolbox);
            myProcessor.OverwriteOutput = true;
            IGeoProcessorResult result = (IGeoProcessorResult)myProcessor.Execute(inProcess, null);
            IGPUtilities util = new GPUtilitiesClass();
            util.DecodeFeatureLayer(result.GetOutput(0), out fc, out qf);
            ReturnMessages(myProcessor);

         }
         catch (Exception ex)
         {
            eLog.Debug(ex);
            ReturnMessages(myProcessor);
         }
         return fc;
      }

      private IFeatureClass RunProcessGetFeatureClass(IGPProcess inProcess, ITrackCancel inCancel, string ignoreMessage)
      {
         IFeatureClass fc = null;
         IQueryFilter qf = null;
         IGeoProcessorResult result = null;
         IGPUtilities util = null;
         try
         {
            string toolbox = inProcess.ToolboxName;
            mLog.Debug("inside run process");
            mLog.Debug("the process I want to run is " + inProcess.ToolName);
            mLog.Debug("the tool box is " + toolbox);
            myProcessor.OverwriteOutput = true;
            result = (IGeoProcessorResult)myProcessor.Execute(inProcess, null);
            ReturnMessages(myProcessor);
            //if result is null then there are no viable areas
            if (result != null)
            {
               util = new GPUtilitiesClass();
               util.DecodeFeatureLayer(result.GetOutput(0), out fc, out qf);
               ReturnMessages(myProcessor, ignoreMessage);
            }


         }
         catch (Exception ex)
         {
            eLog.Debug(ex);
            ReturnMessages(myProcessor);
         }
         return fc;
      }

      private IFeatureClass SelectByValue(string inLayerName, string whereClause)
      {
         IQueryFilter qf = new QueryFilterClass();
         qf.WhereClause = whereClause;
         SelectLayerByAttribute selectByValue = new SelectLayerByAttribute();
         selectByValue.in_layer_or_view = inLayerName;
         selectByValue.selection_type = "NEW_SELECTION";
         selectByValue.where_clause = whereClause;
         return this.RunProcessGetFeatureClass(selectByValue, null);
      }

      private void SelectByValue(string inLayerName, string whereClause, string outLayerName)
      {
         IQueryFilter qf = new QueryFilterClass();
         qf.WhereClause = whereClause;
         SelectLayerByAttribute selectByValue = new SelectLayerByAttribute();
         selectByValue.in_layer_or_view = inLayerName;
         selectByValue.selection_type = "NEW_SELECTION";
         selectByValue.where_clause = whereClause;
         selectByValue.out_layer_or_view = outLayerName;

         this.RunProcess(selectByValue, null);
         this.CopyFeaturesToFeatureClass(selectLayer, outLayerName);

      }
     

      private void UnionFeatures(string LayerList, string fc)
      {
         mLog.Debug("inside UnionFeatures going to make " + fc);
         mLog.Debug("my layer list is " + LayerList);
         Union u = new Union();
         mLog.Debug("made new union tool");
         u.in_features = LayerList;
         mLog.Debug("just set the layers");
         u.out_feature_class = fc;
         mLog.Debug("set the feature class");
         mLog.Debug("Calling Run Process");
         this.RunProcess(u, null);
         mLog.Debug("back from runprocess");
      }

		#endregion Private Methods 

		#endregion Methods 
   }
}