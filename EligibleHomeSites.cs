/******************************************************************************
 * Change Date:   02/10/2008
 * Change By:     Bob Cummings
 * Description:   Added logging capabilites
 * 
 *                Changed logic in setting rank.  Specs said to calculate 
 *                the Nth root of the distance value.  Old code was
 *                Math.Pow(ehs.DistanceFromCurrLocation,distanceFactor)
 *                Changed to some article on the web.
 *                Math.Pow(ehs.DistanceFromCurrLocation,(1/distanceFactor)
 * 
 *                Added call to sort the sites before choosing one.
 * ***************************************************************************
 * Change Date:   02/11/2008
 * Change By:     Bob Cummings
 * Description:   Added logic to reset the rank based on a range of values.
 * ****************************************************************************
 * Change Date:   Saturday, February 16, 2008
 * Change By:     Bob Cummings
 * Description:   Added logic calculating rank via distance factor.
 * ***************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;
using System.Linq;
using log4net;



namespace SEARCH
{
   /// <summary>
   /// Summary description for EligibleHomeSites.
   /// </summary>
   public class EligibleHomeSites
   {
		#region Constructors (1) 

      public EligibleHomeSites()
      {
         this.mySites = new List<EligibleHomeSite>();
      }

		#endregion Constructors 

		#region Fields (3) 

      private ILog mLog = LogManager.GetLogger("homeSite");
      private ILog eLog = LogManager.GetLogger("Error");
      List<EligibleHomeSite> mySites;
      int siteCount;

		#endregion Fields 

		#region Properties (2) 

      public List<EligibleHomeSite> MySites
      {
         get { return mySites; }
         set
         {
            if (mySites != null) mySites = null;
            mySites = value;
         }
      }

      public int SiteCount
      {
         get { return this.mySites.Count; }
         
      }

		#endregion Properties 

		#region Methods (18) 

		#region Public Methods (13) 

      public void addSite(EligibleHomeSite inSite)
      {
         mLog.Debug("adding an eligible home site");
         mLog.Debug("X = " + inSite.X.ToString() + " Y= " +inSite.Y.ToString());
         mLog.Debug("Food = " + inSite.Food.ToString() + " Risk = " + inSite.Risk.ToString());
         this.mySites.Add(inSite);
         mLog.Debug("now there are " + this.mySites.Count.ToString() + " sites to choose from");
      }

      public EligibleHomeSite getFirstSuitableSite()
      {
         EligibleHomeSite ehs = null;
         try
         {
            for (int i = 0; i < this.mySites.Count; i++)
            {
               ehs = this.mySites[i] as EligibleHomeSite;
               if (ehs.SuitableSite)
               {
                  break;
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
         return ehs;
      }

      public List<IPoint> getPoints()
      {
         
         List<IPoint> qualifyingSites = new List<IPoint>();
         foreach (EligibleHomeSite ehs in this.mySites)
         {
          
               IPoint p = new PointClass();
               p.X = ehs.X;
               p.Y = ehs.Y;
               qualifyingSites.Add(p);
           
         }
         return qualifyingSites;
      }

      public List<EligibleHomeSite> getQualifiedSites()
      {
         RandomNumbers rn = RandomNumbers.getInstance();
         double luckyNumber = rn.getUniformRandomNum();
         List<EligibleHomeSite> qualifyingSites = new List<EligibleHomeSite>();
         foreach (EligibleHomeSite ehs in this.mySites)
         {
            if (luckyNumber >= ehs.Rank)
            {
               qualifyingSites.Add(ehs);
            }
         }
         return qualifyingSites;

      }

      public EligibleHomeSite getSite (int index)
      {
         EligibleHomeSite ehs =null;
         try
         {
            if (this.mySites.Count > 0)
            {
               ehs = this.mySites[index] as EligibleHomeSite;
            }
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return ehs;
      }

      public void RemoveSite(IPoint inPoint)
      {
            mySites.RemoveAll(delegate(EligibleHomeSite ehs) { return ehs.X == inPoint.X && ehs.Y == inPoint.Y; });
      }

      public void setComboRank(double distanceFactor)
      {
         double d=0;
         double adjustDistance = 0;
         double maxFood = 0.0;
         double maxRisk = 0.0;
         double foodValue = 0.0;
         double riskValue = 0.0;

         this.sortByFood();
         maxFood = this.getSite(0).Food;
         this.sortByRisk();
         maxRisk = this.getSite(0).Risk;
         mLog.Debug("inside setComboRank with a distance factor of " + distanceFactor.ToString());
         mLog.Debug("max food was " + maxFood.ToString());
         mLog.Debug("max Risk was " + maxRisk.ToString());
         try
         {
            foreach (EligibleHomeSite ehs in this.mySites)
            {
               if(ehs.SuitableSite)
               {
                  adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation,(1/distanceFactor));
                  mLog.Debug(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
                  mLog.Debug("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString() );
                  mLog.Debug("so adjusted distace value is " + adjustDistance.ToString());
                  mLog.Debug("this sites food value is " + ehs.Food.ToString());
                  mLog.Debug("this sites risk value is " + ehs.Risk.ToString());
                  foodValue = ehs.Food/maxFood;
                  mLog.Debug("food value is " + foodValue.ToString());
                  riskValue = (1-ehs.Risk/maxRisk);
                  mLog.Debug("risk value is " + riskValue.ToString());
                  ehs.Rank = (foodValue + riskValue) / adjustDistance;
                  d+= ehs.Rank;
               }
            }
            setRanges(d);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public void setFoodRank(double distanceFactor)
      {
         double d=0;
         double adjustDistance = 0;
         mLog.Debug("inside setFoodRank with a distance factor of " + distanceFactor.ToString());
         try
         {
            mLog.Debug("starting the loop through " + this.mySites.Count.ToString() + " sites");
            foreach (EligibleHomeSite ehs in this.mySites)
            {
               if(ehs.SuitableSite)
               {
                  adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation,(1/distanceFactor));
                  mLog.Debug(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
                  mLog.Debug("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString() );
                  mLog.Debug("so adjusted distace value is " + adjustDistance.ToString());
                  mLog.Debug("the food value is " + ehs.Food.ToString());
                  //BC Saturday, February 16, 2008 made chanage from (ehs.Rank = ehs.Food * adjustDistance)
                  ehs.Rank = ehs.Food / adjustDistance;
                  mLog.Debug("so its adjusted rank is " + ehs.Rank.ToString());
                  d+= ehs.Rank;
               }
            }
            setRanges(d);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public void setRanges(double inDouble)
      {
         double d = 0;
         try
         {
            mLog.Debug("now setting the ranges based on the total rankings = " + inDouble.ToString());
            mLog.Debug("starting the loop");
            foreach(EligibleHomeSite ehs in this.mySites)
            {
               if(ehs.SuitableSite)
               {  
                  mLog.Debug(ehs.X.ToString() + ehs.Y.ToString() + " is eligble site raw rank is " + ehs.Rank.ToString());
                  ehs.Rank = ehs.Rank / inDouble;
                  mLog.Debug("after adjusting rank is " + ehs.Rank.ToString());
                  d+=ehs.Rank;
               }
            }
            
            
            mLog.Debug("total rank is " + d.ToString());
            this.sortByRank();
            this.resetRank();
            

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public void SetRanges(double inValue, List<EligibleHomeSite> inSites)
      {
         double d = 0;
         try
         {
            mLog.Debug("now setting the ranges based on the total rankings = " + inValue.ToString());
            mLog.Debug("starting the loop");
            foreach (EligibleHomeSite ehs in inSites)
            {
               
                  mLog.Debug(ehs.X.ToString() + ehs.Y.ToString() + " is eligble site raw rank is " + ehs.Rank.ToString());
                  ehs.Rank = ehs.Rank / inValue;
                  mLog.Debug("after adjusting rank is " + ehs.Rank.ToString());
                  d += ehs.Rank;
               
            }


            mLog.Debug("total rank is " + d.ToString());
            this.sortByRank();
            this.resetRank();


         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public void setRiskRank(double distanceFactor)
      {
         double d=0;
         double adjustDistance = 0;
         mLog.Debug("inside setRiskRank with a distance factor of " + distanceFactor.ToString());

         try
         {
            foreach (EligibleHomeSite ehs in this.mySites)
            {
               if(ehs.SuitableSite)
               {
                  adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation,(1/distanceFactor));
                  mLog.Debug(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
                  mLog.Debug("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString() );
                  mLog.Debug("so adjusted distace value is " + adjustDistance.ToString());
                  mLog.Debug("the risk value is " + ehs.Risk.ToString());

                  ehs.Rank = (1-ehs.Risk) / adjustDistance;
                  d+= ehs.Rank;
               }
            }
            setRanges(d);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      public void SortSites()
      {
         this.mySites.Sort();
      }

		#endregion Public Methods 
		#region Private Methods (4) 
    
      
      private void resetRank()
      {
         int i = 0;
         int j = 1;

        for(i=0,j=1; j< this.mySites.Count; i++,j++)
         {
            if(this.getSite(j).SuitableSite)
            {
               this.getSite(j).Rank = this.getSite(i).Rank + this.getSite(j).Rank;
            }
        }
         //sometimes it will only be .9999999999999987 and the random number could
         //conviebly be .9999999999989 so eliminate any chance.
        this.getSite(this.mySites.Count - 1).Rank = 1.0;

      }

      private void sortByFood()
      {
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Food;
         this.mySites.Sort();
      }

      private void sortByRank()
      {
          EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Rank;
          this.mySites.Sort();

      }

      private void sortByRisk()
      {
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Risk;
         this.mySites.Sort();
      }

		#endregion Private Methods 
		#region Protected Methods (1) 


		#endregion Protected Methods 

		#endregion Methods 

//end of buildLogger

      
   }
}
