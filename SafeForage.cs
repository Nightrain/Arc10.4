namespace SEARCH
{
   public class SafeForageModifier : Modifier
   {
		#region Public Members (1) 

		#region Constructors (1) 

      public SafeForageModifier()
      {
         base.Name = "SafeForageModifier";
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

