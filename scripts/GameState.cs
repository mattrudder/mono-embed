
using System;

namespace TalonScript
{
	[Native]
	public abstract class GameState
	{	
		GameState()
		{
			Random rand = new Random();
			m_fStoppingPoint = (float)rand.NextDouble() * 5.0f;
			Console.Error.WriteLine("Generated stopping point: {0}", m_fStoppingPoint);
		}
		
		public bool Update(float fTimeDelta, string message)
		{
			Console.Error.WriteLine("Updated with delta of {0}, stopping at {1}: {2}", fTimeDelta, m_fStoppingPoint, message);
			return fTimeDelta < m_fStoppingPoint;
		}

		public string GetState(string message)
		{
			Console.Error.WriteLine("Message: {0}", message);
			return message + " World!";
		}

		public GameState TransitionToState(GameState state)
		{
			Console.Error.WriteLine("Transitioning...");
			return this;
		}
		
		// Virtual methods called by the engine runtime.
		protected virtual void OnActivated()
		{
			Console.WriteLine("OnActivated Called!");
		}
		
		protected virtual void OnDeactivated()
		{
			Console.WriteLine("OnDeactivated Called!");
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