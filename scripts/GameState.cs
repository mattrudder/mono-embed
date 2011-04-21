
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
		[Native]
		protected virtual void OnActivated()
		{
		}
		
		protected virtual void OnDeactivated()
		{
		}
		
		// --- Exposing managed code easily ---
		// Attempt 1: Attribute-marked code generation
		// [NativeVisibleAttribute] marked methods generate function definitions before native build
		// 
		// C#:
		//		[NativeVisible]
		//		protected virtual void OnActivated() { }
		//
		// MonoGenerated/GameState.h:
		//	protected:
		//		virtual OnActivated();
		//
		// MonoGenerated/GameState.cpp:
		// 		GameState::OnActivated()
		//		{
		//			
		//		}
		
		
		
		
		// Private (by convention) methods with native implementations
		// Internal call via monobind
		
		float m_fStoppingPoint;
	}
}