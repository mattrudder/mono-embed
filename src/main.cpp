
#include <mono/jit/jit.h>

#include <mono/metadata/mono-config.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/class.h>
#include <mono/metadata/debug-helpers.h>

#include <stdlib.h>

#include "GameState.h"
using namespace Talon;

void run_simulation(const char* szFilename, MonoDomain* domain)
{
	MonoAssembly* asmScript = mono_domain_assembly_open(domain, szFilename);
	if (asmScript)
	{
		MonoImage* image = mono_assembly_get_image(asmScript);
		MonoClass* klass = mono_class_from_name(image, "TalonScript", "GameState");
		
		MonoObject* gameState = mono_object_new(domain, klass);
		mono_runtime_object_init(gameState);
		
		MonoMethod* updateMethod = mono_class_get_method_from_name(klass, "Update", 1);
		
		typedef bool (*UpdateFunc)(MonoObject*, float);
		//UpdateFunc fnUpdate = (UpdateFunc)mono_method_get_unmanaged_thunk(updateMethod);
		
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
	}
}

int main(int argc, const char** argv)
{
	MonoDomain* domain;
//	mono_config_parse(NULL);
	
	const char* szScriptAssembly = "Talon.Sandbox.Script.dll";
	domain = mono_jit_init(szScriptAssembly);
	
	printf("GameState::TypeName = %s", GameState::TypeName);
	run_simulation(szScriptAssembly, domain);
	
	mono_jit_cleanup(domain);
	
	return 0;
}