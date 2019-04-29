using System;
using System.IO;
using System.Collections.Specialized;
using ESRI.ArcGIS.Geodatabase;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using System.Runtime.InteropServices;
using log4net;

namespace SEARCH
{
   /// <summary>
   /// Summary description for AnimalMap.
   /// </summary>
   public class AnimalMap:Map
   {


      #region MemberVariables
      private IBasicGeoprocessor ibg;
      private IFeatureClass mStepFeatureClass;
      private IFeatureClass mSuitableFeatureClass;
     
      private IFeatureCursor featCursor;
      private IFeature f;
      private IMap myMap;
      private IFeatureWorkspace shapeWksp;
      private Map mMySocialMap;
      private ITable mStepTable;
      private IQueryFilter qf;
      private IGeometryDef mGeoDef;
      private string mErrMessage;
      private const int TRUE = 1;
      private const int FALSE = 0;
      private log4net.ILog mLog = LogManager.GetLogger("animalMap");
      private log4net.ILog eLog = LogManager.GetLogger("Error");
      
      #endregion

      #region publicMethods
      public AnimalMap()
      {
         //ibg = new BasicGeoprocessorClass();
         myMap = new MapClass();
         qf = new QueryFilterClass();
         qf.WhereClause = "SUITABILIT = 'Suitable'";
         
      }
      public AnimalMap(string inName,string path,IGeometryDef inGeoDef):this()
      {
         mLog.Debug("my name is " + inName);
         mLog.Debug("my path is " + path + "\\" + inName);
         this.myFileName = inName;
         this.myPath = path + "\\" + inName;
         this.FullFileName = this.myPath + "\\" + inName + ".shp";
         this.mGeoDef = inGeoDef;
         this.makeMap(path,inName,inGeoDef);
         
      }
     
