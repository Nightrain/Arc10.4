namespace SEARCH
{
   /// <summary>
   /// Modifies the animals behaviors based on the fact it is a female. -The Singleton defines an Instance operation that lets clients access its unique instance. 
   /// -It may be responsible for creating its own unique instance.
   /// 
   /// 
   /// </summary>
   public sealed class FemaleModifier : Modifier
   {
		#region Public Members (1) 

		#region Methods (1) 

      /// <summary>
      /// -This operation implements the logic for returning the unique instance of the Singleton pattern.
      /// </summary>
      public static FemaleModifier GetUniqueInstance()
      {

         if(uniqueInstance == null) 
         { 
            uniqueInstance = new FemaleModifier();
         }
         return uniqueInstance;
      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (2) 

		#region Fields (1) 

      /// <summary>
      ///  -This attribute stores the instance of the Singleton class.
      /// </summary>
      private static FemaleModifier uniqueInstance;

		#endregion Fields 
		#region Constructors (1) 

      private FemaleModifier()
      {
         base.CaptureFood = 1;
         base.PredationRisk = 1;
         base.MoveSpeed = 1;
         base.MoveTurtosity = 1;
         base.EnergyUsed = 1;
         base.PerceptonModifier = 1;
      }

		#endregion Constructors 

		#endregion Non-Public Members 
   }
}
