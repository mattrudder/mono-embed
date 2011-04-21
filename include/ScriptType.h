
#pragma once

#include <string>
#include <mono/metadata/loader.h>

namespace Talon
{
	class ScriptType
	{
	public:
		friend class ScriptProvider;

		typedef void (*TypeMapFunc)(ScriptType&);

		ScriptType(const char* typeName, TypeMapFunc fnMapType)
			: m_typeName(typeName)
			, m_fnMapType(fnMapType)
		{
			ScriptProvider::RegisterType(this);
		}

		inline const char* GetName() { return m_typeName.c_str(); }

		//inline ScriptMethod* GetMethod(const char* methodName)
		//{
		//	// TODO: Implement GetMethod - Retrieve a handle to managed method, for invocation from native code.
		//	return NULL;
		//}

		template <class TFuncType>
		inline void ExposeMethod(const char* methodName, void* fnCall)
		{
			// TODO: Implement ExposeMethod - Register a native method for invocation from managed code.
			mono_add_internal_call(methodName, fnCall);
		}

	protected:
		void MapType()
		{
			// Let the type map itself.
			if (m_fnMapType)
				(*m_fnMapType)(*this);
		}

	private:
		TypeMapFunc m_fnMapType;
		std::string m_typeName;
	};
}