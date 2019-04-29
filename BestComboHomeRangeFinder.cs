using System;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using System.Linq;
using log4net;
namespace SEARCH
{
   /// <summary>
   /// 
   /// </summary>
   public sealed class BestComboHomeRangeFinder : HomeRangeFinder
   {
		#region Public Members (3) 

		#region Constructors (1) 

      public BestComboHomeRangeFinder()
      {
         // 
         // TODO: Add constructor logic here
         //
      }

		#endregion Constructors 
		#region Methods (2) 

      public static BestComboHomeRangeFinder getInstance()
      {
         if (uniqueInstance == null)
         {
            uniqueInstance = new BestComboHomeRangeFinder();
         }
         return uniqueInstance;
      }

      public override bool setHomeRangeCenter(Animal inAnimal, string inFileName)
      {
         bool foundHomeRange = true;
         
         try
         {
            if (base.FindHomeRange(inAnimal, inFileName))
            {
               base.setDistance(inAnimal.Location);
               this.setComboRank(inAnimal.HomeRangeCriteria.DistanceWeight);
               inAnimal.HomeRangeCenter = base.getHomeRangeCenter() as PointClass;
            }
            else
            {
               foundHomeRange = false;
            }
         }
         catch (Exception ex)
         {
            eLog.Debug(ex);
            foundHomeRange = false;
         }
         return foundHomeRange;
      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (2) 

		#region Fields (1) 

      static BestComboHomeRangeFinder uniqueInstance;

		#endregion Fields 
		#region Methods (1) 

       private ILog eLog = LogManager.GetLogger("Error");
       private ILog mLog = LogManager.GetLogger("hrTrigger");
       
       private void setComboRank(double distWeight)
      {
         double d = 0;
         double adjustDistance = 0;
         double maxFood = 0.0;
         double maxRisk = 0.0;
         double foodValue = 0.0;
         double riskValue = 0.0;



         //set up to sort by food first
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Food;
         base.siteList.Sort();
         maxFood = base.siteList[0].Food;
         //get the risk data now
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Risk;
         base.siteList.Sort();
         maxRisk = base.siteList[0].Risk;
         mLog.Debug("inside setComboRank with a distance factor of " + distWeight.ToString());
         mLog.Debug("max food was " + maxFood.ToString());
         mLog.Debug("max Risk was " + maxRisk.ToString());
         try
         {
            for (int i = 0; i < base.siteList.Count; i++)
            {
               EligibleHomeSite ehs = base.siteList[i];
               adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation, (1 / distWeight));
               mLog.Debug(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
               mLog.Debug("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString());
               mLog.Debug("so adjusted distace value is " + adjustDistance.ToString());
               mLog.Debug("this sites food value is " + ehs.Food.ToString());
               mLog.Debug("this sites risk value is " + ehs.Risk.ToString());
               foodValue = ehs.Food / maxFood;
               mLog.Debug("food value is " + foodValue.ToString());
               riskValue = (1 - ehs.Risk / maxRisk);
               mLog.Debug("risk value is " + riskValue.ToString());
               ehs.Rank = (foodValue + riskValue) / adjustDistance;
               mLog.Debug("final rank is " + ehs.Rank.ToString());
               d += ehs.Rank;
            }
            EligibleHomeSites siteManager = new EligibleHomeSites();
            siteManager.MySites = base.siteList;
            siteManager.setRanges(d);
            
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

		#endregion Methods 

		#endregion Non-Public Members 
   }
}
