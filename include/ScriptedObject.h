
#pragma once

#include "ScriptType.h"

extern "C" {
//#include <gc.h>
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/assembly.h>
}

#include <monobind/function.hpp>
#include <monobind/class.hpp>
#include <monobind/namespace.hpp>
#include <monobind/scope.hpp>
#include <monobind/module.hpp>

namespace Talon
{
	template <class TObjectType>
	class ScriptedObject
	{
	public:
		static ScriptType Type;
	private:

	};
}