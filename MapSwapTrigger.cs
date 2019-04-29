/******************************************************************************
 * CHANGE LOG
 * DATE:          Sunday, September 09, 2007 12:06:34 PM
 * Author:        Bob Cummings
 * Description:   Added mOriginalStartDate variable.  Will be needed
 *                When the simulation rolls to another year for hourly and 
 *                seasonal type triggers.
 * 
 *                Added mMyTriggerType variable.  Will be needed to determine 
 *                how to set the next start time.  Problem was hourly time swap
 *                was working great for first day.  But did not advance to next
 *                day.
 *****************************************************************************/              
using System;

namespace SEARCH
{
	/// <summary>
	/// Summary description for MapSwapTrigger.
	/// </summary>
	public class MapSwapTrigger:ICloneable
	{
		#region Public Members (4) 

		#region Enums (1) 

      //For some reason the enum has to be made public to access
      //through a property.  But you can not see it from outside the 
      //cless?
      public enum mTriggerType
      {
         STATIC,
         YEARLY,
         DAILY,
         HOURLY
      }

		#endregion Enums 
		#region Constructors (1) 

public MapSwapTrigger()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion Constructors 
		#region Methods (2) 

      public void setTriggerType(int numYears,int numDays, int numHours)
      {
         if(numHours > 1)
            this.mMyTriggerType = mTriggerType.HOURLY;
         else if(numDays > 1)
            this.mMyTriggerType = mTriggerType.DAILY;
         else if (numYears > 1)
            this.mMyTriggerType = mTriggerType.YEARLY;
         else 
            this.mMyTriggerType = mTriggerType.STATIC;
      }

      public override string ToString()
      {
         return mStartDate.ToShortDateString() + " " + mStartDate.ToShortTimeString() + " " + mFilename + " " + this.mMyTriggerType.ToString();
      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (12) 

		#region Fields (12) 

		private int mDayGrp;
		private string mFilename;
      private mTriggerType mMyTriggerType;
      private System.DateTime mOriginalStartDate;
		private string mPath;
		private int mSeasonGrp;
		private System.DateTime mStartDate;
		private string mStartDay;
		private string mStartHour;
		private string mStartMinute;
		private string mStartMonth;
		private int mYearGrp;

		#endregion Fields 

		#endregion Non-Public Members 


#region getters and setters	

      public mTriggerType MyTriggerType
		{
			get { return mMyTriggerType; }
			set  { mMyTriggerType = value; }
		}

      

		public int YearGrp
		{
			get { return mYearGrp; }
			set { mYearGrp = value; }
		}

		public int SeasonGrp
		{
			get { return mSeasonGrp; }
			set { mSeasonGrp = value; }
		}

		public int DayGrp
		{
			get { return mDayGrp; }
			set { mDayGrp = value; }
		}

		public string StartMonth
		{
			get { return mStartMonth; }
			set { mStartMonth = value; }
		}

		public string StartDay
		{
			get { return mStartDay; }
			set { mStartDay = value; }
		}

		public string StartHour
		{
			get { return mStartHour; }
			set { mStartHour = value; }
		}

		public string StartMinute
		{
			get { return mStartMinute; }
			set { mStartMinute = value; }
		}

		public System.DateTime StartDate
		{
			get { return mStartDate; }
			set { mStartDate = value; }
		}
       public System.DateTime OriginalStartDate
		{
			get { return mOriginalStartDate; }
			set  { mOriginalStartDate = value; }
		}

        public  string Path
		{
            get { return mPath; }
            set { mPath = value; }
		}
		public string Filename
		{
			get { return mFilename; }
			set { mFilename = value; }
		}

		#endregion

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
