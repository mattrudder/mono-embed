
#pragma once
#include "ScriptedObject.h"

namespace Talon
{
	class GameState : public ScriptedObject<GameState>
	{
	public:
		GameState();

		bool IsActivated() const;
		
		void Activate();
		void Deactivate();

		static void ExposeScript(ScriptType& type);
		
	protected:
		virtual void OnActivated();
		virtual void OnDeactivated();
		
	private:
//		static ScriptMethod* s_fnOnActivated;
//		static ScriptMethod* s_fnOnDeactivated;

		bool m_bIsActivated;
	};
}