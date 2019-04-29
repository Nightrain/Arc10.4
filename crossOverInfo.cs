
using System;
using ESRI.ArcGIS.Geometry;

namespace SEARCH
{
	
	public class crossOverInfo
	{
		#region Public Members (8) 

		#region Constructors (1) 

		public crossOverInfo()
		{
		   this.mDistance = 0.0;
	      this.mPoint = new PointClass();
         this.mPoint.X = 0.0;
         this.mPoint.Y = 0.0;
         this.CurrPolyValue = 0.0;
         this.NewPolyValue = 0.0;
         
         onTheMap = true;
         changedPolys = false;
        
		}

		#endregion Constructors 
		#region Properties (7) 

      public bool ChangedPolys
      {
         get {return changedPolys;}
         set {changedPolys = value;}
      }

      public double CurrPolyValue
		{
			get { return mCurrPolyValue; }
			set { mCurrPolyValue = value; }
		}

      public double Distance
		{
			get { return mDistance; }
			set { mDistance = value; }
		}

      public PointClass NewPolyPoint
      {
         get { return mNewPolyPoint; }
         set { mNewPolyPoint = value; }
      }

      public double NewPolyValue
		{
			get { return mNewPolyValue; }
			set { mNewPolyValue = value; }
		}

      public bool OnTheMap
      {
         get { return onTheMap; }
         set { onTheMap = value; }
      }

      public PointClass Point
		{
			get { return mPoint; }
			set { mPoint = value; }
		}

		#endregion Properties 

		#endregion Public Members 

		#region Non-Public Members (7) 

		#region Fields (7) 

      private bool changedPolys;
      private double mCurrPolyValue;
      private double mDistance;
      private PointClass mNewPolyPoint;
      private double mNewPolyValue;
      private PointClass mPoint;
      private bool onTheMap;

		#endregion Fields 

		#endregion Non-Public Members 
	}
}
