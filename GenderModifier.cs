using System.ComponentModel;
namespace SEARCH
{
   public class GenderModifiers
   {
		#region Public Members (6) 

		#region Properties (6) 

      [CategoryAttribute("Gender Modifiers"),
      DescriptionAttribute("Adjusts the amount of energy used in one time step")]
      public double EnergyUsedPerTimeStep
		{
			get { return mEnergyUsedPerTimeStep; }
			set { mEnergyUsedPerTimeStep = value; }
		}

      [CategoryAttribute("Gender Modifiers"),
      DescriptionAttribute("Adjusts the distance traveled in one time step")]
      public double MoveSpeed
		{
			get { return mMoveSpeed; }
			set { mMoveSpeed = value; }
		}

      [CategoryAttribute("Gender Modifiers"),
      DescriptionAttribute("Adjusts the degree of turning in one time step")]
      public double MoveTortusoity
		{
			get { return mMoveTortusoity; }
			set { mMoveTortusoity = value; }
		}

      [CategoryAttribute("Gender Modifiers"),
      DescriptionAttribute("Adjusts the probability of successfully foraging during one time step")]
      public double ProbCaptureFood
		{
			get { return mProbCaptureFood; }
			set { mProbCaptureFood = value; }
		}

      [CategoryAttribute("Gender Modifiers"),
      DescriptionAttribute("Adjusts the probability of dying from predation or other risks during one time step")]
      public double ProgKilledByPredator
		{
			get { return mProgKilledByPredator; }
			set { mProgKilledByPredator = value; }
		}

      [CategoryAttribute("Gender Modifiers"),
      DescriptionAttribute("Adjusts the perceptual window for one time step")]
      public double VisionRange
		{
			get { return mVisionRange; }
			set { mVisionRange = value; }
		}

		#endregion Properties 

		#endregion Public Members 

		#region Non-Public Members (6) 

		#region Fields (6) 

      private double mEnergyUsedPerTimeStep;
      private double mMoveSpeed;
      private double mMoveTortusoity;
       private double mProbCaptureFood;
      private double mProgKilledByPredator;
      private double mVisionRange;

		#endregion Fields 

		#endregion Non-Public Members 
   }
}
