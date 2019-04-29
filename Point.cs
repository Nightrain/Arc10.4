using System;

namespace SEARCH
{
	public class Point
	{
		#region Public Members (5) 

		#region Constructors (1) 

		public Point (double x, double y)
		{
			xCoordinate = x;
			yCoordinate = y;
			radius = Math.Sqrt(x * x + y * y);
			angle = Math.Atan2(y,x);
		}

		#endregion Constructors 
		#region Methods (4) 

		public double getAngle(){return angle;}

		public double getRadius(){return radius;}

		public double getX(){return xCoordinate;}

		public double getY(){return yCoordinate;}

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (4) 

		#region Fields (4) 

		private double angle;
 //in radians
		private double radius;
		private double xCoordinate;
		private double yCoordinate;

		#endregion Fields 

		#endregion Non-Public Members 

//end of constructor
	}//end of class
}//end of namespace
