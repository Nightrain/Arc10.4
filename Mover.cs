/******************************************************************************
 * Author:        Bob Cummings
 * Date:          Wednesday, July 02, 2008
 * Description:   Stopped setting the new move polygon index if they cross over
 *                into a new polygon.  
 *****************************************************************************/ 

using System;
using System.IO;
using ESRI.ArcGIS.Geometry;
using log4net;

namespace SEARCH
{

   /// <summary>
   /// Summary description for Mover.
   /// </summary>
   public abstract class Mover
   {


      #region privateMembers
      //private attributes
		
      private double myStepLength;
      private double myHeading;
      private double myTurnAngleVariability;
      private System.Collections.ArrayList myPath;
      private double myTerrainStepModifier;
      private double myTerrainAngleModifier;
      private double myTimeDayStepModifier;
      private double myTimeDayAngleModifier;
      private double myTimeYearStepModifier;
      private double myTimeYearAngleModifier;
      protected MapManager myMapManager;
      private ILog mLog = LogManager.GetLogger("moverLog");
      private ILog eLog = LogManager.GetLogger("Error");
      #endregion

      #region Contructors
      //constructors
      public Mover()
      {
         myMapManager = MapManager.GetUniqueInstance();

      }
      public Mover (System.Collections.ArrayList inPath)
      {
         myMapManager = MapManager.GetUniqueInstance();
         mLog.Debug("inside mover constructor");
         myPath = inPath;
      }
      public Mover(System.Collections.ArrayList inPath,double inBaseStepLength,double inHeading,double inTurnAngleVariability)
      {
         try
         {
            myMapManager = MapManager.GetUniqueInstance();
            mLog.Debug("has a path Count of " + inPath.Count.ToString());
            mLog.Debug("turn angle of " + inTurnAngleVariability.ToString());
            mLog.Debug("heading is " + inHeading.ToString());
            mLog.Debug("base step length is " + inBaseStepLength.ToString());
            this.Path= inPath;
            this.turnAngleVariability = inTurnAngleVariability;
            this.heading = inHeading;
            this.baseStepLength = inBaseStepLength;
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         
      }//end of constructor
      #endregion

      #region public Methods
      // methods
      
		
      public virtual void move(ref double percentTimeStep, Animal inA)		
      {
         IPoint there = null;
         IPoint here = null;
         crossOverInfo co;
         try
         {
            mLog.Debug(" inside move method with animal passed in " + inA.IdNum.ToString());
            //get specs  
            //get terrain modifiers from polygon
            double stepLen = inA.MoveSpeed * (1 - percentTimeStep);//determine step length
            double turt = inA.MoveTurtosity;//determine turtosity
            double angle = this.getTurnAngle(turt);//determine turn angle

            mLog.Debug("step length is " + stepLen.ToString());
            mLog.Debug("turt is " + turt.ToString());
            mLog.Debug("angle is " + angle.ToString());
            mLog.Debug("Heading is " + inA.Heading.ToString());
            mLog.Debug("start location is X = " + inA.Location.X.ToString() + " Y= " + inA.Location.Y.ToString());
            mLog.Debug("now calling step");
            here = inA.Location;//get current location
            if (percentTimeStep > 0 )
            {
               angle = 0; //keep original heading if partial time step
            }

            //move
            there = step(stepLen,angle,here,inA.Heading);//get new location
            mLog.Debug("after step heading is " + inA.Heading.ToString());
           
            inA.Heading = (inA.Heading + angle );//set new heading
            mLog.Debug("new heading is " + inA.Heading.ToString());
            mLog.Debug("the current location is x = " + inA.Location.X.ToString() + " y = " + inA.Location.Y.ToString() );
            mLog.Debug("the new  location is x = " + there.X.ToString() + " y = " + there.Y.ToString() );
            mLog.Debug("now make sure the end point is on the map");
            if(myMapManager.isPointOnMoveMap(there))
            {
               #region
               co = this.myMapManager.getCrossOverInfo(here,there);//get crossover info

               mLog.Debug("cross over info");
               mLog.Debug("did we cross over any polyons = " + co.ChangedPolys.ToString());
               mLog.Debug("CurrPolyValue = " + co.CurrPolyValue.ToString());
               mLog.Debug("NewPolyValue = " + co.NewPolyValue.ToString());
               mLog.Debug("Distance = " + co.Distance.ToString());
               mLog.Debug(" X = " + co.Point.X.ToString() + " Y = " + co.Point.Y.ToString());
               mLog.Debug("George is on the map = " + co.OnTheMap.ToString());

               //check if this pushed us into a new polygon
               //Wednesday, August 23, 2006 changed from length = length OR polyValue = polyValue
               //length = length AND polyValue = polyValue
               if (co.ChangedPolys == false)
                  //went the full distance stayed inside the same polygon the whole time
               {
                  inA.Location = there;
                  percentTimeStep = 1;
               }
               else //new poly, move to border,decide if you want to go or not, set percentTimeStep
               {
                  double likeHere = 0.0;
                  double likeThere = 0.0;
                  double distToBorder = 0.0;
                  double crossoverRatio = 0.0;
                  PointClass borderCrossing = null;
               

                  mLog.Debug("different polygon so now lets get to it"); 
                  mLog.Debug("calling mapmanager get cross over inFo");
               
               
                  //move to border
                  borderCrossing = co.Point;//get point on border
                  inA.Location = borderCrossing;//move to border
				
                  //decide if you want to go or not
                  likeHere = co.CurrPolyValue;//get the field that represents an animal's affinity for the current polygon
                  likeThere = co.NewPolyValue;//get the field that represents an animal's affinity for the new polygon
                  crossoverRatio = likeThere/likeHere; //calculate the ratio
                  //compare the ratio to a uniform random [0-1]
                  RandomNumbers rn = RandomNumbers.getInstance();
                  
                  if (crossoverRatio > rn.getUniformRandomNum())//crossover
                  {
                     mLog.Debug("I am going to cross over");
                     //change poly to new poly
                     //Wednesday, July 02, 2008
                     //inA.MoveIndex = this.myMapManager.getCurrMovePolygon(co.NewPolyPoint);
                     //if we are going to cross over then give him a nudge into the new polygon
                     inA.Location.PutCoords(co.NewPolyPoint.X,co.NewPolyPoint.Y);
                  }
                  else //go back
                  {  
                     mLog.Debug("I am not going to cross over");
                     inA.Heading = inA.Heading + Math.PI; //turn around 
                     //Now move back into the orginal polygon
                     Mover.stepForward(ref inA);
                  }
               
                  //set percentTimeStep
                  distToBorder = co.Distance;//get dist from start to border crossing
                  percentTimeStep +=(1-percentTimeStep)* (distToBorder/stepLen);//Update percentTimeStep
                  percentTimeStep = Math.Round(percentTimeStep, 2);//round to avoid taking steps < .01 time step
               }
            }
               #endregion
            else
            {
               //Wandered off the rest of the world
               inA.IsDead = true;
               inA.TextFileWriter.addLine("Animal has left the map");
               percentTimeStep = 1;
               return;
            }
            mLog.Debug("percent time step is " + percentTimeStep.ToString());
            mLog.Debug("now the heading value is " + inA.Heading.ToString());
            inA.updateMemory();
         }//end of try
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }//end of catch
      }//end of move
      public abstract double getTurnAngle(double turtosity);
      public abstract double getStepLength();
      #endregion

