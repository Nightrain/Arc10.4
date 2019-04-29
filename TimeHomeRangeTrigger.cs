using System;
using System.IO;
using System.Collections;
using log4net;


namespace SEARCH
{
   
   public sealed class TimeHomeRangeTrigger:IHomeRangeTrigger
   {
		#region Public Members (1) 

		#region Constructors (1) 

       private ILog mLog = LogManager.GetLogger("hrTrigger");
       private ILog eLog = LogManager.GetLogger("Error");

      public TimeHomeRangeTrigger(int numTimes,int numAnimals)
      {
         this.numTimesCalled = new int[numAnimals];
         this.mNumTimesNeeded = numTimes;
      }

		#endregion Constructors 

		#endregion Public Members 

		#region Non-Public Members (4) 

		#region Fields (3) 

      // private int numTimesCalled;
      private int mNumTimesNeeded;
      private int[] numTimesCalled;

		#endregion Fields 
		#region Methods (1) 


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

      public void reset (int numAnimals)
      {
          this.numTimesCalled = new int[numAnimals];
      }
      public bool timeToLookForHome(Animal inA)
      {
         bool success = false;
         try
         {
            mLog.Debug("inside timeToLookForHome for animal number " + inA.IdNum.ToString());
            numTimesCalled[inA.IdNum]++;
            mLog.Debug("we now have " + numTimesCalled[inA.IdNum].ToString() + " active timesteps added");
            mLog.Debug("we need " + mNumTimesNeeded.ToString());
         
            if (this.mNumTimesNeeded <= this.numTimesCalled[inA.IdNum])
            {
               mLog.Debug("so it is time to look for a home");
               success = true;
            }
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return success;
      }

      #endregion
   }
}
