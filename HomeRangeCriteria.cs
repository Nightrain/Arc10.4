using System;
using System.Runtime.Serialization;

namespace SEARCH
{
	/// <summary>
	/// contains the criteria for calculating the home range area
	/// </summary>
    [Serializable()]
	public class HomeRangeCriteria : IHomeRangeCriteria
	{
		#region Public Members (2) 

		#region Constructors (2) 

      public HomeRangeCriteria (double inArea,double inDM, double inDSD,double inDW)
      {
         mArea=inArea;
         mDistanceMean=inDM;
         mDistanceSD=inDSD;
         mDistanceWeight=inDW;
      }

		public HomeRangeCriteria()
      {
         mArea=0;
         mDistanceMean=0;
         mDistanceSD=0;
         mDistanceWeight=0;
      }

		#endregion Constructors 

		#endregion Public Members 

		#region Non-Public Members (4) 

		#region Fields (4) 

      private double mArea;
      private double mDistanceMean;
      private double mDistanceSD;
      private double mDistanceWeight;

		#endregion Fields 

		#endregion Non-Public Members 


      #region IHomeRangeCriteria Members
      public double Area
		{
			get { return mArea; }
			set  { mArea = value; }
		}

      public double DistanceMean
		{
			get { return mDistanceMean; }
			set  { mDistanceMean = value; }
		}

      public double DistanceSD
		{
			get { return mDistanceSD; }
			set  { mDistanceSD = value; }
		}

      public double DistanceWeight
		{
			get { return mDistanceWeight; }
			set  { mDistanceWeight = value; }
		}

     
      #endregion
   }
}
