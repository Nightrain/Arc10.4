namespace SEARCH
{
   public interface  IHomeRangeTrigger
   {
      int numTimes{get;set;}
      
      bool timeToLookForHome(Animal inA);
      void reset(int num);
   }
}