using System;
using ESRI.ArcGIS.Geometry;
namespace SEARCH
{
	/// <summary>
	/// Used to move an animal to its home range
	/// </summary>
	public class BeamMeHomeScottyMover : SEARCH.Mover
	{
		#region Public Members (4) 

		#region Constructors (1) 

		public BeamMeHomeScottyMover(): base()
		{
			rn = RandomNumbers.getInstance();
		}

		#endregion Constructors 
		#region Methods (3) 

//end of getTurnAngle
		public override double getStepLength()
		{
			return this.baseStepLength;
		}

		public override double getTurnAngle(double rho)
		{
			return rn.getWrappedCauchy(rho);
		}

		public override void move(ref double percentTimeStep, Animal inA)
		{
         //inA.Location = inA.HomeRangeCenter;

		}

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (1) 

		#region Fields (1) 

		RandomNumbers rn;

		#endregion Fields 

		#endregion Non-Public Members 

//end of getStepLength
	}
}
