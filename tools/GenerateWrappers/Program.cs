using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using NDesk.Options;

namespace GenerateWrappers
{
	class Program
	{
		static void Main(string[] args)
		{
			List<string> listExtraArguments = s_options.Parse(args);

			// TODO: Verify required arguments 
			if (s_bShowHelp)
			{
				ShowHelp();
				return;
			}

			ModuleDefinition mainModule = ModuleDefinition.ReadModule(c_strMainModule);
			TypeDefinition wrapperAttributeType = mainModule == null ? null : mainModule.Types.FirstOrDefault(t => string.Equals(t.Name, c_strWrapperAttribute, StringComparison.InvariantCultureIgnoreCase));

			if (wrapperAttributeType == null)
			{
				Console.Error.WriteLine("Wrapper attribute type \"{0}\" not defined in \"{1}\"!", c_strWrapperAttribute, c_strMainModule);
				return;
			}

			Console.WriteLine("Generating Wrappers...");
			foreach (var assemblyPath in s_listAssemblies)
			{
				string assemblyFullPath = Path.GetFullPath(assemblyPath);
				if (!File.Exists(assemblyFullPath))
				{
					Console.Error.WriteLine("Assembly file \"{0}\" does not exist!", assemblyFullPath);
					continue;
				}

				//TypeReference typeRef = new TypeReference("TalonScript", "NativeAttribute", mainModule, IMetadataScope);
				Console.WriteLine("Inspecting {0}...", assemblyFullPath);
				ModuleDefinition definition = ModuleDefinition.ReadModule(assemblyPath);
				foreach (var type in definition.Types)
				{
					if (type.FullName == wrapperAttributeType.FullName || !type.Methods.Any(m => !m.CustomAttributes.Any(a => a.AttributeType.FullName == wrapperAttributeType.FullName)))
						continue;

					Console.WriteLine("Inspecting {0}...", type.Name);
					
					foreach (var method in type.Methods)
					{
						CustomAttribute wrapperAttribute = method.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == wrapperAttributeType.FullName);
						if (wrapperAttribute != null)
						{
							Console.WriteLine("Wrapping {0}:{1}", type.Name, method.Name);
						}
					}
				}
			}
			
		}

		private static void ShowHelp()
		{
			Console.WriteLine("Usage: {0} [options]+", Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
			Console.WriteLine("Generates Mono wrappers implemented by a set of provided template files.");
			Console.WriteLine();
			Console.WriteLine("Options:");
			s_options.WriteOptionDescriptions(Console.Out);
		}

		private const string c_strMainModule = "TalonScript.dll";
		private const string c_strWrapperAttribute = "NativeAttribute";

		static readonly OptionSet s_options = new OptionSet()
		{
			{ "?|help", "Displays this help text.", v => s_bShowHelp = v != null },
			{ "a|asm=", "Adds an assembly to inspect for wrappable types.", v => s_listAssemblies.Add(v) },
			{ "o|output=", "Provides the path to generated wrapper files.", v => s_outputPath = v }
		};

		private static readonly List<string> s_listAssemblies = new List<string>();
		private static string s_outputPath;
		private static bool s_bShowHelp;
	}
}
