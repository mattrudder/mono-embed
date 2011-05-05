
#include "ScriptProvider.h"
#include "ScriptType.h"

namespace Talon
{
	ScriptProvider ScriptProvider::Instance;
	MonoDomain* ScriptProvider::ScriptDomain = NULL;
	MonoAssembly* ScriptProvider::ScriptAssembly = NULL;
	MonoImage* ScriptProvider::ScriptAssemblyImage = NULL;
	monobind::module ScriptProvider::Module(NULL, NULL);

	std::vector<ScriptType*>* ScriptProvider::s_registeredTypes = NULL;

	void ScriptProvider::Initialize()
	{
		const char* szScriptAssembly = "TalonScript.dll";
		ScriptDomain = mono_jit_init(szScriptAssembly);
		ScriptAssembly = mono_domain_assembly_open(ScriptDomain, szScriptAssembly);
		ScriptAssemblyImage = mono_assembly_get_image(ScriptAssembly);

		Module.SetDomain(ScriptDomain);
		Module.SetImage(ScriptAssemblyImage);

		printf("Mapping Scripted Types:\n");
		std::vector<ScriptType*>::iterator itCurrent = s_registeredTypes->begin();
		std::vector<ScriptType*>::iterator itEnd = s_registeredTypes->end();
		while (itCurrent != itEnd)
		{
			ScriptType* pType = *itCurrent;
			printf("\t%s\n", pType->GetName());
			
			pType->MapType();

			itCurrent++;
		}
	}

	void ScriptProvider::Shutdown()
	{
		mono_jit_cleanup(ScriptDomain);
	}

	void ScriptProvider::RegisterType(ScriptType* pType)
	{
		if (s_registeredTypes == NULL)
			s_registeredTypes = new std::vector<ScriptType*>;

		s_registeredTypes->push_back(pType);
	}
}