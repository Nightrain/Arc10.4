/******************************************************************************
 *    Changed By:    Bob Cummings
 *    Changed On:    Saturday, February 23, 2008
 *    Description:   Added method public DailyModifier getFirst()
 *                   needed to start the simulation.
 * ***************************************************************************/
using System;
namespace SEARCH
{
   
   public sealed class DailyModiferCollection:System.Collections.SortedList
   {
		#region Public Members (6) 

		#region Properties (1) 

     public DateTime NextStartDate
		{
			get { return mNextStartDate; }
			set { mNextStartDate = value; }
		}

		#endregion Properties 
		#region Methods (5) 

      public void advanceOneYear()
      {  
         DailyModifier dm;
         
         for(int i=0;i<this.Count;i++)
         {
            dm = (DailyModifier)this.GetByIndex(i);
            dm.advanceOneYear();
         }
      }

      public DailyModifier getFirst()
      {
         DailyModifier nextDM;
         if (currIndex == this.Count-1)
         {
            currIndex = 0;
         }
         else
         {
            currIndex++;
         }
         nextDM = (DailyModifier)this.GetByIndex(currIndex);
         this.mNextStartDate = nextDM.StartDate;
         return (DailyModifier)this.GetByIndex(0);
         
      }

      public DailyModifier getNext()
      {
         DailyModifier dm;
         DailyModifier nextDM;
         dm = (DailyModifier)this.GetByIndex(currIndex);
         if (currIndex == this.Count-1)
         {
            currIndex = 0;
         }
         else
         {
            currIndex++;
         }
         nextDM = (DailyModifier)this.GetByIndex(currIndex);
         this.mNextStartDate = nextDM.StartDate;
         return dm;
      }

      public static DailyModiferCollection GetUniqueInstance()
      {
         if(uniqueInstance == null) 
         { 
            uniqueInstance = new DailyModiferCollection();
         }
         return uniqueInstance;
      }

      public void reset()
      {
         currIndex = 0;
      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (4) 

		#region Fields (3) 

      int currIndex;
      private DateTime mNextStartDate;
      private static DailyModiferCollection uniqueInstance;

		#endregion Fields 
		#region Constructors (1) 

      private DailyModiferCollection()
      {
         currIndex = 0;
      }

		#endregion Constructors 

		#endregion Non-Public Members 
   }
}
