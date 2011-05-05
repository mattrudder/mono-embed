
#include <mono/jit/jit.h>

#include <mono/metadata/mono-config.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/class.h>
#include <mono/metadata/debug-helpers.h>

#include <stdlib.h>

#include "main.h"
#include "GameState.h"
using namespace Talon;

MonoImage* g_asmTalonScript = NULL;


void run_simulation()
{
	MonoAssembly* asmScript = ScriptProvider::ScriptAssembly;
	if (asmScript)
	{
		MonoImage* image = mono_assembly_get_image(asmScript);
		MonoClass* klass = mono_class_from_name(image, "TalonScript", "GameState");
		
		MonoObject* gameState = mono_object_new(ScriptProvider::ScriptDomain, klass);
		mono_runtime_object_init(gameState);
		
		MonoMethod* updateMethod = mono_class_get_method_from_name(klass, "Update", 1);
		
		typedef bool (*UpdateFunc)(MonoObject*, float);
		//UpdateFunc fnUpdate = (UpdateFunc)mono_method_get_unmanaged_thunk(updateMethod);
		//GameState state;
		//state.Activate();

		float fDelta = 0.0f;
		bool bRunning = true;
		while (bRunning)
		{
			// Setup arguments
			MonoObject* exception = NULL;
			void* args[1];
			args[0] = &fDelta;
			
			// Call the update method.
			MonoObject* result = mono_runtime_invoke(updateMethod, gameState, args, &exception);
			
			// Update() returns a value type (bool), need to unbox it.
			bRunning = *(bool*)mono_object_unbox(result);
			
			if (exception)
			{
				printf("Exception thrown!");
				mono_print_unhandled_exception(exception);
				return;
			}
			
			fDelta += 0.1f;
		}

		//state.Deactivate();
	}
}

int main(int argc, const char** argv)
{
	
//	mono_config_parse(NULL);
	
	ScriptProvider::Initialize();
	
	printf("[main] GameState::TypeName = %s\n", GameState::Type.GetName());
	GameState state;
	state.Activate();
	printf("Managed return: '%s'\n", state.GetState("Hello"));

	GameState newState;
	GameState* previousState = state.TransitionToState(&newState);

	printf("Current state: %x, New state: %x, Previous state: %x", state.GetMonoObject(), newState.GetMonoObject(), previousState->GetMonoObject());

	float fDelta = 0.0f;
	bool bRunning = true;
	while (bRunning)
	{
		bRunning = state.Update(fDelta, "win!");
		fDelta += 0.1f;
	}

	//run_simulation();
	
	ScriptProvider::Shutdown();
	
	return 0;
}