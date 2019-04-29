using System;
using ESRI.ArcGIS.Geometry;

namespace SEARCH
{
	/// <summary>
	/// Summary description for InitialResidentInfo.
	/// </summary>
	public class InitialResidentInfo
	{
		#region Public Members (3) 

		#region Constructors (1) 

		public InitialResidentInfo()
		{
			mLocation = null;
         mSex = null;
		}

		#endregion Constructors 
		#region Properties (2) 

     public IPoint Location
		{
			get { return mLocation; }
			set  { mLocation = value; }
		}

      public string Sex
		{
			get { return mSex; }
			set  { mSex = value; }
		}

		#endregion Properties 

		#endregion Public Members 

		#region Non-Public Members (2) 

		#region Fields (2) 

      private IPoint mLocation;
      private string mSex;

		#endregion Fields 

		#endregion Non-Public Members 
	}
}
