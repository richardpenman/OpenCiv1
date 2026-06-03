using System;

namespace OpenCivOne
{
	public class ResourceMissingException : Exception
	{
		public ResourceMissingException() : base() { }

		public ResourceMissingException(string message) : base(message) { }

		public ResourceMissingException(string message, Exception innerException) : base(message, innerException) { }
	}
}
