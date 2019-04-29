using System;
using log4net;

namespace SEARCH
{
	/// <summary>
	/// 
	/// </summary>
	public class SleepMover : SEARCH.Mover
	{
		#region Public Members (4) 

		#region Methods (4) 

      public static SleepMover getSleepMover()
      {
         if (s==null)
            s=new SleepMover();
         return s;

      }

//end of getTurnAngle
		//this mover doesn't use step length, so return 0 to make compiler happy
		public override double getStepLength()
		{
			return 0;
		}

		//this mover doesn't use turn angle, so return 0 to make compiler happy
		public override double getTurnAngle(double rho)
		{
			return 0;
		}

		//Sleeping, so don't move, but set percent time step to 1 to indicate we slept the whole time step
		public override void move(ref double percentTimeStep, Animal inA)
		{
            ILog mLog = LogManager.GetLogger("moverLog");
         mLog.Debug("inside move for sleep mover");
			percentTimeStep = 1;
		}

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (2) 

		#region Fields (1) 

      private static SleepMover s;

		#endregion Fields 
		#region Constructors (1) 

		private SleepMover()
		{
         
		}

		#endregion Constructors 

		#endregion Non-Public Members 

//end of getStepLength
	}
}
