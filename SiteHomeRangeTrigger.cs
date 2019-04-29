using System;
using System.IO;
using log4net;

namespace SEARCH
{
	/// <summary>
	/// Summary description for SiteHomeRangeTrigger.
	/// </summary>
	public sealed class SiteHomeRangeTrigger:IHomeRangeTrigger
	{
		#region Public Members (1) 

		#region Constructors (1) 

		public SiteHomeRangeTrigger(int numTimes,int inNumAnimals)
		{
         mLog.Debug("inside SiteHomeRangeTrigger constructor we need " + numTimes.ToString());
         this.mNumTimesNeeded = numTimes;
         goodSites = new int[inNumAnimals];
      }

		#endregion Constructors 

		#endregion Public Members 

		#region Non-Public Members (4) 

		#region Fields (3) 

      //this will hold the number of good sites each animal has visited
      //we are indexing by the animals id so goodSites[0] will hold the number of
      //good sites animal 0 has witnessed
      private int[] goodSites;
      private int mNumTimesNeeded;

		#endregion Fields 
		#region Methods (1) 

      private ILog mLog = LogManager.GetLogger("hrTrigger");
      private ILog eLog = LogManager.GetLogger("Error");


		#endregion Methods 

		#endregion Non-Public Members 


      #region HomeRangeTrigger Members

      public int numTimes
      {
         get
         {
            return mNumTimesNeeded;
         }
         set
         {
            mNumTimesNeeded = value;
         }
      }

      public void reset(int inNumAnimals)
      {
         
         goodSites = new int[inNumAnimals];
      }
      public bool timeToLookForHome(Animal inA)
      {
         bool time = false;
         mLog.Debug("inside timeToLookForHome for animal number " + inA.IdNum.ToString());
         mLog.Debug("checking to see if the site is good or not");
         bool isSiteGood = inA.isSiteGood();
         if (isSiteGood)
         {
            mLog.Debug("it was good so we have " + System.Convert.ToString(goodSites[inA.IdNum] + 1) + " sites");
            mLog.Debug("we need " + mNumTimesNeeded.ToString());
            if (++goodSites[inA.IdNum]>=mNumTimesNeeded)
            {
               time = true;
               mLog.Debug("so return true");
            }
         }
         return time;
      }

      #endregion
   }
}