      #region private Methods
      /********************************************************************************
       *  Function name   : changeMovePolygon
       *  Description     : George's next move would put him in a new landscape area.
       *                    so we need to check the move value from one polygon to the 
       *                    other so we can decide if he should move or not?
       *  Return type     : bool 
       * *******************************************************************************/
      
      private bool changeMoveMapPolygon()
      {
         bool changePolygon = false;
         return changePolygon;
         //
      }
      private PointClass step(double stepLength, double turnAngle, IPoint start, double heading)
      {
         PointClass newPoint = new PointClass();
         mLog.Debug("inside step with the following values");
         mLog.Debug("stepLength is " + stepLength.ToString());
         mLog.Debug("turnAngle is " + turnAngle.ToString());
         mLog.Debug("heading is " + heading.ToString());
         mLog.Debug("start x is " + start.X.ToString() + " start y is " + start.Y.ToString());

         try
         {
            double newX = start.X+ stepLength * System.Math.Cos(heading + turnAngle);
            double newY = start.Y + stepLength * System.Math.Sin(heading + turnAngle);
            newPoint.X = newX;
            newPoint.Y = newY;
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         mLog.Debug("leaving step with new  x is " + newPoint.X.ToString() + " start y is " + newPoint.Y.ToString());
         return newPoint;
      }//end of step

      public PointClass stepBack(Animal inA)
         
      {
         PointClass newPoint = new PointClass();
         double stepLength = inA.MoveSpeed;
        
         IPoint start = inA.Location;
         double heading = inA.Heading;
         
         
         mLog.Debug("inside step with the following values");
         mLog.Debug("orginal stepLength is " + stepLength.ToString());
         //fw.writeLine("turnAngle is " + turnAngle.ToString());
         mLog.Debug("orgiginal heading is " + heading.ToString());
         heading += Math.PI;
         stepLength = stepLength * .001;
         mLog.Debug("I want to go backwards so my new heading is " + heading.ToString());
         mLog.Debug("but I am only going to go " + stepLength.ToString());
         mLog.Debug("start x is " + start.X.ToString() + " start y is " + start.Y.ToString());
         try
         {
            double newX = start.X+ stepLength * System.Math.Cos(heading);
            double newY = start.Y + stepLength * System.Math.Sin(heading);
            newPoint.X = newX;
            newPoint.Y = newY;
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         mLog.Debug("leaving step with new  x is " + newPoint.X.ToString() + " start y is " + newPoint.Y.ToString());
         return newPoint;
      }//end of stepBack

     
      #endregion

      #region staticMembers

      public static PointClass stepForward(IPoint startPoint, IPoint endPoint)
         
      {
          ILog mLog = LogManager.GetLogger("moverLog");
          ILog eLog = LogManager.GetLogger("Error");
         PointClass newPoint = new PointClass();
         ILine line = new LineClass();
         mLog.Debug("inside stepForward(IPoint startPoint, IPoint endPoint)");
         mLog.Debug("start point is " + PointToString(startPoint));
         mLog.Debug("end point is " + PointToString(endPoint));
         line.FromPoint = startPoint;
         line.ToPoint = endPoint;
         double angle = line.Angle;
         mLog.Debug("the angle we are going to move at is " + angle.ToString());
         
         double stepLength = .5;
         
         try  
         {
            double newX = endPoint.X+ stepLength * System.Math.Cos(angle);
            double newY = endPoint.Y + stepLength * System.Math.Sin(angle);
            newPoint.X = newX;
            newPoint.Y = newY;
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         mLog.Debug("leaving stepForward with a point value of " + PointToString(newPoint));
         return newPoint;
      }//end of stepForward

      public static void stepForward(ref Animal inA)
      {
          ILog eLog = LogManager.GetLogger("Error");
          IPoint startPoint = inA.Location;
         double stepLength = .05;
         try
         {
            inA.Location.X = startPoint.X  + stepLength * System.Math.Cos(inA.Heading);
            inA.Location.Y = startPoint.Y + stepLength * System.Math.Sin(inA.Heading);
        
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         
      }

      public static PointClass stepBack (IPoint startPoint, IPoint endPoint)
         
      {
          ILog eLog = LogManager.GetLogger("Error");
          PointClass newPoint = new PointClass();
         ILine line = new LineClass();

         line.FromPoint = startPoint;
         line.ToPoint = endPoint;
         double angle = line.Angle;
         double stepLength = -.05;
         
         try  
         {
            double newX = endPoint.X+ stepLength * System.Math.Cos(angle);
            double newY = endPoint.Y + stepLength * System.Math.Sin(angle);
            newPoint.X = newX;
            newPoint.Y = newY;
         }
         catch(System.Exception ex)
         {
            eLog.Debug(ex);
         }
         return newPoint;
      }//end of stepForward

      public static string PointToString(IPoint inPoint)
      {
         return " X = " + inPoint.X.ToString() + " Y = " + inPoint.Y.ToString();
      }
      #endregion

      #region getters and setters
      //public properties
      public System.Collections.ArrayList Path
      {
         get{return myPath;}
         set{myPath = value;}
      }//end of property Path

      public double baseStepLength
         /// <summary>
         /// This property represent the step length of any object that has a movement state.  It is used by the move() method to determine the distance traveled during a particular time step.
         /// </summary>
      {
         get	{return myStepLength;}
         set	{myStepLength = value;}
      }//end of stepLength property
      public Point currentLocation
      {
         get	{return (Point)myPath[myPath.Count - 1];}
         set	{myPath.Add(value);}
      }//end of stepLength property

      public double heading //in radians
         /// <summary>
         /// This property represent the heading of any object that has a movement state.  It is used by the move() method to determine the distance traveled during a particular time step.
         /// </summary>
      {
         get{return myHeading;}
         set{myHeading = value;}
      }//end of heading property
      public double turnAngleVariability
         /// <summary>
         /// This property is used by the random turn angle function to specify the degree of variation in the returned turn angle.
         /// </summary>
      {
         get
         {
            return myTurnAngleVariability;			
         }
         set
         {
            myTurnAngleVariability = value;			
         }
      }//end of turnAngleVariability property
	
      public double terrainStepModifier 
      {
         get{return myTerrainStepModifier;}
         set{myTerrainStepModifier = value;}
      }//end of TerrainStepModifier property
      public double timeDayAngleModifier 
      {
         get{return myTimeDayAngleModifier;}
         set{myTimeDayAngleModifier = value;}
      }//end of mytimeDayAngleModifier property
      public double terrainAngleModifier 
      {
         get{return myTerrainAngleModifier;}
         set{myTerrainAngleModifier = value;}
      }//end of terrainAngleModifier property
      public double timeDayStepModifier 
      {
         get{return myTimeDayStepModifier;}
         set{myTimeDayStepModifier = value;}
      }//end of timeDayStepModifier property
      public double timeYearStepModifier 
      {
         get{return myTimeYearStepModifier;}
         set{myTimeYearStepModifier = value;}
      }//end of timeYearStepModifier property
      public double timeYearAngleModifier 
      {
         get{return myTimeYearAngleModifier;}
         set{myTimeYearAngleModifier = value;}
      }//end of timeYearAngleModifier property
      #endregion
   }//end of class
}//end of namespace
