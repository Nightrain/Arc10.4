namespace SEARCH
{
   /// <summary>
   /// This one will modify the animals behavior under the assumption that the animal feels threatned but very hungry. -The Singleton defines an Instance operation that lets clients access its unique instance. 
   /// -It may be responsible for creating its own unique instance.
   /// 
   ///  -The Context defines the interface of interest to clients.
   /// -It maintains an instance of a ConcreteState subclass that defines the current state.
   /// 
   /// </summary>
   public  class BehaviourModifier : GenderModifiers
   {
		#region Public Members (1) 

		#region Methods (1) 

      /// <summary>
      ///  -This operation sets  the attribute "state" in the Context class.
      /// </summary>
      /// <summary>
      /// -This operation implements the logic for returning the unique instance of the Singleton pattern.
      /// </summary>
      public static BehaviourModifier GetUniqueInstance()
      {

         if(uniqueInstance == null) 
         { 
            uniqueInstance = new BehaviourModifier();
         }
         return uniqueInstance;
      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (1) 

		#region Fields (1) 

      /// <summary>
      ///  -This attribute stores the instance of the Singleton class.
      /// </summary>
      private static BehaviourModifier uniqueInstance;

		#endregion Fields 

		#endregion Non-Public Members 
   }
}
