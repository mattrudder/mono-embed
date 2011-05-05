using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using Mono.Cecil;
using NDesk.Options;

namespace GenerateWrappers
{
	class Program
	{
		static void Main(string[] args)
		{
			List<string> listExtraArguments = s_options.Parse(args);

			ProvideDefaults();

			// TODO: Verify required arguments 
			if (!HasValidArguments() || s_bShowHelp)
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

			List<TypeDefinition> wrappedClasses = new List<TypeDefinition>();
			Console.WriteLine("Generating Wrappers...");
			foreach (var assemblyPath in s_listAssemblies)
			{
				string assemblyFullPath = Path.GetFullPath(assemblyPath);
				if (!File.Exists(assemblyFullPath))
				{
					Console.Error.WriteLine("Assembly file \"{0}\" does not exist!", assemblyFullPath);
					continue;
				}

				Console.WriteLine("Inspecting {0}...", assemblyFullPath);
				ModuleDefinition definition = ModuleDefinition.ReadModule(assemblyPath);
				foreach (var type in definition.Types)
				{
					CustomAttribute wrapperAttribute = type.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == wrapperAttributeType.FullName);
					if (type.FullName == wrapperAttributeType.FullName || wrapperAttribute == null)
						continue;

					wrappedClasses.Add(type);
				}
			}

			NativeHelpers.RegisterWrappedTypes(wrappedClasses);

			ProcessTemplates(wrappedClasses);
		}

		private static void ProvideDefaults()
		{
			// Default templates
			// TODO: Bundle default templates in the app?
			if (s_listTemplates.Count == 0)
				s_listTemplates.AddRange(new [] { "CppHeaderTemplate.t4", "CppSourceTemplate.t4" } );
		}

		private static bool HasValidArguments()
		{
			return s_listAssemblies.Count > 0 && !string.IsNullOrEmpty(s_outputPath);
		}

		private static void ProcessTemplates(IEnumerable<TypeDefinition> wrappedClasses)
		{
			if (!Directory.Exists(s_outputPath))
				Directory.CreateDirectory(s_outputPath);

			foreach (var template in s_listTemplates)
			{
				string templatePath = ResolveTemplatePath(template);
				if (templatePath == null)
					continue;

				s_textHost.TemplateFile = Path.GetFullPath(templatePath);
				string templateText = File.ReadAllText(s_textHost.TemplateFile);
				if (string.IsNullOrEmpty(templateText))
				{
					Console.Error.WriteLine("Unable to load template.");
					continue;
				}

				foreach (TypeDefinition klass in wrappedClasses)
				{
					s_textHost.CurrentClass = klass;

					string templateOutput = s_textEngine.ProcessTemplate(templateText, s_textHost);
					if (s_textHost.TemplateErrors.HasErrors)
					{
						foreach (CompilerError error in s_textHost.TemplateErrors)
							Console.Error.WriteLine(error.ErrorText);

						break;
					}

					string wrapperPath = Path.Combine(s_outputPath, s_textHost.OutputFilename);
					using (TextWriter tw = new StreamWriter(wrapperPath))
					{
						Console.WriteLine("Generating {0}...", Path.GetFileName(wrapperPath));
						tw.Write(templateOutput);
					}
				}
			}
		}

		private static string ResolveTemplatePath(string templatePath)
		{
			if (!File.Exists(templatePath))
			{
				templatePath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Templates", templatePath));
				if (!File.Exists(templatePath))
				{
					Console.Error.WriteLine("Unable to locate template path.");
					templatePath = null;
				}
			}

			return templatePath;
		}

		private static void ShowHelp()
		{
			Console.WriteLine("Usage: {0} [options]+", Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
			Console.WriteLine("Generates Mono wrappers implemented by a set of provided template files.");
			Console.WriteLine();
			Console.WriteLine("Options:");
			s_options.WriteOptionDescriptions(Console.Out);
		}

		static Program()
		{
			s_listAssemblies = new List<string>(new [] { c_strMainModule });
			s_listTemplates = new List<string>();
			s_textEngine = new Engine();
			s_textHost = new WrapperTemplateHost();
			s_outputPath = Path.Combine(Directory.GetCurrentDirectory(), "../../include/Generated");
		}

		private const string c_strMainModule = "TalonScript.dll";
		private const string c_strWrapperAttribute = "NativeAttribute";

		private static readonly OptionSet s_options = new OptionSet()
		{
			{ "?|help", "Displays this help text.", v => s_bShowHelp = v != null },
			{ "a|asm=", "Adds an assembly to inspect for wrappable types.", v => s_listAssemblies.Add(v) },
			{ "t|template", "Adds a template to generate wrapper files.", v => s_listTemplates.Add(v) },
			{ "o|output=", "Provides the path to generated wrapper files.", v => s_outputPath = v }
		};

		private static readonly List<string> s_listAssemblies;
		private static readonly List<string> s_listTemplates;
		private static readonly Engine s_textEngine;
		private static readonly WrapperTemplateHost s_textHost;
		private static string s_outputPath;
		private static bool s_bShowHelp;

		//private static Engine s_textEngine;
	}
}
