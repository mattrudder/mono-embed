using System;

namespace GenerateWrappers
{
	public sealed class TypeConversionInfo
	{
		public TypeConversionInfo(string nativeType)
			: this(nativeType, null)
		{
		}

		public TypeConversionInfo(string nativeType, string thunkType)
		{
			if (nativeType == null)
				throw new ArgumentNullException("nativeType");
			if (nativeType.Length == 0)
				throw new ArgumentException("The native type must be specified.");

			NativeType = nativeType;
			ThunkType = thunkType;
		}

		public string NativeType { get; private set; }
		public string ThunkType { get; private set; }
	}
}