using System;
using System.ComponentModel;
namespace SEARCH
{
   /// <summary>
   /// A modifier that hangs out for a certain number of days
   /// </summary>
   public class DailyModifier : Modifier
   {
		#region Public Members (2) 

		#region Properties (1) 

      [CategoryAttribute("Daily Modifer Start Setting"), 
      DescriptionAttribute("Sets the day of the year this modifier should take effect")]
      public DateTime StartDate
		{
			get { return mStartDate; }
			set { mStartDate = value; }
		}

		#endregion Properties 
		#region Methods (1) 

      public void advanceOneYear()
      {  
         mStartDate = mStartDate.AddYears(1);

      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (1) 

		#region Fields (1) 

      private DateTime mStartDate;

		#endregion Fields 

		#endregion Non-Public Members 
   }
}
