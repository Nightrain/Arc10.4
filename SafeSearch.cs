namespace SEARCH
{
   public class SafeSearchModifier : Modifier
   {
		#region Public Members (1) 

		#region Constructors (1) 

      public SafeSearchModifier()
      {
         base.Name = "SafeSearchModifier";
         base.CaptureFood = 1.0;
         base.PredationRisk = 1;
         base.MoveSpeed = 1.0;
         base.MoveTurtosity = 1.0;
         base.EnergyUsed = 1.0;
         base.PerceptonModifier = 1.0;
      }

		#endregion Constructors 

		#endregion Public Members 
   }  
}

