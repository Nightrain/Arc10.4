using System;
using System.Runtime.Serialization;

namespace SEARCH
{
   /// <summary>
   /// Summary description for ResidentAttributes.
   /// </summary>
   [Serializable()]
   public class ResidentAttributes
   {
		#region Public Members (2) 

		#region Constructors (2) 

      public ResidentAttributes(double inTimeRisk,
         double inYearRisk,
         double inPercentBreed,
         double inPercentFemale,
         double inMeanLitterSize,
         double inSDLitterSize)
      {
         mResidentTimeStepRisk=inTimeRisk;
         mResidentYearlyRisk=inYearRisk;
         mPercentBreed=inPercentBreed;
         mPercentFemale=inPercentFemale;
         mNumChildernMean = inMeanLitterSize;
         mNumChildernSD=inSDLitterSize;
      }

      public ResidentAttributes()
      {
         mResidentTimeStepRisk=0;
         mResidentYearlyRisk=0;
         mPercentBreed=0;
         mPercentFemale=0;
      }

		#endregion Constructors 

		#endregion Public Members 

		#region Non-Public Members (7) 

		#region Fields (7) 

      private double mNumChildernMean;
      private double mNumChildernSD;
      private string mOrginalID;
      private double mPercentBreed;
      private double mPercentFemale;
      private double mResidentTimeStepRisk;
      private double mResidentYearlyRisk;

		#endregion Fields 

		#endregion Non-Public Members 


      #region gettersAndSetters

      public string OriginalID
      {
         get {return mOrginalID;}
         set {mOrginalID = value;}
      }
       public double PercentBreed
		{
			get { return mPercentBreed; }
			set  { mPercentBreed = value; }
		}


      public double PercentFemale
		{
			get { return mPercentFemale; }
			set  { mPercentFemale = value; }
		}



      public double NumChildernMean
      {
         get { return mNumChildernMean; }
         set { mNumChildernMean = value; }
      }

      public double NumChildernSD
      {
         get { return mNumChildernSD; }
         set { mNumChildernSD = value; }
      }

      public double ResidentTimeStepRisk
      {
         get { return mResidentTimeStepRisk; }
         set  { mResidentTimeStepRisk = value;} 
         
      }

      public double ResidentYearlyRisk
      {
         get { return mResidentYearlyRisk; }
         set  { mResidentYearlyRisk = value; }
      }
      #endregion
   }
}
