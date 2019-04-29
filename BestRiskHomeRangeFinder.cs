using System;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
namespace SEARCH
{
	/// <summary>
	/// 
	/// </summary>
	public class BestRiskHomeRangeFinder :HomeRangeFinder
	{
		#region Public Members (3) 

		#region Constructors (1) 

		public BestRiskHomeRangeFinder()
		{
		}

		#endregion Constructors 
		#region Methods (2) 

      public static BestRiskHomeRangeFinder getInstance()
      {
         if (uniqueInstance==null)
         {
            uniqueInstance = new BestRiskHomeRangeFinder();
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
               this.setRiskRank(inAnimal.HomeRangeCriteria.DistanceWeight);
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

      static BestRiskHomeRangeFinder uniqueInstance;

		#endregion Fields 
		#region Methods (1) 

      private ILog mLog = LogManager.GetLogger("hrTrigger");
      private ILog eLog = LogManager.GetLogger("Error");
        
      private void setRiskRank(double distanceFactor)
      {
         double d = 0;
         double adjustDistance = 0;
         mLog.Debug("inside setRiskRank with a distance factor of " + distanceFactor.ToString());

         try
         {
            foreach (EligibleHomeSite ehs in base.siteList)
            {
               if (ehs.SuitableSite)
               {
                  adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation, (1 / distanceFactor));
                  mLog.Debug(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
                  mLog.Debug("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString());
                  mLog.Debug("so adjusted distace value is " + adjustDistance.ToString());
                  mLog.Debug("the risk value is " + ehs.Risk.ToString());

                  ehs.Rank = (1 - ehs.Risk) / adjustDistance;
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

		#endregion Methods 

		#endregion Non-Public Members 
	}
}
