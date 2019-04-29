using System;

namespace SEARCH
{
	/// <summary>
	/// Exception used to identify bad xml load file
	/// </summary>
	internal sealed class CraptasticXmlException : System.ApplicationException
	{
		#region Public Members (1) 

		#region Constructors (1) 

		public CraptasticXmlException (string msg):base(msg)
		{
		}

		#endregion Constructors 

		#endregion Public Members 
	}
}
