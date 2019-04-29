using log4net;

namespace SEARCH
{
   public class Male : Animal
   {
		#region Public Members (2) 

		#region Constructors (1) 

       private ILog mLog = LogManager.GetLogger("animalLog");

      public Male():base()
      {
         base.sex = "Male";
      }

		#endregion Constructors 
		#region Methods (1) 

      public override void dump()
      {
         mLog.Debug("");
         mLog.Debug("my sex is male");
         
         base.dump();

      }

		#endregion Methods 

		#endregion Public Members 
   }
}
