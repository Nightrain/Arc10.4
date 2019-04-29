using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using System;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Collections;
using log4net;
namespace SEARCH
{
   public class Map
   {
		#region Constructors (4) 

      public Map(string path,string fileName,IGeometryDef inGeoDef,IFields inFieldsCollection):this()
      {
        
         try
         {
            myPath = path;
            myFileName = fileName;
            IWorkspaceFactory shpWkspFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace shapeWksp = null;
            IPropertySet connectionProperty = new PropertySetClass();
            IGeometryDefEdit geoDef = new GeometryDefClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)inFieldsCollection;
            
            connectionProperty.SetProperty ("DATABASE",path);
            shapeWksp = (IFeatureWorkspace) shpWkspFactory.Open(connectionProperty,0);
            this.mySelf = shapeWksp.CreateFeatureClass(fileName,fieldsEdit,null,null,esriFeatureType.esriFTSimple,"Shape","");


         }
         catch(System.Exception ex)
         {
           
            if (ex.Source == "ESRI.ArcGIS.Geodatabase")
            {
               //mErrMessage = "That Directory is full with maps already";
               throw new System.Exception("That Directory is full with maps already");
            }
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
           
         }
        
      }

      public Map(IFeatureClass inSelf, string fullName):this()
      {
         mMySelf = inSelf;
         this.fullFileName = fullName;
      }

      public Map(IFeatureClass inSelf):this()
      {
         mMySelf = inSelf;
      }

      public Map()
      {
         outShapeFileName = new FeatureClassNameClass();
         wsName = new WorkspaceNameClass();
         myHash = new Hashtable();
         this.myFeatureLayer = new FeatureLayerClass();
         this.myMap = new MapClass();
         this.myFeature = null;
        
      }

		#endregion Constructors 

		#region Fields (17) 

      protected IDatasetName dsName;
      private string fullFileName;
      private DateTime mBeginTime;
      private string mChangeType;
      protected IFeatureClass mMySelf;
      protected string mTypeOfMap;
      private IFeature myFeature;
      protected IFeatureLayer myFeatureLayer;
      private IField myField;
      protected string myFileName;
      private Hashtable myHash;
      private IMap myMap;
      private object myObject;
      protected string myPath;
      protected IFeatureClassName outShapeFileName;
      protected IWorkspaceName wsName;
      private log4net.ILog mLog = LogManager.GetLogger("mapLog");
      private log4net.ILog eLog = LogManager.GetLogger("Error");

		#endregion Fields 

		#region Properties (6) 

      public DateTime BeginTime
      {
         get { return mBeginTime; }
         set { mBeginTime = value; }
      }

      public string ChangeType
      {
         get { return mChangeType; }
         set { mChangeType = value; }
      }

      public string FullFileName
      {
         get { return fullFileName; }
         set { this.fullFileName = value; }
      }

      public IFeatureClass mySelf
      {
         get { return mMySelf; }
         set { mMySelf = value; }
      }

      public string Path
      {
         get { return this.myPath; }
         set {this.myPath = value;}
      }

      public string TypeOfMap
      {
         get { return mTypeOfMap; }
         set  { mTypeOfMap = value; }
      }

		#endregion Properties 

		#region Methods (44) 

		#region Public Methods (29) 

      public void addDay()
      {
         mLog.Debug("inside add day for " + this.mySelf.AliasName + " adding a day to the start time");
         this.BeginTime = this.BeginTime.AddDays(1);
      }

      public void addField(string name,esriFieldType type)
      {
         IFieldEdit fieldEdit;
         fieldEdit = new FieldClass();
         fieldEdit.Name_2 = name;
         fieldEdit.Type_2 = type;//esriFieldType.esriFieldTypeSmallInteger;
         this.mySelf.AddField(fieldEdit);


      }

      public void addYear()
      {
         mLog.Debug("inside add year for " + this.mySelf.AliasName + " adding a year to the start time");
         mLog.Debug("before adding year = " + this.BeginTime.ToShortDateString());
         this.BeginTime = this.BeginTime.AddYears(1);
         mLog.Debug("after adding year = " + this.BeginTime.ToShortDateString());
      }

      public static IPolygon BuildHomeRangePolygon(Animal inAnimal, double stretchFactor)
      {
         const int numberOfPoints = 30;
         IPointCollection boundaryPoints = new PolygonClass();
         IPointCollection myPoints = new MultipointClass();
         double[] anglesList = new double[numberOfPoints];
         RandomNumbers rn = RandomNumbers.getInstance();
         IGeometry geom = null;
         double angle = 0;
         double radius = 0;
         IPoint tempPoint;
         object missing = Type.Missing;


         geom = (IGeometry)boundaryPoints;


         for (int i = 0; i < numberOfPoints; i++)
         {
            anglesList[i] = (rn.getUniformRandomNum() * Math.PI * 2);
         }
         System.Array.Sort(anglesList);
         //go backwards to get clockwise polygon for external ring
         for (int i = numberOfPoints - 1; i >= 0; i--)
         {
            tempPoint = new PointClass();
            angle = anglesList[i];
            //radius is slightly larger than needed for home range to compensate for not being a circle
            radius = Math.Sqrt(1000000 * inAnimal.HomeRangeCriteria.Area / Math.PI) * rn.getPositiveNormalRandomNum(1.2, .1) * stretchFactor;
            tempPoint.X = inAnimal.HomeRangeCenter.X + radius * Math.Cos(angle);
            tempPoint.Y = inAnimal.HomeRangeCenter.Y + radius * Math.Sin(angle);
            boundaryPoints.AddPoint(tempPoint, ref missing, ref missing);
         }

         return boundaryPoints as PolygonClass;
      }

