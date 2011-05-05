
#include "ScriptProvider.h"
#include "GameState.h"

namespace Talon
{	
	template <>
	ScriptType Scriptable<GameState>::Type("GameState", &GameState::ExposeScript);
	
#include "Generated/GameState.g.cpp"
	
	void GameState::ExposeScript(ScriptType& type)
	{
		using namespace monobind;
		ScriptProvider::Module
		[
			namespace_( "TalonScript" )
			[
				class_< GameState >( "GameState" )
					.def( constructor() )
					.def( "Activate", &GameState::Activate )
					.def( "Deactivate", &GameState::Deactivate )
			]
		];
	}

	GameState::GameState()
		: m_bIsActivated(false)
	{
	}
	
	void GameState::Activate()
	{
		m_bIsActivated = true;
		OnActivated();
	}
	
	void GameState::Deactivate()
	{
		OnDeactivated();
		m_bIsActivated = false;
	}
}