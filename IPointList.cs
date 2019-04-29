using System;
using System.Collections;
using ESRI.ArcGIS.Geometry;

namespace SEARCH
{
	/// <summary>
	/// Summary description for IPointList.
	/// </summary>
	public class IPointList
	{
		#region Public Members (6) 

		#region Constructors (1) 

		public IPointList()
		{
			mPoints = new ArrayList();
		}

		#endregion Constructors 
		#region Properties (1) 

		public IPoint this[int i]
		{
			get {return (IPoint)mPoints[i];}
			set {mPoints.Add(value);}
		}

		#endregion Properties 
		#region Methods (4) 

		public void add(IPoint inPoint)
		{
			mPoints.Add(inPoint); 
		}

      public int Count()
      {
         int i = mPoints.Count;
         i--;
         return i;
      }

		public int getLastIndex()
		{
			return this.mPoints.Count - 1;
		}

		public IPoint getPointByIndex(int i)
		{
			return (IPoint)mPoints[i];
		}

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (1) 

		#region Fields (1) 

		private ArrayList mPoints;

		#endregion Fields 

		#endregion Non-Public Members 
	}
}
