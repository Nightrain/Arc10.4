using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace SEARCH
{
   struct MapValue
   {
      private double mCaptureFood;
      private IPoint mCurrLocation;
      private double mEnergyUsed;
      private int mFoodIndex;
      private double mFoodMeanSize;
      private double mFoodSD_Size;
      private double mHeading;
      private int mMoveIndex;
      private double mMoveSpeed;
      private double mMoveTurtosity;
      private double mPerceptionDist;
      private double mPerceptonModifier;
      private double mPredationRisk;
      private int mRiskIndex;
      private int mSocialIndex;
      public double CaptureFood
      {
         get { return mCaptureFood; }
         set { mCaptureFood = value; }
      }
      public IPoint CurrLocation
      {
         get { return mCurrLocation; }
         set { mCurrLocation = value; }
      }
      public double EnergyUsed
      {
         get { return mEnergyUsed; }
         set { mEnergyUsed = value; }
      }
       //index of the polygon on the food index
      public int FoodIndex
      {
         get { return mFoodIndex; }
         set { mFoodIndex = value; }
      }
      public double FoodMeanSize
      {
         get { return mFoodMeanSize; }
         set { mFoodMeanSize = value; }
      }
      public double FoodSD_Size
      {
         get { return mFoodSD_Size; }
         set { mFoodSD_Size = value; }
      }
      public double Heading
      {
         get { return mHeading; }
         set { mHeading = value; }
      }
    //index of the polygon on the move map
      public int MoveIndex
      {
         get { return mMoveIndex; }
         set { mMoveIndex = value; }
      }
      public double MoveSpeed
      {
         get { return mMoveSpeed; }
         set { mMoveSpeed = value; }
      }
      public double MoveTurtosity
      {
         get { return mMoveTurtosity; }
         set { mMoveTurtosity = value; }
      }
      public double PerceptionDist
      {
         get { return mPerceptionDist; }
         set { mPerceptionDist = value; }
      }
      public double PerceptonModifier
      {
         get { return mPerceptonModifier; }
         set { mPerceptonModifier = value; }
      }
      public double PredationRisk
      {
         get { return mPredationRisk; }
         set { mPredationRisk = value; }
      }
       //index of the polygon on the risk index
      public int RiskIndex
      {
         get { return mRiskIndex; }
         set { mRiskIndex = value; }
      }
    // index of the polygon on the social index
      public int SocialIndex
      {
         get { return mSocialIndex; }
         set { mSocialIndex = value; }
      }
   }
}
