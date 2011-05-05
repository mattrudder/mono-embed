
#pragma once

extern "C" {
//#include <gc.h>
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/environment.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/exception.h>
#include <mono/metadata/object.h>
}

#include <boost/static_assert.hpp>

namespace Talon { namespace Script
{
	template <class TInput, class TReturn>
	class TypeConverter
	{
	public:
		typedef TInput InputType;
		typedef TReturn ReturnType;

		static TReturn Convert(TInput input)
		{
			BOOST_STATIC_ASSERT(sizeof(TReturn) == 0); 
		}
	};

	// -- String type conversions --
	template <>
	class TypeConverter<const char*, MonoString*>
	{
	public:
		typedef const char* InputType;
		typedef MonoString* ReturnType;

		static MonoString* Convert(const char* input)
		{
			return mono_string_new(mono_domain_get(), input);
		}
	};

	template <>
	class TypeConverter<MonoString*, const char*>
	{
	public:
		typedef MonoString* InputType;
		typedef const char* ReturnType;

		static const char* Convert(MonoString* input)
		{
			return mono_string_to_utf8(input);
		}
	};
}}