      public void addPolygon (IPolygon inPoly)
      {
         IFeature feature = null;
         IFields fields;
         int index = 0;
         try
         {
            feature = this.mySelf.CreateFeature();
            fields = feature.Fields;
            index = fields.FindField("CurrTime");
            if (index >=0)
               feature.set_Value(index,1);
            index = fields.FindField("Available");
            if (index >=0)
               feature.set_Value(index,0);
             feature.Shape = inPoly;
            feature.Store();
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
      }
      public void addPolygon (IPolygon inPoly, int timeStep)
      {
         IFeature feature = null;
         IFields fields;
         int index = 0;
         
       
         try
         {
            feature = this.mySelf.CreateFeature();
            fields = feature.Fields;
            index = fields.FindField("CurrTime");
            if (index >=0)
               feature.set_Value(index,timeStep);
            feature.Shape = inPoly;
            feature.Store();
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
      }
      
     

      private void getFieldID (ref StringCollection sc)
      {
         featCursor = this.mySelf.Search(null,true);
         f = featCursor.NextFeature();
         while(f!=null)
         {
            sc.Add(f.OID.ToString());
            f = featCursor.NextFeature();
         }
      }

      private void deleteFields(ref StringCollection sc)
      {  
         
        
         featCursor = this.mySelf.Update(null,false);
         f = featCursor.NextFeature();
         while(f!=null)
         {
            if(sc.Contains( f.OID.ToString()))
            {
               featCursor.DeleteFeature();
            }
            f = featCursor.NextFeature();
         }

      }

      public void dissolveAvailablePolygons(string inTimeStep, ITable inputTable)
      {

          ITable t = null;
         IBasicGeoprocessor bg = new ESRI.ArcGIS.Carto.BasicGeoprocessorClass();
         int numRows = 0;
         try
         {
            
            this.removeTimeStepMaps();
            //inputTable = this.getTable();
            
            numRows = inputTable.RowCount(null);
            this.buildOutShapeFileName();
            this.buildWorkSpaceName();
            this.buildDataSetName("TimeStep"+inTimeStep);
            //this.removeAllPolygons();
            t = bg.Dissolve(inputTable,false,"Available","Dissolve.Shape,Minimum.Available",dsName);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            
            eLog.Debug(ex);
         }
      }

      public void explode (string timeStep)
      {
         try
         {
            //string newFileName = "explode" + timeStepPath;
            IFeatureClass ifc = getShapeFile("TimeStep"+timeStep); 

      
            // Copy each feature from the original feature class to the new feature class
            IFeatureCursor loopCursor = ifc.Search(null,false);
            IFeatureCursor insertCurr = ifc.Insert(false);
            IFeatureBuffer insertBuff = ifc.CreateFeatureBuffer();
            IFeature feature;
            IGeometryCollection geometryColl;
            
            while ((feature = loopCursor.NextFeature()) != null)
            {
               geometryColl = feature.Shape as IGeometryCollection;
               IPolygon4 poly4 = feature.Shape as IPolygon4;
               IGeometryCollection gc = poly4.ConnectedComponentBag as IGeometryCollection;
               for (int i = 0; i <poly4.ExteriorRingCount; i++) 
               {
                  InsertFeature(insertCurr, insertBuff, feature,gc.get_Geometry(i) );
               }
               feature.Delete();
            }
            
              System.Runtime.InteropServices.Marshal.ReleaseComObject(loopCursor);
              System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCurr);
              System.Runtime.InteropServices.Marshal.ReleaseComObject(insertBuff);
           // this.updateSpatialIndex();
               
         }
         
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }
     
      public void makeSinglePart(string timeStep)
      {
         IFeatureCursor searchCurr;
         IFeatureCursor insertCurr;
         IFeatureBuffer insertBuff;
         IFeature f;
         IGeometryCollection geoColl;
         int i = 1;
         mLog.Debug("inside animal map make single part");
         IFeatureClass ifc = getShapeFile("TimeStep"+timeStep); 
         mLog.Debug("file we are going to work with is " + ifc.AliasName);

         searchCurr = ifc.Search(null,false);
         mLog.Debug("there are " + ifc.FeatureCount(null).ToString() + " polygons to work with");
         insertCurr = ifc.Insert(true);
         insertBuff = ifc.CreateFeatureBuffer();
         f = searchCurr.NextFeature();

         geoColl = (IGeometryCollection)f.Shape;
         mLog.Debug("we have " + geoColl.GeometryCount + " geometries to work on");
         if (timeStep == "1")
         {
            i = 0;
         }
         
         for(i=0;i<geoColl.GeometryCount; i++)
         {
            this.InsertFeature(insertCurr,insertBuff,f,geoColl.get_Geometry(i));
         }
         insertCurr.Flush();
         f.Delete();
      }

      public IFeatureClass getShapeFile(string fileName)
      {
         IFeatureClass ifc=null;
         try
         {
            IWorkspaceFactory wrkSpaceFactory = new ShapefileWorkspaceFactory();
            IFeatureWorkspace featureWorkspace=null;
            featureWorkspace = (IFeatureWorkspace)wrkSpaceFactory.OpenFromFile(this.myPath,0);
            ifc = featureWorkspace.OpenFeatureClass(fileName);
         }
         catch (COMException COMEx)
         {
        //    System.Windows.Forms.MessageBox.Show(COMEx.GetBaseException().ToString(),"COM Error: " + COMEx.ErrorCode.ToString()); 
         }

         catch (System.Exception ex)
         {
          //  System.Windows.Forms.MessageBox.Show (ex.Source + " " );//+ ex.InnerException.ToString());
         }

         return ifc;
      }
      public new void removePolygon(IFeature inF)
      {
         base.removePolygon(inF);
      }
      public void resetCurrStep()
      {
         int i = 0;
         mLog.Debug("inside reset currstep for animal map making new query filter class");
         IQueryFilter qf = new QueryFilterClass();
         qf.WhereClause="CurrTime = 1";
         mLog.Debug("now getting the cursor ");
         IFeatureCursor fc = this.mySelf.Search(qf,false);
         mLog.Debug("getting ready to enter the loop");
         IFeature f = fc.NextFeature();
         if (f != null)
         {
            int currTimeIndex = f.Fields.FindField("CurrTime");
            while(f!=null)
            {  
               mLog.Debug("loop number is " + i.ToString());
               f.set_Value(currTimeIndex,0);
               f.Store();
               f = fc.NextFeature();
            }
         }
          
         
      }
     
      public void resetBaseMap()
      {
         //HACK try to clean up the old files
         Directory.Delete(this.myPath,true);
         makeMap(this.myPath,this.myFileName,this.mGeoDef);
      }
      /********************************************************************************
       *   Function name   : makeMap
       *   Description     : makes the animal movement maps. One for each animal.
       *   Return type     : void 
       *   Argument        : string shapePath
       *   Argument        : string shapeFileName
       * ********************************************************************************/
      public bool makeMap (string shapePath, string shapeFileName, IGeometryDef inGeoDef)
      {
         bool success = true;
         try
         {
            shapePath = shapePath + "\\" + shapeFileName;
           
            Directory.CreateDirectory(shapePath);
            this.myPath = shapePath;
            IWorkspaceFactory shpWkspFactory = new ShapefileWorkspaceFactoryClass();
            IPropertySet connectionProperty = new PropertySetClass();
            IGeometryDefEdit geoDef = new GeometryDefClass();
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            IField field = new FieldClass() ;
            IFieldEdit fieldEdit = (IFieldEdit)field;
            
            connectionProperty.SetProperty ("DATABASE",shapePath);
            shapeWksp = (IFeatureWorkspace) shpWkspFactory.Open(connectionProperty,0);
          
            fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "OBJECTID";
            fieldEdit.AliasName_2 = "OBJECTID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(fieldEdit);

            fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = inGeoDef;
            fieldEdit.Required_2 = true;
            fieldsEdit.AddField(fieldEdit);
            
            fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "ID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fieldsEdit.AddField(fieldEdit);

            fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "CurrTime";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fieldEdit.DefaultValue_2 =0;
            fieldsEdit.AddField(fieldEdit);

            fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "Available";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fieldEdit.DefaultValue_2 =0;
            fieldsEdit.AddField(fieldEdit);

            
            this.mySelf = shapeWksp.CreateFeatureClass(shapeFileName,fieldsEdit,null,null,esriFeatureType.esriFTSimple,"Shape","");
            System.Runtime.InteropServices.Marshal.ReleaseComObject(shpWkspFactory);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(connectionProperty);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(geoDef);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(fields);


            
         }
         catch(System.Exception ex)
         {
            success = false;
            if (ex.Source == "ESRI.ArcGIS.Geodatabase")
            {
               mErrMessage = "That Directory is full with maps already";
               throw new System.Exception("That Directory is full with maps already");
            }
           
            eLog.Debug(ex);
         }
         return success;
      }

    
      
         
      #endregion

      #region privateMethods
      private void AddFields(IFeatureBuffer featureBuffer, IFeature feature)
      {
         // Copy the attributes of the orig feature the new feature
         IRowBuffer rowBuffer = (IRowBuffer) featureBuffer;
         IFields fieldsNew = rowBuffer.Fields;

         IFields fields = feature.Fields;
         for (int i = 0; i <= fields.FieldCount - 1; i++)
         {
            IField field = fields.get_Field(i);
            if ((field.Type != esriFieldType.esriFieldTypeGeometry) &&
               (field.Type != esriFieldType.esriFieldTypeOID))
            {
               int  intFieldIndex = fieldsNew.FindField(field.Name);
               if (intFieldIndex != -1)
               {
                  featureBuffer.set_Value(intFieldIndex, feature.get_Value(i));
               }
            }
         }
      }


      private ILayer getIntersectLayer()
      {
         try
         {
            this.myFeatureLayer.FeatureClass = this.mSuitableFeatureClass;
            this.myFeatureLayer.Name = this.mStepFeatureClass.AliasName;
            this.myMap.AddLayer(this.myFeatureLayer);
            
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         return myMap.get_Layer(0);

      }
      private ILayer getStepLayer()
      {
        
         this.myFeatureLayer.FeatureClass = this.mStepFeatureClass;
         this.myFeatureLayer.Name = this.mStepFeatureClass.AliasName;
         this.myMap.AddLayer(this.myFeatureLayer);
         return myMap.get_Layer(0);

      }
      private ITable getStepTable()
      {
         ITable t;
         t = (ITable)this.getStepLayer();
         return t;

      }
      private ITable getIntersectTable()
      {
         ITable t;
         t = (ITable)this.getIntersectLayer();
         return t;

      }

      public   ITable getUnionTable()
      {
        
         IFeatureClass mUnionFeatureClass=null;
         IFeatureLayer fl=null;
         ILayer l=null;
         try
         {
            //first make sure there is a file to open
            if (File.Exists(this.FullFileName + "TimeStep.shp"))
            {
            
               mUnionFeatureClass=Map.openFeatureClass(this.myPath,this.mySelf.AliasName + "TimeStep");
               myMap.ClearLayers();
               fl = new FeatureLayerClass();
               fl.FeatureClass = mUnionFeatureClass;
               fl.Name = mUnionFeatureClass.AliasName;
               myMap.AddLayer(fl);
               l=myMap.get_Layer(0);
               
            }
            
         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            eLog.Debug(ex);
         }
         return (ITable)l;
         
      }
      
      private void InsertFeature(IFeatureCursor inCurr,
         IFeatureBuffer inBuff, 
         IFeature origF, 
         IGeometry inGeo)
      {
         mLog.Debug("inside insert feature");
         IField tmpField;
         IFields fieldColl;
                 
         //object Missing = Type.Missing;
         fieldColl = origF.Fields;
         mLog.Debug("this feature has " + fieldColl.FieldCount.ToString() + " fields to work with");
         for (int i=0;i<fieldColl.FieldCount;i++)
         {
            tmpField = fieldColl.get_Field(i);
            mLog.Debug("this field is " + tmpField.Name);
            mLog.Debug("this field type is " + tmpField.VarType.ToString());
                        
            if (tmpField.Type != esriFieldType.esriFieldTypeGeometry &&
               tmpField.Type != esriFieldType.esriFieldTypeOID &&
               tmpField.Editable)
            {
               mLog.Debug(inBuff.get_Value(i).ToString());
               mLog.Debug("seting the value to " + origF.get_Value(i));
               inBuff.set_Value(i,origF.get_Value(i));
              
            }
         }
         

         
         inBuff.Shape = inGeo;
         inCurr.InsertFeature(inBuff);
         inCurr.Flush();
      }
      public void removeExtraFields()
      {
         IFeature feat;
         IFeatureCursor fc;
         int index;
         object fieldValue;
         try
         {
            fc = this.mySelf.Search(null,false);
            feat = fc.NextFeature();
            while(feat != null)
            {
               index = feat.Fields.FindField("ID");
               fieldValue = feat.get_Value(index);
               if (fieldValue.ToString() == this.mySelf.AliasName )
               {
                  this.removePolygon(feat);
               }
               feat = fc.NextFeature();
            }

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }
      private void removeTimeStepMaps()
      {
         string []fileNames;
         try
         { 
            fileNames = Directory.GetFiles(myPath,"TimeStep*");
            for(int i=0;i<fileNames.Length;i++)
            {
               File.Delete(fileNames[i]);
            }

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
        
      }
      private void updateSpatialIndex()
      {
         try
         {
            IFieldsEdit fieldsEdit = new FieldsClass();
            IIndexEdit indexEditor = new IndexClass();
            fieldsEdit.AddField(this.mySelf.Fields.get_Field(1));
            indexEditor.Name_2 = "Shape";
            indexEditor.Fields_2 = fieldsEdit;
            this.mySelf.AddIndex((IIndex)indexEditor);
         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            eLog.Debug(ex);
         }
      }
      private void validateFields(ref IFields shpFields, ref IFields covFields,ref IEnumFieldError enumError )
      {
         IFieldChecker fChecker = new FieldCheckerClass();
         fChecker.Validate(shpFields,out enumError,out covFields);
         if(enumError != null)
         {
            System.Windows.Forms.MessageBox.Show("error on validate fields");
         }
      }
      #endregion

      #region gettersAndSetters
      public ITable StepTable
      {
         get { return mStepTable; }
         set { mStepTable = value; }
      }

      public IFeatureClass StepFeatureClass
      {
         get { return mStepFeatureClass; }
         set  { mStepFeatureClass = value; }
      }

      public IFeatureClass SuitableFeatureClass
      {
         get { return mSuitableFeatureClass; }
         set  { mSuitableFeatureClass = value; }
      }

       

      

      public string ErrMessage
      {
         get { return mErrMessage; }
         set  { mErrMessage = value; }
      }


      public Map MySocialMap
      {
         get { return mMySocialMap; }
         set  { mMySocialMap = value; }
      }

      #endregion
   }
}

