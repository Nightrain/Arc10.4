using System;

namespace SEARCH
{
   /// <summary>
   /// Summary description for Duration.
   /// </summary>
   public struct Duration
   {
      private double mMeanAmt;
      private double mStandardDeviation;
      private string mType;
      public double MeanAmt
      {
         get { return mMeanAmt; }
         set 
         { 
            mMeanAmt = value;
            if(mMeanAmt > 24)
               mMeanAmt = 24;
         }
      }
      public double StandardDeviation
      {
         get { return mStandardDeviation; }
         set 
         { 
            mStandardDeviation = value;
            if (mStandardDeviation > 24)
               mStandardDeviation = 24;
         }
      }
      public string Type
      {
         get { return mType; }
         set { mType = value; }
      }
      public Duration(string inType,double inMean,double inSD)
      {
         mType = inType;
         
         mMeanAmt = inMean;
         if(mMeanAmt > 24)
            mMeanAmt = 24;
         mStandardDeviation = inSD;
         if (mStandardDeviation > 24)
            mStandardDeviation = 24;
      }
   }
}
