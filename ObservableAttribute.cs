namespace SEARCH
{
	/// <summary>
	/// This class represents a field that other classes care about.  When this class's value changes, it iterates through its list of registered observers and calls their update method.
	/// </summary>
	public class ObservableAttribute
	{
		#region Public Members (3) 

		#region Properties (1) 

		/// <summary>
		/// This is the value of the attribute that the observers care about.  The set method needs to call the notifyObsrvers method to let all the observers know whenever the value changes.
		/// </summary>
		public object value
		{
			get
			{
				return mvalue;
			}
			set
			{
				mvalue = value;
			}
		}

		#endregion Properties 
		#region Methods (2) 

		/// <summary>
		/// Used to add IAttributeObserver to a collection of objects to be notified.
		/// </summary>
		public void registerObserver(IAttributeObserver observer)
		{
		}

		/// <summary>
		/// Used to remove an IAttributeObserver from the collection of observers.
		/// </summary>
		public void removeObsever(IAttributeObserver observer)
		{
		}

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (2) 

		#region Fields (1) 

		private object mvalue;

		#endregion Fields 
		#region Methods (1) 

		/// <summary>
		/// Used to let IAtributeObserves know when the value changes.  Called from the set method, it iterates through the list of observers and calls their update method.
		/// </summary>
		private void notifyObservers()
		{
		}

		#endregion Methods 

		#endregion Non-Public Members 
	}
}
