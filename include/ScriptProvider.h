
#pragma once

#include <mono/jit/jit.h>

#include <mono/metadata/mono-config.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/class.h>
#include <mono/metadata/debug-helpers.h>

#include "monobind/module.hpp"

#include <vector>

namespace Talon
{
	class ScriptType;

	class ScriptProvider
	{
	public:
		static ScriptProvider Instance;
		static MonoDomain* ScriptDomain;
		static MonoAssembly* ScriptAssembly;
		static MonoImage* ScriptAssemblyImage;
		static monobind::module Module;

		static void Initialize();
		static void Shutdown();

		static void RegisterType(ScriptType* pType);

	private:
		static std::vector<ScriptType*>* s_registeredTypes;
	};
}