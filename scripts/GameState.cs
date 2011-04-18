
using System;

namespace TalonScript
{
	public abstract class GameState
	{	
		GameState()
		{
			Random rand = new Random();
			m_fStoppingPoint = (float)rand.NextDouble() * 5.0f;
			Console.Error.WriteLine("Generated stopping point: {0}", m_fStoppingPoint);
		}
		
		bool Update(float fTimeDelta)
		{
			Console.Error.WriteLine("Updated with delta of {0}, stopping at {1}.", fTimeDelta, m_fStoppingPoint);
			return fTimeDelta < m_fStoppingPoint;
		}
		
		// Virtual methods called by the engine runtime.
		protected virtual void OnActivated()
		{
		}
		
		protected virtual void OnDeactivated()
		{
		}
		
		// Private (by convention) methods with native implementations
		// Internal call vs P/Invoke
		
		
		float m_fStoppingPoint;
	}
}