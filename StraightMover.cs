using System;

namespace SEARCH
{
	/// <summary>
	/// Summary description for StraightMover.
	/// </summary>
	public class StraightMover:Mover
	{
		#region Public Members (3) 

		#region Methods (3) 

//end of getTurnAngle
			public override double getStepLength()
		{
			return this.baseStepLength;
		}

		public static StraightMover getStraightMover()
		{
			if (r==null)
			r = new StraightMover();
			return r;
		}

			public override double getTurnAngle(double rho)
		{
			return 0;
		}

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (3) 

		#region Fields (1) 

		private static StraightMover r;

		#endregion Fields 
		#region Constructors (2) 

		private StraightMover(System.Collections.ArrayList inPath,
									double inBaseStepLength,
									double inHeading,
									double inTurnAngleVariability):base (inPath,inBaseStepLength,inHeading,inTurnAngleVariability){}

		private StraightMover(): base()
		{
		}

		#endregion Constructors 

		#endregion Non-Public Members 

//end of getStepLength
	}
}
