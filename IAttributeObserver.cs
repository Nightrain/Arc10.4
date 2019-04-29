namespace SEARCH
{
	/// <summary>
	/// This interface defines the update method used to participate in the observer pattern
	/// </summary>
	public interface IAttributeObserver
	{
		#region Operations (1) 

		/// <summary>
		/// This method gets called by the attribute being observed.  It is used by the observer to update whatever needs updating when the observed attribute changes.
		/// </summary>
		void update(object newValue, object oldValue);

		#endregion Operations 
	}
}
