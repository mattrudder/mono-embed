
#include "GameState.h"

namespace Talon
{	
	template <>
	const char* ScriptedObject<GameState>::TypeName = "GameState";
	
	GameState::GameState()
	{
		
	}
	
	void GameState::Activate()
	{	
	}
	
	void GameState::Deactivate()
	{	
	}
	
	// Default virtual method calls on script objects forward work to script layer.
	void GameState::OnActivated()
	{
	}
	
	void GameState::OnDeactivated()
	{
		
	}
}