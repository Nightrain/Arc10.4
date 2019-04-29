using System;

using System.ComponentModel;
namespace SEARCH
{
	/// <summary>
	/// Summary description for HourlyModifier.
	/// </summary>
	public class HourlyModifier:Modifier
	{
		#region Public Members (2) 

		#region Constructors (1) 

		public HourlyModifier()
		{
			
		}

		#endregion Constructors 
		#region Properties (1) 

      [CategoryAttribute("Hourly Modifier Settings"), 
      DescriptionAttribute("Sets the time this set of modifers will start each day")]
      public int StartTime
		{
			get { return mStartTime; }
			set { mStartTime = value; }
               
		}

		#endregion Properties 

		#endregion Public Members 

		#region Non-Public Members (1) 

		#region Fields (1) 

      private int mStartTime;

		#endregion Fields 

		#endregion Non-Public Members 
	}
}
