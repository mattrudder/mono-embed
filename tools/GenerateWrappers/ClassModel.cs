
using System;
using System.Collections.Generic;

namespace GenerateWrappers
{
	public sealed class ClassModel
	{
		public string Name { get; set; }
		public string FullName { get; set; }

		public List<MethodModel> Methods
		{
			get { return m_listMethods; }
		}

		public ClassModel()
		{
			m_listMethods = new List<MethodModel>();
		}

		private readonly List<MethodModel> m_listMethods;
	}
}