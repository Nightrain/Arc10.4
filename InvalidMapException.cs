using System;

namespace SEARCH
{
	/// <summary>
	/// thrown when a map doesn't validate correctly
	/// </summary>
	internal sealed class InvalidMapException : System.ApplicationException
	{
		#region Non-Public Members (1) 

		#region Constructors (1) 

		internal InvalidMapException(string msg):base(msg)
		{
		}

		#endregion Constructors 

		#endregion Non-Public Members 
	}
}
