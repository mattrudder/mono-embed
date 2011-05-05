using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace GenerateWrappers
{
	public static class NativeHelpers
	{
		public static IEnumerable<MethodDefinition> FilterWrappableMethods(this IEnumerable<MethodDefinition> methods)
		{
			return methods.Where(m => !m.IsConstructor);
		}

		public static bool IsWrapperType(this TypeReference managedType)
		{
			return s_listWrapperClasses.Contains(managedType.Resolve());
		}

		public static string GetNativeType(this TypeReference managedType)
		{
			TypeConversionInfo info = managedType.GetTypeConversionInfo();
			if (info != null)
				return info.NativeType;
			if (managedType.IsValueType)
				return managedType.Name;

			return "Scriptable*";
		}

		public static string GetThunkType(this TypeReference managedType)
		{
			TypeConversionInfo info = managedType.GetTypeConversionInfo();
			return info != null ? info.ThunkType ?? info.NativeType : "MonoObject*";
		}

		public static TypeConversionInfo GetTypeConversionInfo(this TypeReference managedType)
		{
			TypeConversionInfo typeConverterInfo;
			string managedTypeName = managedType.FullName;
			if (!s_dictManagedToNative.TryGetValue(managedTypeName, out typeConverterInfo))
				Console.Error.WriteLine("Type conversion undefined for '{0}'", managedTypeName);

			return typeConverterInfo;
		}

		public static void RegisterWrappedTypes(IEnumerable<TypeDefinition> types)
		{
			s_listWrapperClasses = types.ToList();
			foreach (TypeDefinition type in types)
			{
				TypeConversionInfo info;
				if (!s_dictManagedToNative.TryGetValue(type.FullName, out info))
					s_dictManagedToNative[type.FullName] = new TypeConversionInfo(type.Name + "*", "MonoObject*");
			}
		}

		public static string GetNativeArgumentList(this MethodDefinition method)
		{
			StringBuilder sb = new StringBuilder();
			foreach (ParameterDefinition parameter in method.Parameters)
			{
				string parameterType = parameter.ParameterType.GetNativeType();
				string parameterName = parameter.Name;

				if (sb.Length > 0)
					sb.Append(", ");

				sb.AppendFormat("{0} {1}", parameterType, parameterName);
			}

			return sb.ToString();
		}

		public static string GetNativeReturnType(this MethodDefinition method)
		{
			return method.ReturnType.GetNativeType();
		}

		public static string GetNativeProtection(this MethodDefinition method)
		{
			if (method.IsPrivate)
				return "private";
			if (method.IsPublic)
				return "public";
			
			return "protected";
		}

		public static string GetNativeFuncPtrName(this MethodDefinition method)
		{
			// TODO: Protect against name collisions?
			return "m_fn" + method.Name;
		}

		public static string GetThunkReturnType(this MethodDefinition method)
		{
			string thunkType = method.ReturnType.GetThunkType();
			if (thunkType != null)
				return thunkType;
			if (method.ReturnType.IsValueType)
				return method.ReturnType.GetNativeType();

			return "MonoObject*";
		}
		public static string GetThunkArgumentList(this MethodDefinition method)
		{
			StringBuilder sb = new StringBuilder("MonoObject* obj");
			foreach (ParameterDefinition parameter in method.Parameters)
			{
				string parameterType = parameter.ParameterType.GetNativeType();
				string parameterName = parameter.Name;

				if (sb.Length > 0)
					sb.Append(", ");

				sb.AppendFormat("{0} {1}", parameterType, parameterName);
			}

			return sb.ToString();
		}

		public static List<string> GetMethodParameters(this MethodDefinition method)
		{
			List<string> parameterList = method.Parameters.Select(p => p.Name).ToList();
			return method.Parameters.Select(p => p.Name).ToList();
		}

		private static readonly Dictionary<string, TypeConversionInfo> s_dictManagedToNative = new Dictionary<string, TypeConversionInfo>
		{
			{ "System.Void", new TypeConversionInfo("void") },
			{ "System.Boolean", new TypeConversionInfo("bool") },
			{ "System.Single", new TypeConversionInfo("float") },
            { "System.Double", new TypeConversionInfo("double") },
            { "System.String", new TypeConversionInfo("const char*", "MonoString*") }                                          		
		};

		private static List<TypeDefinition> s_listWrapperClasses;
	}
}