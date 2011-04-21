using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalonScript
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class NativeAttribute : Attribute
	{
	}
}