      public void disovleFeatures(string fieldName)
      {
         ITable myTable = null;
         IDatasetName dsName = null;
         IBasicGeoprocessor bgp = new BasicGeoprocessorClass();
         IFeatureClassName fcn = new FeatureClassNameClass();
         try
         { 
            fcn.FeatureType=this.mySelf.FeatureType;
            fcn.ShapeType = this.mySelf.ShapeType;
            fcn.ShapeFieldName = this.mySelf.ShapeFieldName;

            dsName=(IDatasetName)fcn;
            dsName.Name = "testDisolove";
            dsName.WorkspaceName=this.getWorkspaceName();
           
            myTable = this.getTable();
            if (myTable.FindField(fieldName) >= 0)
            {
               ITable t = bgp.Dissolve(myTable,false,fieldName,"Dissolve.Shape,Minimum."+fieldName,dsName);
            }
            else
            {
#if debug
               System.Windows.Forms.MessageBox.Show(fieldName + " can not be found for dissolve");
#endif
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

      public void dissolvePolygons(string inFieldName)
      {
         try
         {
            IBasicGeoprocessor bgp = new BasicGeoprocessorClass();
            mLog.Debug("inside map mergepolygons clear out any exsiting layers");
            this.myMap.ClearLayers();
            mLog.Debug("make the feature class layer and add to map");
            this.myFeatureLayer = (IFeatureLayer)getLayer();
            ITable t = (ITable) this.myFeatureLayer;
            mLog.Debug("now work on the workspace thing");
            IWorkspaceName wsName = getWorkspaceName();
            IFeatureClassName outShapeFileName = new FeatureClassNameClass();
            outShapeFileName.FeatureType = mMySelf.FeatureType;
            outShapeFileName.ShapeType = mMySelf.ShapeType;
            outShapeFileName.ShapeFieldName = mMySelf.ShapeFieldName;
            IDatasetName dsName=(IDatasetName)outShapeFileName;
            dsName.Name = this.myFileName;
            dsName.WorkspaceName = wsName;

            t = bgp.Dissolve(t,false,inFieldName,"Dissolve.Shape",dsName);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public double getAllAvailableArea(int inAnimalID, string sex)
      {
         IArea a;
         //HACK remove after testing
         int numFeature = 0;
         double area = 0;double d = 0;
         IFeatureCursor search = null;
         mLog.Debug("inside getAllAvailableArea for animal id " + inAnimalID.ToString() + " who is " + sex);
         try
         {
            IFeature f = null;
            
            IQueryFilter qf = new QueryFilterClass();
            if (sex.ToUpper() == "MALE")
               qf.WhereClause = "OCCUP_MALE = '" + inAnimalID.ToString() + "'";
            else
               qf.WhereClause = "OCCUP_FEMA = '" + inAnimalID.ToString() + "'";
            
            mLog.Debug("so my cursor where clause is " + qf.WhereClause);
            search = this.mySelf.Search(qf,false);
            mLog.Debug("just filled the cursor");
            
            f = search.NextFeature();
            while (f != null)
            {
               
               a = (IArea)f.ShapeCopy;
               d = a.Area /(1000000);
               area += d;
               mLog.Debug("working on feature number " + numFeature.ToString());
               mLog.Debug("that feature has " + a.Area.ToString());
               mLog.Debug("so after area/(1000000) value is " + d.ToString());
               mLog.Debug("so current total area is " + area.ToString());
              
               f = search.NextFeature();
            }


         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(search);
         }
         return area;
      }

      /********************************************************************************
       *  Function name   : getAllValuesForSinglePolygon
       *  Description     : finds the specific polygon requested.  Then loops throug
       *                    all the fields in the database row for that polygon.
       *                    and adds them to the hash table with the name as the key
       *                    and the value as an objec.  The calling methos must know 
       *                    how to convert the object based on the names.
       *  Return type     : Hashtable 
       *  Argument        : int inPolyIndex
       * *******************************************************************************/
      public Hashtable getAllValuesForSinglePolygon(int inPolyIndex)
      {
         mLog.Debug("inside getAllValuesForSinglePolygon for poly index" + inPolyIndex.ToString());
         mLog.Debug("my map name is " + this.mySelf.AliasName);
         try
         {
            //clear out the hashtable
            this.myHash.Clear();
            //get the feature
            myFeature = this.mySelf.GetFeature(inPolyIndex);
            //loop through all the fields in the database
            for(int i=0;i<myFeature.Fields.FieldCount;i++)
            {
               myField = myFeature.Fields.get_Field(i);
               //do not want the two fields that esri adds
               if ((myField.Type != esriFieldType.esriFieldTypeGeometry) &&
                  (myField.Type != esriFieldType.esriFieldTypeOID))
               {
                  mLog.Debug("getting the value for " + myField.Name);
                  mLog.Debug("the value is " + myFeature.get_Value(i).ToString());
                  this.myHash.Add(myField.Name,myFeature.get_Value(i));
               }
            }
         }
        
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            
         }
         mLog.Debug("leaving getAllValuesForSinglePolygon");
         return this.myHash;
	
      }

      public double getArea(IPoint inPoint)
      {
         double d = -1;
         IFeature tempPoly = null;
         IArea tempArea;
         int polyIndex = -1;
         try
         {
            polyIndex = this.getCurrentPolygon(inPoint);
            tempPoly = this.mySelf.GetFeature(polyIndex);
            tempArea = tempPoly.Shape as IArea;
            d = tempArea.Area;  

         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            eLog.Debug(ex);
         }
         return d;
      }

      public double getAvailableArea(int inPolyIndex)
      {
         double area = 0;
         try
         {
            IFeature f = this.mySelf.GetFeature(inPolyIndex);
            IArea a = (IArea)f.ShapeCopy;
            area = a.Area/(1000^2);
         }
         catch(System.Exception ex)
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
         crossOverInfo coInfo = new crossOverInfo();
         IPolyline tempLine = new PolylineClass();
         IPointCollection pointCollection = null;
         IFeatureCursor fc;
         
         
         try
         {
            mLog.Debug("");
            mLog.Debug("inside map get cross over info making the line");
            tempLine.FromPoint = startPoint;
            tempLine.ToPoint = endPoint;
            mLog.Debug("the start point is " + startPoint.X.ToString() + " " + startPoint.Y.ToString());
            mLog.Debug("the end point is " + endPoint.X.ToString() + " " + endPoint.Y.ToString());
            mLog.Debug("the line is " + tempLine.Length.ToString() + " long");
            if(tempLine.Length > 0)
            {
               mLog.Debug("now get all the polygons that intersect with this line");
               fc = this.getCrossOverPolygons(tempLine);
               mLog.Debug("now get the points that are on the intersect line");
               pointCollection = this.getIntersectionPoints(fc,tempLine);
               if(pointCollection.PointCount>0)
               {
                  
                  mLog.Debug("now get the closest point on the line");
                  this.getClosestPoint(pointCollection,startPoint,ref coInfo);
                  mLog.Debug("now make sure we are not headed off the map");
                  if(this.isPointOnMap(coInfo.Point))
                  {
                     mLog.Debug("Point is on map so set the flag to true");
                     coInfo.OnTheMap = true;
                     mLog.Debug("now get the current polygons crossing value");
                     coInfo.CurrPolyValue = this.getCrossingValue(startPoint);
                     mLog.Debug("that value is " + coInfo.CurrPolyValue.ToString());
                     mLog.Debug("now get the new polygons crossing value");
                     coInfo.NewPolyValue = this.getCrossingValue(coInfo.NewPolyPoint);
                     mLog.Debug("that value is " + coInfo.CurrPolyValue.ToString());
                     coInfo.ChangedPolys = true;
                  }
                  else
                  {
                     coInfo.OnTheMap = false;
                  }
                  //fw.writeLine("cursor has this many " + fc.);
               }
               else
               {
                  mLog.Debug("there were not any intersection points");
                  mLog.Debug("the start point is " + startPoint.X.ToString() + " " + startPoint.Y.ToString());
                  mLog.Debug("the end point is " + endPoint.X.ToString() + " " + endPoint.Y.ToString());
                  mLog.Debug("the line is " + tempLine.Length.ToString() + " long");
                  

               }
            }
            else
            {
               mLog.Debug("the line had no length so there is nothing to send back");
               coInfo.OnTheMap = true;
               
            }
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return coInfo;
      }

      public crossOverInfo getCrossOverInfo(IPoint startPoint,IPoint endPoint,ref int curPolyIndex, ref int newPolyIndex)
      {
         crossOverInfo coInfo = new crossOverInfo();
         IPolyline tempLine = new PolylineClass();
         PointClass tempPoint = null;
         IPolygon tempPolygon = null;
         IRelationalOperator relOp = null;
         ITopologicalOperator topo = null;
         IMultipoint tempGeom = null;
         IPointCollection pointCollection = null;
         IFeature currFeaturePolygon = null;
         IFeature newFeaturePolygon = null;
         IRowBuffer rowBuf = null;
        
         IField field = null;

         try
         {
           
            mLog.Debug("inside map get cross over info making the line");
            mLog.Debug("the start point is " + startPoint.X.ToString() + " " + startPoint.Y.ToString());
            mLog.Debug("the end point is " + endPoint.X.ToString() + " " + endPoint.Y.ToString());
            tempLine.FromPoint = startPoint;
            tempLine.ToPoint = endPoint;
            mLog.Debug("the line is " + tempLine.Length.ToString() + " long");
            
            if(newPolyIndex <= this.mySelf.FeatureCount(null))
            {
               mLog.Debug("new poly index is " + newPolyIndex.ToString());
               //Tuesday, March 11, 2008 check to see if the point is off the map or on a boundary
               if(this.isPointOnMap(endPoint))
               {
                  
                  //if on the map just bump it up a bit for a new end point
                  mLog.Debug("endpoint is " + endPoint.X.ToString ()+" " + endPoint.Y.ToString());
                  mLog.Debug("so take step forward");
                  endPoint = Mover.stepForward(startPoint,endPoint);
                  mLog.Debug("now the end point is "+ endPoint.X.ToString() +" " + endPoint.Y.ToString());
                  mLog.Debug("but new poly index is " + newPolyIndex.ToString());
                  newPolyIndex = this.getCurrentPolygon(endPoint);
                  mLog.Debug("now new poly index is " + newPolyIndex.ToString());
                  //need to get new currPolyIndex
               }
               else
               {
                  //George has left the building
                  coInfo.OnTheMap = false;
               }
            }
               
            if(coInfo.OnTheMap)
            {
               //make the polyline between startPoint and endPoint and create the relational operator
               tempLine.FromPoint = startPoint;
               tempLine.ToPoint = endPoint;
               this.getCrossOverPolygons(tempLine);
               mLog.Debug("the distance between the two points is " + tempLine.Length.ToString() + " should be as long as a step length");
               relOp = (IRelationalOperator)tempLine;
           
               //get the current polygon from the map
               //get feature returns an IFeature object so we need to cast it
               //to an IGeometry type to use the IRelationalOperator in this
               //case we are going to cast it to a polygon
           
            
               currFeaturePolygon = this.mySelf.GetFeature(curPolyIndex);
               newFeaturePolygon = this.mySelf.GetFeature(newPolyIndex);
           
               tempPolygon = (IPolygon) newFeaturePolygon.ShapeCopy;
               mLog.Debug("the polygon we want to cross is " + newPolyIndex);
               mLog.Debug("now check to make sure we really cross for some reason");
               //get tempPoint where tempLine crosses the tempPoly
               //but sometimes the turosity of new landscape makes him want to turn around
               if (relOp.Crosses(tempPolygon))
               {
                  topo = (ITopologicalOperator)tempLine;
                  //this is stupid the intersect will return a multipoint even if it only has one point
                  tempGeom = (IMultipoint)topo.Intersect(tempPolygon,esriGeometryDimension.esriGeometry0Dimension);
                  pointCollection = (IPointCollection) tempGeom;
                  mLog.Debug("there are " + pointCollection.PointCount.ToString() + " points on the intersections");
                  mLog.Debug("the entry in the collection has x = " + pointCollection.get_Point(0).X.ToString() + " y = " + pointCollection.get_Point(0).Y .ToString());
                  tempPoint = new PointClass();
                  tempPoint.X = pointCollection.get_Point(0).X;
                  tempPoint.Y = pointCollection.get_Point(0).Y;
                  tempLine.ToPoint = tempPoint;
                  coInfo.Point = tempPoint;
               }
             
               else
               {
                  coInfo.Point = (PointClass)endPoint;
               }
         
               //make new line for getting the distace from where we start to the border
               //tempLine.ToPoint = tempPoint;
               mLog.Debug("the distance George is going to travel in this polygon is " + tempLine.Length.ToString());


               //get the distance from the start to the border
               coInfo.Distance = tempLine.Length;

               //current polygon value
               rowBuf = (IRowBuffer)currFeaturePolygon;

               for(int i=0;i<rowBuf.Fields.FieldCount;i++)
               {
                  field = rowBuf.Fields.get_Field(i);
                  if(field.Name == "CROSSING")
                  {
                     coInfo.CurrPolyValue = System.Convert.ToDouble(rowBuf.get_Value(i));
                     break;
                  }
               }

               //new polygon value
               rowBuf = (IRowBuffer)newFeaturePolygon;

               for(int i=0;i<rowBuf.Fields.FieldCount;i++)
               {
                  field = rowBuf.Fields.get_Field(i);
                  if(field.Name == "CROSSING")
                  {
                     coInfo.NewPolyValue = System.Convert.ToDouble(rowBuf.get_Value(i));
                     break;
                  }
               }
            }
            else
            {
               
            }
               
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         return coInfo;
      }

        /********************************************************************************
       *   Function name   : getCurrentPolygon
       *   Description     : takes the point in and tells us which polygon it is in.
       *   Return type     : int    the index for the ploygon or a negative value if
       *                      the point is off the map.
       *   Argument        : IPoint inPoint  the location we are wondering about
       * ********************************************************************************/
      public int getCurrentPolygon (IPoint inPoint)
      {
         int polyIndex = 0;
         int numPolys = 0;
         try
         {
            mLog.Debug("inside Map getCurrentPolygon for X = " + inPoint.X.ToString() + " Y " + inPoint.Y.ToString());
            //each individual polygon in the shapefile
            IPolygon searchPoly = null;
            IRelationalOperator relOp = null;
            IFeature tempPoly = null;
            //used to enumerate through the all the polygons
            IFeatureCursor polyCursor = null;

                       
            relOp = (IRelationalOperator)inPoint;
            //fill the cursor with all polygons
            polyCursor = this.mySelf.Search(null,false);
           
            numPolys = this.mySelf.FeatureCount(null);
            mLog.Debug("the cursor has " + polyCursor.Fields.FieldCount.ToString() + " number of fields");
            //get the feature class 
            tempPoly = polyCursor.NextFeature();
            mLog.Debug("have the first polygon");
            while(tempPoly != null)
            {
               // then cast it to a polygon
               searchPoly = tempPoly.ShapeCopy as IPolygon;
               if (searchPoly == null)
                  System.Windows.Forms.MessageBox.Show(tempPoly.FeatureType.ToString());

               if(relOp.Within(searchPoly))
               {
                  mLog.Debug("found it time to leave");
                  //found it so time to leave
                  break;
               }
               //keep going until we find what we are looking for
               tempPoly = polyCursor.NextFeature();
               polyIndex++;
            }
            //either we are off the map or the point is on the polygon boundary
            //so either way we do not have a value so set it to -1 so the calling 
            //function knows
            if(polyIndex >= numPolys)
            {
               
               polyIndex = -1;
            }
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
#if DEBUG
            System.Windows.Forms.MessageBox.Show("Error");
#endif
            
         }
         return polyIndex;

      }

      public void GetInitialAnimalAttributes(out InitialAnimalAttributes [] outAttributes)
      {
         IPoint tmpPoint = null;
         IFeature tmpFeature = null;
         IFeatureClass tmpFeatureClass = null;
         IFeatureCursor tmpCur = null;
         IRowBuffer rowBuff;
         
         ArrayList tmpArrayList = new ArrayList();
         int numToMake=0;
         int fieldIndex;

         InitialAnimalAttributes tempIAA;

         tmpFeatureClass = mySelf;

         tmpCur = tmpFeatureClass.Search(null,false);
         tmpFeature = tmpCur.NextFeature();

         if (tmpFeature.Shape.GeometryType.ToString() == "esriGeometryPoint")
         {
            while (tmpFeature != null)
            {
               //have to have a row buffer to get the value out of the database
               rowBuff = (IRowBuffer)tmpFeature;
               tmpPoint = (IPoint)tmpFeature.ShapeCopy;
               fieldIndex = tmpFeature.Fields.FindField("MALES");
               if (fieldIndex >= 0)
               { 
                  numToMake = System.Convert.ToInt32( rowBuff.get_Value(fieldIndex));
                  if(numToMake > 0)
                  {
                     //Author: Bob CummingsFriday, October 13, 2006 9:55:59 AM
                     //Moved logic inside if statement.  May have to move back out
                     tempIAA = new InitialAnimalAttributes();
                     tempIAA.setPointValues(tmpPoint);
                     tempIAA.Sex = 'M';
                     tempIAA.NumToMake = numToMake;
                     tempIAA.NumToMake=System.Convert.ToInt32( rowBuff.get_Value(fieldIndex));
                     tmpArrayList.Add(tempIAA);
                  }
               }

               
              
               fieldIndex = tmpFeature.Fields.FindField("FEMS");
               if (fieldIndex >= 0)
               { 
                  numToMake = System.Convert.ToInt32(rowBuff.get_Value(fieldIndex));
                  if(numToMake>0)
                  {
                     tempIAA = new InitialAnimalAttributes();
                     tempIAA.setPointValues(tmpPoint);
                     tempIAA.Sex = 'F';
                     tempIAA.NumToMake = numToMake;
                     tempIAA.NumToMake = System.Convert.ToInt32(rowBuff.get_Value(fieldIndex));
                     tmpArrayList.Add(tempIAA);
                  }
               }
               tmpFeature = tmpCur.NextFeature();

            }
         }
         outAttributes = new InitialAnimalAttributes[tmpArrayList.Count];
         tmpArrayList.CopyTo(outAttributes);

        
      }

      public void GetInitialResidentInformation(out InitialAnimalAttributes [] outInfo)
      { 
         outInfo = null;
         mLog.Debug("inside GetInitialResidentInformation");
         try
         {
           
            InitialAnimalAttributes tempIAA;
            ArrayList tmpArrayList = new ArrayList();
            ArrayList tmpIDList = new ArrayList();
            IQueryFilter qf = new QueryFilterClass();
            IFeatureCursor tmpCur;
            IFeature tmpFeature= null;
            IRowBuffer rowBuf= null;
            int fieldIndex=0;
            string fieldName = "OCCUP_MALE";
            string resID = null;
            
            // Get the resident males
            qf.WhereClause = fieldName + " <> 'none'";
            tmpCur = mySelf.Search(qf,false);
            tmpFeature = tmpCur.NextFeature();
            if (tmpFeature != null)
            {                             
               fieldIndex = tmpFeature.Fields.FindField(fieldName);
               while(tmpFeature != null)
               {
                  //get a searable row cursor
                  rowBuf=(IRowBuffer)tmpFeature;
                  resID = rowBuf.get_Value(fieldIndex).ToString();
                  if(! tmpIDList.Contains(resID))
                  {
                     tmpIDList.Add(resID);
                     tempIAA = new InitialAnimalAttributes();
                     tempIAA.Sex = 'M';
                     tempIAA.setPointValues(getCenterOfPolygon(tmpFeature));

                     tempIAA.OrginalID=resID;
                     tmpArrayList.Add(tempIAA);
                  }
                 
                  tmpFeature = tmpCur.NextFeature();
               }
            }

            //Get the resident females
            fieldName = "OCCUP_FEMA";
            qf.WhereClause = fieldName + " <> 'none'";
            tmpCur = mySelf.Search(qf,false);
            tmpFeature = tmpCur.NextFeature();
            if (tmpFeature != null)
            {                             
               fieldIndex = tmpFeature.Fields.FindField(fieldName);
               while(tmpFeature != null)
               {
                  //get a searable row cursor
                  rowBuf=(IRowBuffer)tmpFeature;
                  resID = rowBuf.get_Value(fieldIndex).ToString();
                  if(! tmpIDList.Contains(resID))
                  {
                     tmpIDList.Add(resID);
                     tempIAA = new InitialAnimalAttributes();
                     tempIAA.Sex = 'F';
                     tempIAA.setPointValues(getCenterOfPolygon(tmpFeature));
                     tempIAA.OrginalID=resID;
                     tmpArrayList.Add(tempIAA);
                  }
                 
                  tmpFeature = tmpCur.NextFeature();
               }
            }
            outInfo = new InitialAnimalAttributes[tmpArrayList.Count];
            tmpArrayList.CopyTo(outInfo);
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         
      }

      public IFeatureLayer getLayer()
      {
         myMap.ClearLayers();
         this.myFeatureLayer.FeatureClass = mMySelf;
         this.myFeatureLayer.Name = this.mySelf.AliasName;
         this.myMap.AddLayer(this.myFeatureLayer);
         ILayer l = myMap.get_Layer(0);
         return (IFeatureLayer)l;

      }

      public static IFeatureClass getMap(string path, string fileName)
      {
         IFeatureClass ifc = null;
         log4net.ILog eLog = LogManager.GetLogger("Error");
         string newPath = path + "\\" + fileName;
         try
         {
            IWorkspaceFactory wrkSpaceFactory = new ShapefileWorkspaceFactory();
            IFeatureWorkspace featureWorkspace=null;
            featureWorkspace = (IFeatureWorkspace)wrkSpaceFactory.OpenFromFile(newPath,0);
            ifc = featureWorkspace.OpenFeatureClass(fileName);
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         return ifc;
      }

      public object getNamedValueForSinglePolygon(int inPolyIndex,string inName)
      {
         this.myObject=null;
         try
         {
            int index;
            //get the polygon we want
            mLog.Debug("inside getNamedValueForSinglePolygon polyindex is " + inPolyIndex.ToString() + " value we want is " + inName);
            myFeature = this.mySelf.GetFeature(inPolyIndex);
            index=myFeature.Fields.FindField(inName);
            myObject = myFeature.get_Value(index);

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         

         return myObject;

      }

      public void getNumResidents(out int numMales, out int numFemales)
      {
         numFemales = 0;
         numMales = 0;
         mLog.Debug("inside getNumResidents for map " + this.mySelf.AliasName);
         try
         {
            
            numMales = this.getNumRes("male");
            numFemales = this.getNumRes("female");

         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }

      public ITable getTable()
      {
         ITable t;
         t = (ITable)getLayer();
         return t;
      }

      public IWorkspaceName getWorkspaceName()
      {
         IWorkspaceName wsName=null;
         try
         {
            wsName = new WorkspaceNameClass();
            wsName.PathName=this.myPath;
            wsName.WorkspaceFactoryProgID = "esriCore.ShapeFileWorkspaceFactory.1";
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         return wsName;
      }

      public bool hasFeatures()
      {
         return this.mySelf.FeatureCount(null) > 0;
      }

      public bool isPointOnMap(IPoint inPoint)
      {
         bool onMap=true;
         try
         {
            mLog.Debug("inside isPointOnMap with a value of X=" + inPoint.X.ToString() + " Y=" + inPoint.Y.ToString());
            //now we need to see if we are off the map or not.
            IEnvelope e = this.getLayer().AreaOfInterest;
            mLog.Debug("XMax = "+e.XMax.ToString());
            mLog.Debug("XMin = "+e.XMin.ToString());
            mLog.Debug("YMax = "+e.YMax.ToString());
            mLog.Debug("YMin = "+e.YMin.ToString());
            if (inPoint.X > e.XMax || inPoint.X < e.XMin || inPoint.Y > e.YMax || inPoint.Y < e.YMin)
               onMap=false;
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         mLog.Debug("leaving isPointOnMap with a value of " + onMap.ToString());
         return onMap;
      }

      public static IFeatureClass openFeatureClass(string path,string fileName)
      {
         IName name=null;
         log4net.ILog eLog = LogManager.GetLogger("Error");
         try
         {
            // Create the workspace name object
            IWorkspaceName workspaceName = new WorkspaceNameClass();
            workspaceName.PathName = path;
            workspaceName.WorkspaceFactoryProgID = "esriDataSourcesFile.ShapefileWorkspaceFactory";

            // Create the feature class name object
            IDatasetName datasetName = new FeatureClassNameClass();
            datasetName.Name          = fileName;
            datasetName.WorkspaceName = workspaceName;

            // Open the feature class
            name = (IName) datasetName;
		
            
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         // Return FeaureClass
         return (IFeatureClass) name.Open();

      }

      /********************************************************************************
       *   Function name   : pointInPolygon
       *   Description     : checks to see if the point is in the specific polygon
       *   Return type     : bool   true if it is and false otherwise
       *   Argument        : IPoint inPoint  the point we are concerned about
       *   Argument        : int inPolygonIndex index of the polygon to look in
       * ********************************************************************************/
      public bool pointInPolygon(IPoint inPoint, int inPolygonIndex)
      {
         bool success = false;
         IFeature tempPoly = null;
         IPolygon searchPoly = null;
         IRelationalOperator relOp = null;
         mLog.Debug("");
         mLog.Debug("inside pointInPolygon checking to see if the point is in the polygon");
         mLog.Debug("point is at x = " + inPoint.X.ToString() + " y = " + inPoint.Y.ToString() );
         mLog.Debug("the polygon checking is " + inPolygonIndex);
         mLog.Debug("the map is " + this.mySelf.AliasName);
         try
         {
            //if it was less then zero it can not be on the map
            if(inPolygonIndex>=0)
            {
               mLog.Debug("now check if there are at least " + inPolygonIndex.ToString() + " polygons in this map" );
               int numFeatures = mySelf.FeatureCount(null);
               mLog.Debug("there are " + numFeatures.ToString() );
               if (numFeatures >= inPolygonIndex)
               {
                  //cast the point object into the relational operator 
                  relOp = (IRelationalOperator)inPoint;
                  tempPoly = this.mySelf.GetFeature(inPolygonIndex);
                  //get feature returns an IFeature object so we need to cast it
                  //to an IGeometry type to use the IRelationalOperator in this
                  //case we are going to cast it to a polygon
                  searchPoly = (IPolygon)tempPoly.ShapeCopy;
                  if (searchPoly != null)
                  {
                     bool inside = relOp.Within(searchPoly);
                     bool touches = relOp.Touches(searchPoly);
                     //HACK
                     if (relOp.Within(searchPoly) || relOp.Touches(searchPoly))
                     {
                        success = true;
                     }
                  }
               }
            }
         }
         catch(System.Exception ex)
         {
           
            eLog.Debug(ex);
   
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         
         mLog.Debug("leaving with a value of " + success.ToString());
         mLog.Debug("");
         return success;
        

      }

      public void removeAllPolygons()
      {
         try
         {
            IFeatureCursor tmpCur;
            IFeature tmpFeature;
            tmpCur = this.mySelf.Update(null,false);
            tmpFeature = tmpCur.NextFeature();

            while(tmpFeature != null)
            {
               tmpCur.DeleteFeature();
               tmpFeature = tmpCur.NextFeature();
            }
            tmpCur.Flush();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(tmpCur);
            
         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            eLog.Debug(ex);
         }
         
      }

      public void removeAllPolygons(ref IFeatureClass inFeatureClass)
      {
         IFeatureCursor tmpCur;
         IFeature tmpFeature;
         tmpCur = inFeatureClass.Update(null,false);
         tmpFeature = tmpCur.NextFeature();

         while(tmpFeature != null)
         {
            tmpFeature.Delete();
            tmpFeature = tmpCur.NextFeature();
         }
         tmpCur.Flush();
         int j = inFeatureClass.FeatureCount(null);




      }

      public void removeCertainPolygons(string fieldName,string fieldValue)
      {
         IQueryFilter qf = new QueryFilterClass();
         qf.WhereClause = fieldName + "=" + fieldValue;
         IFeatureCursor tmpCur;
         IFeature tmpFeature;
         tmpCur = mySelf.Search(qf,false);
         tmpFeature = tmpCur.NextFeature();

         while(tmpFeature != null)
         {
            tmpFeature.Delete();
            tmpFeature = tmpCur.NextFeature();
         }
      }

      public void resetFields(string fieldName, string oldValue, string newValue)
      {  
         IQueryFilter qf = null;
         IFeatureCursor tmpCur = null;
         IFeature tmpFeature = null;
         int fieldIndex;
         try
         {
            mLog.Debug("inside reset fields we are going to change " + fieldName + "from " + oldValue + " to " + newValue);
            qf = new QueryFilterClass();
            qf.WhereClause = fieldName + "='" + oldValue +"'";
            tmpCur = mySelf.Update(qf,false);
            tmpFeature = tmpCur.NextFeature();
            
            while(tmpFeature != null)
            {
               fieldIndex = tmpFeature.Fields.FindField(fieldName);
               mLog.Debug("before update value was " + tmpFeature.get_Value(fieldIndex).ToString());
               tmpFeature.set_Value(fieldIndex,newValue);
               tmpCur.UpdateFeature(tmpFeature);
               mLog.Debug("after update value is " + tmpFeature.get_Value(fieldIndex).ToString());
               tmpFeature = tmpCur.NextFeature();
            }

         }
         catch(System.Exception ex)
         { 
            eLog.Debug(ex);
            eLog.Debug("my name is " + this.mySelf.AliasName);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
           
         }
         mLog.Debug("leaving resetFields");
      }

		#endregion Public Methods 
		#region Private Methods (10) 


      private IPoint getCenterOfPolygon(IFeature tempPoly)
      {
        
         IPolygon searchPoly = null;
         IPoint tempPoint = new PointClass();
         mLog.Debug("inside getCenterOfPolygon for map " + this.mySelf.AliasName);
         try
         {
            
            //get feature returns an IFeature object so we need to cast it
            //to an IGeometry type to use the IEnvelope interface in this
            //case we are going to cast it to a polygon
            searchPoly = (IPolygon)tempPoly.ShapeCopy;
            if (searchPoly != null)
            {
               tempPoint.X = (searchPoly.Envelope.XMax +searchPoly.Envelope.XMin) / 2;
               tempPoint.Y = (searchPoly.Envelope.YMax + searchPoly.Envelope.YMin) / 2;
            }

         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         return tempPoint;
      }

      private void getClosestPoint(IPointCollection inPoints, IPoint inPoint, ref crossOverInfo coInfo)
      {
         IPoint closestPoint=null;
         double oldDistance = 0;
        
         try
         {
            
            mLog.Debug("inside getClosestPoint setting the from and to points on the line");
            mLog.Debug("from point is " + this.PointToString(inPoint) );
            closestPoint = inPoints.get_Point(0);
            ILine tempLine = new LineClass();
            tempLine.FromPoint = inPoint;
            tempLine.ToPoint = closestPoint;
            oldDistance = tempLine.Length;
            
            for(int i=0;i<inPoints.PointCount;i++)
            {
               tempLine.ToPoint = inPoints.get_Point(i);
               mLog.Debug("to point is " + this.PointToString(tempLine.ToPoint));
               mLog.Debug("so the line is " + tempLine.Length.ToString());
               if(tempLine.Length < oldDistance)
               {
                  mLog.Debug("old distance was " + oldDistance.ToString());
                  closestPoint = inPoints.get_Point(i);
                  oldDistance = tempLine.Length;
               }
            }
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         
         coInfo.NewPolyPoint =  Mover.stepForward(inPoint,closestPoint);
         coInfo.Point.PutCoords(closestPoint.X,closestPoint.Y);
         coInfo.Distance = oldDistance;
         mLog.Debug("leaving get closest point which is " + this.PointToString(coInfo.Point));
         mLog.Debug("and the distance between the two points is " + coInfo.Distance.ToString());
         return;
      }

      private double getCrossingValue(IPoint inPoint)
      {
         mLog.Debug("inside getCrossingValue for point where x = " + inPoint.X.ToString() + " and Y = " + inPoint.Y.ToString());
         int polyIndex = this.getCurrentPolygon(inPoint);
         mLog.Debug("polyindex is " + polyIndex.ToString());
         return this.getPolyValue(polyIndex,"CROSSING");

      }

      private IFeatureCursor getCrossOverPolygons(IPolyline inLine)
      {
         ISpatialFilter myFilter=null;
         IFeatureCursor tempCur=null;
        
        
         try
         {
            myFilter = new SpatialFilterClass();
            myFilter.Geometry = inLine;
            myFilter.GeometryField = "SHAPE";
            myFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIndexIntersects;
            tempCur = this.mySelf.Search(myFilter,false);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return tempCur;
      }

      private IPointCollection getIntersectionPoints(IFeatureCursor inCurr,IPolyline inLine)
      {
         IFeature f = null;
         IMultipoint myTempPoints = null;
         IPointCollection myIntersectingPoints = null;
         ArrayList temp = new ArrayList();
         ITopologicalOperator topo = null;
         IRelationalOperator relOp = null;
         IPolygon tempPolygon = null;
         mLog.Debug("inside getIntersectionPoints");
        
         try
         {
            
            myTempPoints = new MultipointClass();
            myIntersectingPoints = new MultipointClass();
            relOp = (IRelationalOperator)inLine;
            topo = (ITopologicalOperator)inLine;
            f = inCurr.NextFeature();
            
            while(f!=null)
            {
               mLog.Debug("id is " + f.OID.ToString());
               tempPolygon  = (IPolygon) f.ShapeCopy;
               if (relOp.Crosses(tempPolygon))
               {
                  myTempPoints = (IMultipoint)topo.Intersect(tempPolygon,esriGeometryDimension.esriGeometry0Dimension);
                  myIntersectingPoints.AddPointCollection((IPointCollection)myTempPoints);
                  
                   mLog.Debug("there are now " + myIntersectingPoints.PointCount.ToString() + " points to check");
               }
               f = inCurr.NextFeature();
            }

         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            
         }
     
         return myIntersectingPoints;
      }

      private int getNumRes(string sex)
      {
         IQueryFilter qf = new QueryFilterClass();
         IFeatureCursor tmpCur;
         IFeature tmpFeature;
         IRowBuffer rowBuf;
         System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
         string s;
         int j;
         string fieldName = "OCCUP_MALE";
         mLog.Debug("inside getNumRes for " + sex);
         if(sex == "female")
            fieldName = "OCCUP_FEMA";

         try
         {
            qf.WhereClause = fieldName + " <> 'none'";
            
            tmpCur = mySelf.Search(qf,false);
            tmpFeature = tmpCur.NextFeature();

            while(tmpFeature != null)
            {
               rowBuf=(IRowBuffer)tmpFeature;
               j = rowBuf.Fields.FindField(fieldName);
               s = rowBuf.get_Value(j).ToString();
               if(! sc.Contains(s))
                  sc.Add(s);
               tmpFeature = tmpCur.NextFeature();
            }


         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         return sc.Count;
      }

      private double getPolyValue(int polyIndex, string fieldName)
      {
         double returnValue = 0.0;
         IFeature currPoly = null;
         IField field = null;
         try
         {
            //get the polygon from the map and a database reader for the polygon values
            currPoly = this.mySelf.GetFeature(polyIndex);
            IRowBuffer rowBuf =(IRowBuffer)currPoly;

            //loop through and find the matching name
            for(int i=0;i<rowBuf.Fields.FieldCount;i++)
            {
               field = rowBuf.Fields.get_Field(i);
               if (field.Name == fieldName)
               {
                  returnValue = System.Convert.ToDouble(rowBuf.get_Value(i));
               }
            }
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
            eLog.Debug("my name is " + this.mySelf.AliasName);
            eLog.Debug("looking for polyindex " + polyIndex.ToString());
         }
         return returnValue;
      }

      private void getTable(ref ITable inOutTable)
      {
         IMap myMap = new MapClass();
         IFeatureLayer tempFLayer = new FeatureLayerClass();
         ILayer tempLayer;

         tempFLayer.FeatureClass = mySelf;
         tempFLayer.Name = mySelf.AliasName;
         myMap.AddLayer(tempFLayer);
         tempLayer = myMap.get_Layer(0);
         inOutTable = (ITable)tempLayer;
      }

      private string PointToString(IPoint p)
      {
         return " X = " + p.X.ToString() + " Y = " + p.Y.ToString();
      }

		#endregion Private Methods 
		#region Protected Methods (5) 

      protected void buildDataSetName(string inName)
      {
         try
         {
            dsName=(IDatasetName)outShapeFileName;
            dsName.Name = inName;
            dsName.WorkspaceName = wsName;

         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
            
#endif
         }
         
      }

      protected void buildOutShapeFileName()
      {
         try
         {
            outShapeFileName.FeatureType = esriFeatureType.esriFTSimple;
            outShapeFileName.ShapeType = esriGeometryType.esriGeometryPolygon;
            outShapeFileName.ShapeFieldName = "Shape";

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }

      }

      protected void buildWorkSpaceName()
      {
         wsName.PathName = myPath;
         wsName.WorkspaceFactoryProgID = "esriCore.ShapeFileWorkspaceFactory.1";

      }

      protected void getTable(out ITable inOutTable, IFeatureClass inFeatureClass)
      {
         IMap myMap = new MapClass();
         IFeatureLayer tempFLayer = new FeatureLayerClass();
         ILayer tempLayer;

         tempFLayer.FeatureClass = inFeatureClass;
         tempFLayer.Name = inFeatureClass.AliasName;
         myMap.AddLayer(tempFLayer);
         tempLayer = myMap.get_Layer(0);
         inOutTable = (ITable)tempLayer;
      }

      protected void removePolygon(IFeature inFeature)
      {
         IFeatureCursor tmpCur;
         IFeature tmpFeature;
         try
         {
            mLog.Debug("inside remove polgon on map class");
            tmpCur = mySelf.Update(null,false);
            tmpFeature = tmpCur.NextFeature();
            mLog.Debug("going to enter loop to look for feature");
            while(tmpFeature != null)
            {
               if (tmpFeature.OID == inFeature.OID)
               {
                  mLog.Debug("found the match so delete the feature");
                  tmpCur.DeleteFeature();
                  break;
               }
               tmpFeature = tmpCur.NextFeature();

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

		#endregion Protected Methods 

		#endregion Methods 
   }
}
