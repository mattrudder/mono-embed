using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using Mono.Cecil;
using Talon.Utility;

namespace GenerateWrappers
{
	public class WrapperTemplateHost : ITextTemplatingEngineHost
	{
		public WrapperTemplateHost()
		{
			TemplateErrors = new CompilerErrorCollection();
		}

		public object GetHostOption(string optionName)
		{
			object returnObject = null;
			switch (optionName)
			{
				case "CacheAssemblies":
					returnObject = true;
					break;
				case "Class":
					returnObject = CurrentClass;
					break;
			}
			return returnObject;
		}

		public bool LoadIncludeText(string requestFileName, out string content, out string location)
		{
			content = String.Empty;
			location = String.Empty;

			// TODO: Support Embedded Resources

			if (File.Exists(requestFileName))
			{
				content = File.ReadAllText(requestFileName);
				location = requestFileName;
				return true;
			}

			string templatePath = Path.Combine("Templates", requestFileName);
			if (File.Exists(templatePath))
			{
				content = File.ReadAllText(templatePath);
				location = templatePath;
				return true;
			}

			return false;
		}

		public void LogErrors(CompilerErrorCollection errors)
		{
			TemplateErrors.AddRange(errors);
		}

		public AppDomain ProvideTemplatingAppDomain(string content)
		{
			return null;
		}

		public string ResolveAssemblyReference(string assemblyReference)
		{
			if (File.Exists(assemblyReference))
				return assemblyReference;

			string candidate = Path.Combine(Path.GetDirectoryName(TemplateFile), assemblyReference);
			if (File.Exists(candidate))
				return candidate;

			return "";
		}

		public Type ResolveDirectiveProcessor(string processorName)
		{
			throw new Exception("Directive Processor not found");
		}

		public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
		{
			if (directiveId == null)
				throw new ArgumentNullException("directiveId");
			if (processorName == null)
				throw new ArgumentNullException("processorName");
			if (parameterName == null)
				throw new ArgumentNullException("parameterName");

			return String.Empty;
		}

		public string ResolvePath(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (File.Exists(path))
				return path;

			string candidate = Path.Combine(Path.GetDirectoryName(TemplateFile), path);
			if (File.Exists(candidate))
				return candidate;

			return path;
		}

		public void SetFileExtension(string extension)
		{
			m_defaultFileExtension = extension;
		}

		public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
		{
			m_fileEncoding = encoding;
		}

		public IList<string> StandardAssemblyReferences
		{
			get
			{ 
				return new[]
				{
					// mscorlib
					typeof(Uri).Assembly.Location,
					// System.Core
					typeof(Enumerable).Assembly.Location,
					// Mono.TextTemplating
					typeof(Engine).Assembly.Location,
					// Mono.Cecil
					typeof(MethodDefinition).Assembly.Location,
					// GenerateWrappers (this assembly)
					typeof(WrapperTemplateHost).Assembly.Location,
					// Talon.Utility
					typeof(EnumerableUtility).Assembly.Location,
				};
			}
		}

		public IList<string> StandardImports
		{
			get { return new[] { "System", "GenerateWrappers", "Mono.Cecil", "System.Linq", "Talon.Utility", "System.Collections.Generic" }; }
		}

		public string OutputFilename
		{
			get { return CurrentClass.Name + m_defaultFileExtension; }
		}

		public string TemplateFile { get; internal set; }

		public CompilerErrorCollection TemplateErrors { get; private set; }

		public TypeDefinition CurrentClass { get; set; }

		private string m_defaultFileExtension;
		private Encoding m_fileEncoding;
	}
}