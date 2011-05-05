
#pragma once

#include "ScriptType.h"
//#include "ScriptConversions.h"

extern "C" {
//#include <gc.h>
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/exception.h>
#include <mono/metadata/object.h>
}

#include <monobind/function.hpp>
#include <monobind/class.hpp>
#include <monobind/namespace.hpp>
#include <monobind/scope.hpp>
#include <monobind/module.hpp>

#include "ScriptConversions.h"

// Mono unmanaged thunks must be called as __stdcall on Windows.
#ifdef WIN32
#define MONO_THUNK_DECL __stdcall
#else
#define MONO_THUNK_DECL
#endif

extern MonoImage* g_asmTalonScript;

namespace Talon
{
	class ScriptedObject
	{
	public:
		ScriptedObject(MonoObject* pObject = NULL)
			: m_pMonoObject(pObject)
		{
		}

		inline MonoObject* GetMonoObject() const { return m_pMonoObject; }
	
	protected:
		inline void LogScriptException(MonoException* e)
		{
			printf("An exception was thrown from script.");
			mono_print_unhandled_exception((MonoObject*)e);
		}

		inline MonoMethod* GetMethod(const char* methodName, const char* args, MonoClass* klass)
		{
			MonoMethod* method;
			void* iterator = NULL;

			while ((method = mono_class_get_methods (klass, &iterator)))
			{
				if (IsMethodMatch(methodName, args, method))
					return method;
			}

			return NULL;
		}

		inline bool IsMethodMatch (const char* methodName, const char* args, MonoMethod *method)
		{
			const char* monoMethodName = mono_method_get_name(method);
			bool bIsMatch = _stricmp(methodName, monoMethodName) == 0;
			if (!bIsMatch)
				return false;
			if (args == NULL || args[0] == '\0')
				return true;

			const char* monoMethodSignature = mono_signature_get_desc(mono_method_signature(method), false);
			return _stricmp(args, monoMethodSignature) == 0;
		}

	protected:
		MonoObject* m_pMonoObject;
	};

	template <class TObjectType>
	class Scriptable : public ScriptedObject
	{
	public:
		static ScriptType Type;

		Scriptable(MonoObject* pObject = NULL)
			: ScriptedObject(pObject)
		{
		}

		inline void VerifyLifetime()
		{
			if (m_pMonoObject != NULL)
				return;

			if (s_pMonoClass == NULL)
				s_pMonoClass = mono_class_from_name(ScriptProvider::ScriptAssemblyImage, GetScriptNamespace(), GetScriptName());

			m_pMonoObject = mono_object_new(ScriptProvider::ScriptDomain, s_pMonoClass);
			mono_runtime_object_init(m_pMonoObject);
			
			// TODO: Additional initalization of native type.
		}

	protected:
		const char* GetScriptNamespace() const;
		const char* GetScriptName() const;

		static inline MonoClass* GetMonoClass() { return s_pMonoClass; }

	private:
		static MonoClass* s_pMonoClass;
	};

	template <class TObjectType>
	MonoClass* Scriptable<TObjectType>::s_pMonoClass = NULL;

	
	// -- Wrapper type conversions, defined separately to avoid circular includes --
	namespace Script
	{
		template <>
		class TypeConverter<ScriptedObject*, MonoObject*>
		{
		public:
			typedef ScriptedObject* InputType;
			typedef MonoObject* ReturnType;

			static MonoObject* Convert(ScriptedObject* input)
			{
				return input != NULL ? input->GetMonoObject() : NULL;
			}
		};
	}
}