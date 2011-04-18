
#pragma once
#include "ScriptedObject.h"

namespace Talon
{
	class GameState : public ScriptedObject<GameState>
	{
	public:
		GameState();
		
		void Activate();
		void Deactivate();
		
	protected:
		virtual void OnActivated();
		virtual void OnDeactivated();
		
	private:
	};
}