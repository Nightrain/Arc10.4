 #define DEVELOP
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SEARCH
{

   [Serializable()]
   public  class Modifier
   {
		#region Public Members (1) 

		#region Constructors (1) 

      public Modifier()
      {
#if (DEVELOP)
         mCaptureFood = 1;
         mPredationRisk = 1;
         mMoveSpeed = 1;
         mMoveTurtosity = 1;
         mEnergyUsed = 1;
         mPerceptonModifier = 1;

#else
         mCaptureFood = 1;
         mPredationRisk = 1;
         mMoveSpeed = 1;
         mMoveTurtosity = 1;
         mEnergyUsed = 1;
         mPerceptonModifier = 1;

#endif

      }

		#endregion Constructors 

		#endregion Public Members 

		#region Non-Public Members (7) 

		#region Fields (7) 

      private double mCaptureFood;
      private double mEnergyUsed;
      private double mMoveSpeed;
      private double mMoveTurtosity;
      private string mName;
      private double mPerceptonModifier;
      private double mPredationRisk;

		#endregion Fields 

		#endregion Non-Public Members 


      #region getters and setters

      public string Name
		{
			get { return mName; }
			set { mName = value; }
		}



      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the probability of successfully foraging during one time step")]
      public double CaptureFood
      {
         get { return mCaptureFood; }
         set { mCaptureFood = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the probability of dying from predation or other risks during one time step")]
      public double PredationRisk
      {
         get { return mPredationRisk; }
         set { mPredationRisk = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the distance traveled in one time step")]
     
      public double MoveSpeed
      {
         get { return mMoveSpeed; }
         set { mMoveSpeed = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the degree of turning in one time step")]
      public double MoveTurtosity
      {
         get { return mMoveTurtosity; }
         set { mMoveTurtosity = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the amount of energy used in one time step")]
    
      public double EnergyUsed
      {
         get { return mEnergyUsed; }
         set { mEnergyUsed = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the perceptual window for one time step")]
    
      public double PerceptonModifier
      {
         get { return mPerceptonModifier; }
         set { mPerceptonModifier = value; }
      }
      #endregion
   }
}
