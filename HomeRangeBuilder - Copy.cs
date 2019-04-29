using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using FileWriter;
using System.IO;

namespace SEARCH
{
   class HomeRangeBuilder
   {
      DataManipulator myDataManipulator;
      string clipPath;
      string homeRangeFileName;
      string dissolveHomeRangePolygon;
      string availablePolygonsFileName;
      FileWriter.FileWriter fw;
      public HomeRangeBuilder()
      {
         myDataManipulator = new DataManipulator();
         this.buildLogger();

      }

      public string BuildHomeRange(Animal inAnimal, string currSocialFileName)
      {
         string returnVal = "";
         double minArea = 0;
         bool success = false;
         double stretchFactor = 1.0;
         int index = 0;
         IPolygon tempPoly = null;
         fw.writeLine("inside BuildHomeRange for George number " + inAnimal.IdNum.ToString());
         fw.writeLine("the current social map is " + currSocialFileName);
         string path = System.IO.Path.GetDirectoryName(inAnimal.MapManager.getAnimalMapName(inAnimal.IdNum));
         while (minArea < inAnimal.HomeRangeArea)
         {
            fw.writeLine("inside loop for making the polygon");
            this.buildPathNames(path, index.ToString());
            fw.writeLine("going to call buildHomeRangePolygon with a stretch factor of " + stretchFactor.ToString());
            tempPoly = this.buildHomeRangePolygon(inAnimal, stretchFactor);
            fw.writeLine("now add it to a feature class");
            this.myDataManipulator.AddHomeRangePolyGon(homeRangeFileName, tempPoly);
            fw.writeLine("now clip it against the current social map");
            this.myDataManipulator.Clip(currSocialFileName, homeRangeFileName, clipPath);
            fw.writeLine("now get all the good polygons from the clip to meassure the area");
            IFeatureClass fc = this.myDataManipulator.GetSuitablePolygons(clipPath, inAnimal.Sex, availablePolygonsFileName);
            if (fc != null)
            {
               minArea = this.getArea(fc);
               fw.writeLine("ok we have " + minArea.ToString() + " and George needs " + inAnimal.HomeRangeArea.ToString());
               MapManager.RemoveFiles(homeRangeFileName);
               index++;
               if (minArea < inAnimal.HomeRangeArea)
               {
                  stretchFactor += .1;
                  MapManager.RemoveFiles(availablePolygonsFileName);
                  fc = null;
                  //MapManager.RemoveFiles(clipPath);
                  fw.writeLine("was not big enough so now the stretch factor is " + stretchFactor.ToString());
               }
               else
               {
               }
               fw.writeLine("leaving with a file name of " + path + availablePolygonsFileName);
               returnVal = path + availablePolygonsFileName;
            }
            else
            {
               returnVal = "No Home Found";
            }
         }

         System.Runtime.InteropServices.Marshal.ReleaseComObject(tempPoly);

         fw.writeLine("leaving with a file name of " + returnVal);
         return returnVal;
      }

      private void buildPathNames(string path,string multiplier)
      {
        
         homeRangeFileName = path + "\\HomeRangePolygon" + multiplier + ".shp";
         clipPath = path + "\\clipHomeRange" + multiplier + ".shp";
         availablePolygonsFileName = "\\availablePolygons" + multiplier + ".shp";
         dissolveHomeRangePolygon = path + "\\dissolveHomeRangePolygon" + multiplier + ".shp";


      }
      private IPolygon buildHomeRangePolygon(Animal inAnimal, double stretchFactor)
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
         IPolygon returnPoly;


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
            radius = Math.Sqrt(1000000 * inAnimal.HomeRangeArea / Math.PI) * rn.getPositiveNormalRandomNum(1.2, .1) * stretchFactor;
            tempPoint.X = inAnimal.HomeRangeCenter.X + radius * Math.Cos(angle);
            tempPoint.Y = inAnimal.HomeRangeCenter.Y + radius * Math.Sin(angle);
            boundaryPoints.AddPoint(tempPoint, ref missing, ref missing);
         }
         returnPoly = boundaryPoints as PolygonClass;
         returnPoly.Close();

         return returnPoly;
      }
      private double getArea(IFeatureClass inFC)
      {
         double area = 0;
         IArea areaGetter = null;
         IFeatureCursor currsor = null;
         IFeature currFeat = null;
         
         try
         {
            currsor = inFC.Search(null, true);
            while ((currFeat = currsor.NextFeature()) != null)
            {
               areaGetter = (IArea)currFeat.ShapeCopy;
               area += areaGetter.Area;
            }
            //area is in meters we are measuring in km so divide by 1000^2
            area = areaGetter.Area / (1000000);
            // fw.writeLine("total area is " + area.ToString() + " kilometers");
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
      
   }
}
