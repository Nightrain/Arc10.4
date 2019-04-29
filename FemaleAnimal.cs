using log4net;

namespace SEARCH
{
   public class Female : Animal
   {
		#region Public Members (2) 

		#region Constructors (1) 

       private ILog mLog = LogManager.GetLogger("animalLog");
       
       public Female():base()
      {
         base.sex = "Female";
      }

		#endregion Constructors 
		#region Methods (1) 

      public override void dump()
      {
         mLog.Debug("");
         mLog.Debug("my sex is female");
         base.dump();

      }

		#endregion Methods 

		#endregion Public Members 
   }
   
}
