using ESRI.ArcGIS.Geometry;
using System;

namespace SEARCH
{
	/// <summary>
	/// Summary description for InitialAnimalAttributes.
	/// </summary>
	public class InitialAnimalAttributes
	{
		#region Public Members (2) 

		#region Constructors (1) 

		public InitialAnimalAttributes()
		{
			
         myLocation = new PointClass();
         myNumToMake = 0;
		}

		#endregion Constructors 
		#region Methods (1) 

      public void setPointValues(IPoint inP)
      {
         myLocation.X = inP.X;
         myLocation.Y = inP.Y;

      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (4) 

		#region Fields (4) 

      private IPoint myLocation;
      private int myNumToMake;
      private string myOrginalID;
      private char mySex;

		#endregion Fields 

		#endregion Non-Public Members 


      #region getters and setters
      public string OrginalID
      {
         get{return myOrginalID;}
         set{myOrginalID = value;}
      }
      public char Sex
		{
			get { return mySex; }
			set { mySex = value; }
		}

      public IPoint Location
		{
			get { return myLocation; }
			set { myLocation = value; }
		}

      public int NumToMake
		{
			get { return myNumToMake; }
			set { myNumToMake = value; }
		}
      #endregion
	}
}
