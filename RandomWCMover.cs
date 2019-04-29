using System;
using ESRI.ArcGIS.Geometry;



namespace SEARCH

{
   /// <summary>
   /// Summary description for RandomWCMover.
   /// </summary>
   public class RandomWCMover:Mover
   {
		#region Public Members (3) 

		#region Methods (3) 

      public static RandomWCMover getRandomWCMover()
      {
         if (r==null)
            r = new RandomWCMover();
         return r;


      }

//end of getTurnAngle
      public override double getStepLength()
      {
         return this.baseStepLength;
      }

      public override double getTurnAngle(double rho)
      {
         return rn.getWrappedCauchy(rho);
      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (4) 

		#region Fields (2) 

      private static RandomWCMover r;
      RandomNumbers rn;

		#endregion Fields 
		#region Constructors (2) 

      private RandomWCMover(System.Collections.ArrayList inPath,
         double inBaseStepLength,
         double inHeading,
         double inTurnAngleVariability)
         :base (inPath,inBaseStepLength,inHeading,inTurnAngleVariability){}

      private RandomWCMover(): base()
      {
         rn = RandomNumbers.getInstance();
      }

		#endregion Constructors 

		#endregion Non-Public Members 

//end of getStepLength
   }//end of class
}//end of namespace
