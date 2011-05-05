
#pragma once
#include "ScriptedObject.h"

namespace Talon
{
	class GameState : public Scriptable<GameState>
	{
	public:
		GameState();

		bool IsActivated() const;
		
		void Activate();
		void Deactivate();

		static void ExposeScript(ScriptType& type);
		
	private:
		bool m_bIsActivated;
		
	#include "Generated/GameState.g.h"
	};
}