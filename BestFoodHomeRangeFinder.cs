using System;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using log4net;


namespace SEARCH
{
	/// <summary>
	/// Summary description for BestFoodHomeRangeFinder.
	/// </summary>
	public class BestFoodHomeRangeFinder:HomeRangeFinder     
	{
		#region Public Members (2) 

		#region Constructors (1) 

		public BestFoodHomeRangeFinder()
		{
      }

		#endregion Constructors 
		#region Methods (1) 

      public static BestFoodHomeRangeFinder getInstance()
      {
         if (uniqueInstance==null)
         {
            uniqueInstance = new BestFoodHomeRangeFinder();
         }
         return uniqueInstance;
      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (1) 

		#region Fields (1) 

      static BestFoodHomeRangeFinder uniqueInstance;

		#endregion Fields 

		#endregion Non-Public Members 


      #region IHomeRangeFinder Members

      private ILog mLog = LogManager.GetLogger("hrTrigger");
      private ILog eLog = LogManager.GetLogger("Error");

      private void setFoodRank(double distanceFactor)
      {
         double d = 0;
         double adjustDistance = 0;
         mLog.Debug("inside setFoodRank with a distance factor of " + distanceFactor.ToString());
         try
         {
            mLog.Debug("starting the loop through " + base.siteList.Count.ToString() + " sites");
            foreach (EligibleHomeSite ehs in base.siteList)
            {
               if (ehs.SuitableSite)
               {
                  adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation, (1 / distanceFactor));
                  mLog.Debug(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
                  mLog.Debug("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString());
                  mLog.Debug("so adjusted distace value is " + adjustDistance.ToString());
                  mLog.Debug("the food value is " + ehs.Food.ToString());
                  //BC Saturday, February 16, 2008 made chanage from (ehs.Rank = ehs.Food * adjustDistance)
                  ehs.Rank = ehs.Food / adjustDistance;
                  mLog.Debug("so its adjusted rank is " + ehs.Rank.ToString());
                  d += ehs.Rank;
               }
            }
            
            base.SetRanges(d);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public override bool setHomeRangeCenter(Animal inAnimal, string inFileName)
      {
         bool foundHomeRange = true;
         
         try
         {
            if (base.FindHomeRange(inAnimal, inFileName))
            {
               base.setDistance(inAnimal.Location);
               this.setFoodRank(inAnimal.HomeRangeCriteria.DistanceWeight);
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
            
      #endregion
   }
}
