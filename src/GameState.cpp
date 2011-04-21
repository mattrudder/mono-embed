
#include "ScriptProvider.h"
#include "GameState.h"

namespace Talon
{	
	template <>
	ScriptType ScriptedObject<GameState>::Type("GameState", &GameState::ExposeScript);
	
	void GameState::ExposeScript(ScriptType& type)
	{
//		s_fnOnActivated = type.GetMethod("OnActivated");
//		s_fnOnDeactivated = type.GetMethod("OnDeactivated");

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
	
	// Default virtual method calls on script objects forward work to script layer.
	void GameState::OnActivated()
	{
//		s_fnOnActivated->Invoke(this);
	}
	
	void GameState::OnDeactivated()
	{
//		s_fnOnDeactivated->Invoke(this);
	}
}