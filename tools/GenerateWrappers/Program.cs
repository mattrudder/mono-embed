using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GenerateWrappers.Templates;
using Mono.Cecil;
using NDesk.Options;

using System.CodeDom.Compiler;

namespace GenerateWrappers
{
	//public class WrapperTemplate : ITextTemplatingEngineHost
	//{
	//    public object GetHostOption(string optionName)
	//    {
	//        return null;
	//    }

	//    public bool LoadIncludeText(string requestFileName, out string content, out string location)
	//    {
	//        content = String.Empty;
	//        location = String.Empty;

	//        if (File.Exists(requestFileName))
	//        {
	//            content = File.ReadAllText(requestFileName);
	//            return true;
	//        }
		
	//        return false;
	//    }

	//    public void LogErrors(CompilerErrorCollection errors)
	//    {
	//        TemplateErrors = errors;
	//    }

	//    public AppDomain ProvideTemplatingAppDomain(string content)
	//    {
	//        return AppDomain.CreateDomain("Generation App Domain");
	//    }

	//    public string ResolveAssemblyReference(string assemblyReference)
	//    {
	//        if (File.Exists(assemblyReference))
	//            return assemblyReference;

	//        string candidate = Path.Combine(Path.GetDirectoryName(TemplateFile), assemblyReference);
	//        if (File.Exists(candidate))
	//            return candidate;

	//        return "";
	//    }

	//    public Type ResolveDirectiveProcessor(string processorName)
	//    {
	//        throw new Exception("Directive Processor not found");
	//    }

	//    public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
	//    {
	//        if (directiveId == null)
	//            throw new ArgumentNullException("directiveId");
	//        if (processorName == null)
	//            throw new ArgumentNullException("processorName");
	//        if (parameterName == null)
	//            throw new ArgumentNullException("parameterName");

	//        return String.Empty;
	//    }

	//    public string ResolvePath(string path)
	//    {
	//        if (path == null)
	//            throw new ArgumentNullException("path");

	//        if (File.Exists(path))
	//            return path;

	//        string candidate = Path.Combine(Path.GetDirectoryName(TemplateFile), path);
	//        if (File.Exists(candidate))
	//            return candidate;

	//        return path;
	//    }

	//    public void SetFileExtension(string extension)
	//    {
	//        m_defaultFileExtension = extension;
	//    }

	//    public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
	//    {
	//        m_fileEncoding = encoding;
	//    }

	//    public IList<string> StandardAssemblyReferences
	//    {
	//        get { return new[] { typeof(Uri).Assembly.Location }; }
	//    }

	//    public IList<string> StandardImports
	//    {
	//        get { return new[] { "System" }; }
	//    }

	//    public string TemplateFile { get; internal set; }

	//    public CompilerErrorCollection TemplateErrors { get; private set; }

	//    private string m_defaultFileExtension;
	//    private Encoding m_fileEncoding;
	//}

	class Program
	{
		static void Main(string[] args)
		{
			List<string> listExtraArguments = s_options.Parse(args);

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


			//s_textEngine = new Engine();

			//CppHeaderTemplate template = new CppHeaderTemplate();
			
			List<ClassModel> wrappedClasses = new List<ClassModel>();
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
					if (type.FullName == wrapperAttributeType.FullName || !type.Methods.Any(m => !m.CustomAttributes.Any(a => a.AttributeType.FullName == wrapperAttributeType.FullName)))
						continue;

					ClassModel klass = new ClassModel
					{
						Name = type.Name,
						FullName = type.FullName,
					};

					Console.WriteLine("Inspecting {0}...", type.Name);
					
					foreach (var method in type.Methods)
					{
						CustomAttribute wrapperAttribute = method.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == wrapperAttributeType.FullName);
						if (wrapperAttribute != null)
						{
							// TODO: Add MethodModel
							MethodModel model = new MethodModel
							{
								ScriptName = method.Name,
								NativeName = method.Name,
							};
							
							klass.Methods.Add(model);
						}
					}

					if (klass.Methods.Count > 0)
						wrappedClasses.Add(klass);
				}
			}

			ProcessTemplates(wrappedClasses);
		}

		private static bool HasValidArguments()
		{
			return s_listAssemblies.Count > 0 && !string.IsNullOrEmpty(s_outputPath);
		}

		private static void ProcessTemplates(IEnumerable<ClassModel> wrappedClasses)
		{
			if (!Directory.Exists(s_outputPath))
				Directory.CreateDirectory(s_outputPath);

			CppHeaderTemplate template = new CppHeaderTemplate();
			foreach (ClassModel klass in wrappedClasses)
			{
				// TODO: Process template with ClassModel.
				// TODO: Make this let dependent on a given wrapper type.

				template.Class = klass;
				string wrapperPath = Path.Combine(s_outputPath, template.WrapperFilename);
				Console.WriteLine("Wrapping {0} -> {1}", klass.FullName, Path.GetFileName(wrapperPath));

				using (TextWriter tw = new StreamWriter(wrapperPath))
					tw.Write(template.TransformText());
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
			{ "t|template", "Adds a template to generate wrapper files.", v => s_listTemplates.Add(v) },
			{ "o|output=", "Provides the path to generated wrapper files.", v => s_outputPath = v }
		};

		private static readonly List<string> s_listAssemblies = new List<string>(new [] { c_strMainModule });
		private static readonly List<string> s_listTemplates = new List<string>();
		private static string s_outputPath = Path.Combine(Directory.GetCurrentDirectory(), "Generated");
		private static bool s_bShowHelp;
		//private static Engine s_textEngine;
	}
}
