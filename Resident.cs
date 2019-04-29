using System;
using DesignByContract;
using log4net;

namespace SEARCH
{
   /// <summary>
   /// Summary description for Resident.
   /// </summary>
   public class Resident : Animal
   {
		#region Constructors (1) 

       private ILog mLog = LogManager.GetLogger("animalLog");
       private ILog eLog = LogManager.GetLogger("Error");
       
       public Resident()
      {
         mMyAttributes = null;
         this.IdNum = -1;
      }

		#endregion Constructors 

		#region Fields (2) 

      private ResidentAttributes mMyAttributes;
      

		#endregion Fields 

		#region Properties (2) 

     

      public ResidentAttributes MyAttributes
      {
         get { return mMyAttributes; }
         set 
         {
            mMyAttributes = value;
            mLog.Debug("inside resident having attributes set"); }
      }

		#endregion Properties 

		#region Methods (4) 

		#region Public Methods (3) 

      public void breed(out int numMales, out int numFemales)
      {
         int numChildren = 0;
         numMales = 0;
         numFemales = 0;
         //TODO REMOVE VARIABLE USED ONLY FOR LOGGING
         double rollDice;
         try
         {
            mLog.Debug("inside breed for resident " + this.IdNum);
            //roll the dice to see if we breed or not.
            rollDice = rn.getUniformRandomNum();
            mLog.Debug("rolling the dice returned " + rollDice.ToString());
            mLog.Debug("percent breed is " + this.mMyAttributes.PercentBreed.ToString());
            if (rollDice <= this.mMyAttributes.PercentBreed)
            {
               //pretend we win now roll to see how many
               //TODO figure out how to get correct amount of kids
               mLog.Debug("my mean is " + this.mMyAttributes.NumChildernMean.ToString());
               mLog.Debug("my SD is " + this.MyAttributes.NumChildernSD.ToString());
               numChildren = rn.getNormalRandomInt(this.mMyAttributes.NumChildernMean, this.mMyAttributes.NumChildernSD);
               mLog.Debug("we are going to have a litter with " + numChildren.ToString());
               mLog.Debug("percent chance of having a female is " + this.mMyAttributes.PercentFemale.ToString());
               for (int i = 0; i < numChildren; i++)
               {
                  rollDice = rn.getUniformRandomNum();
                  mLog.Debug("rolling the dice returned " + rollDice.ToString());
                  if (rollDice <= this.mMyAttributes.PercentFemale)
                     numFemales++;
                  else
                     numMales++;
               }
               mLog.Debug("we have " + numMales + " males");
               mLog.Debug("we have " + numFemales + " females");
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         
         
      }

      public override void doTimeStep(HourlyModifier inHM, DailyModifier inDM, DateTime currTime, bool doTextOutput, ref string status)
      {
         Check.Require(mMyAttributes != null, "Resident Attributes have not been set");
         Check.Require(this.IdNum >= 0, "Resident ID was not set");
         mLog.Debug("inside time step for resident number " + this.IdNum.ToString());
         die(ref status, currTime);
      }

      public void winterKill(int currentTime)
      {
         try
         {
            mLog.Debug("inside winter kill for resident number " + this.IdNum.ToString());
            double rollOfTheDice = 0.0;
            rollOfTheDice = rn.getUniformRandomNum();
            mLog.Debug("inside winter kill with roll of " + rollOfTheDice.ToString());
            mLog.Debug("my chance of winter kill is " + this.MyAttributes.ResidentYearlyRisk.ToString());
            if (mMyAttributes.ResidentYearlyRisk > rollOfTheDice)
            {
               mLog.Debug("did not make it through the winter so setting mDead to true");
               this.mTextFileWriter.addLine("Resident died as winter kill in " + currentTime.ToString());
               this.mIsDead = true;
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

		#endregion Public Methods 
		#region Private Methods (1) 

      private void die(ref string status, DateTime currentTime)
      {
         try
         {
            double rollOfTheDice = 0.0;
            rollOfTheDice = rn.getUniformRandomNum();
            mLog.Debug("inside resident " + this.IdNum.ToString() + " time step die with roll of " + rollOfTheDice.ToString());
            mLog.Debug("my chance of dieing during a time is " + this.MyAttributes.ResidentTimeStepRisk.ToString());
            if (mMyAttributes.ResidentTimeStepRisk > rollOfTheDice)
            {
               status = "dead FROM ROLL OF DICE";
               mLog.Debug("george dies");
               this.mTextFileWriter.addLine("Resident died during timestep at " + currentTime.ToString());
               this.mIsDead = true;
            }
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
      }

		#endregion Private Methods 

		#endregion Methods 
   }
}
