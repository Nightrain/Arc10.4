using System;

namespace SEARCH
{
   /// <summary>
   /// Summary description for IHomeRangeCriteria.
   /// </summary>
   public interface IHomeRangeCriteria
   {
		#region Data Members (4) 

      double Area
      {
         get;
         set;
      }

      double DistanceMean
      {
         get;
         set;
      }

      double DistanceSD
      {
         get;
         set;
      }

      double DistanceWeight
      {
         get;
         set;
      }

		#endregion Data Members 
   }
}
