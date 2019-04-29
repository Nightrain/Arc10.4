using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections;
using System.Collections.Specialized;
namespace SEARCH
{
   public class MemoryMap : Map
   {
		#region Public Members (2) 

		#region Constructors (1) 

      public MemoryMap(IFeatureClass inSelf)
      {
         base.mySelf=inSelf;
         

      }

		#endregion Constructors 
		#region Methods (1) 

      /// <summary>
      /// When this map is first loaded we need to query the map and build the initial set of animals. 
      /// this will return an array of Initial Animal Attributes.
      /// </summary>
      /// 
      public new void  GetInitialAnimalAttributes(out InitialAnimalAttributes [] outAttributes)
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

         tmpFeatureClass = base.mySelf;
         tmpCur = tmpFeatureClass.Search(null,false);
         
         
         tmpFeature = tmpCur.NextFeature();
         if (tmpFeature.Shape.GeometryType.ToString() == "esriGeometryPoint")
         {
            while (tmpFeature != null)
            {
               //make an attribute from the first row in the data base
               tempIAA = new InitialAnimalAttributes();
               //have to have a row buffer to get the value out of the database
               rowBuff = (IRowBuffer)tmpFeature;

               tmpPoint = (IPoint)tmpFeature.ShapeCopy;
               fieldIndex = tmpFeature.Fields.FindField("MALES");
               if (fieldIndex >= 0)
               {
                  numToMake = System.Convert.ToInt32( rowBuff.get_Value(fieldIndex));
               }
               else
               {
                  numToMake = 0;
               }
               tempIAA.setPointValues(tmpPoint);
               tempIAA.Sex = 'M';
               tempIAA.NumToMake = numToMake;
               tmpArrayList.Add(tempIAA);

               tempIAA = new InitialAnimalAttributes();
               fieldIndex = tmpFeature.Fields.FindField("Fems");
               if (fieldIndex >= 0)
               {
                  numToMake = System.Convert.ToInt32(rowBuff.get_Value(fieldIndex));
               }
               else
               {
                  numToMake = 0;
               }
               tempIAA.setPointValues(tmpPoint);
               tempIAA.Sex = 'F';
               tempIAA.NumToMake = numToMake;
               tmpArrayList.Add(tempIAA);
               tmpFeature = tmpCur.NextFeature();

            }
         }
         outAttributes = new InitialAnimalAttributes[tmpArrayList.Count];
         tmpArrayList.CopyTo(outAttributes);

        
      }

		#endregion Methods 

		#endregion Public Members 
   }
}
