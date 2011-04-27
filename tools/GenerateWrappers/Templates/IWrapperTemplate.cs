using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenerateWrappers.Templates
{
	internal interface IWrapperTemplate
	{
		void SetClass(ClassModel klass);
		string TransformText();
	}
}
