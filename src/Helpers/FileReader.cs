namespace OpenCivOne
{
	public static class FileReader
	{
		public static StreamReader GetStreamReader(string path)
		{
			string dir = Path.GetDirectoryName(path)!;
			string file = Path.GetFileName(path);

			string? actualFile = Directory
				.GetFiles(dir)
				.FirstOrDefault(f =>
					string.Equals(
						Path.GetFileName(f),
						file,
						StringComparison.OrdinalIgnoreCase));

			return actualFile != null
				? new StreamReader(actualFile)
				: throw new FileNotFoundException(actualFile + " not found!");
		}

		public static FileStream GetFileStream(string path, FileMode fileMode, FileAccess fileAccess = FileAccess.Read)
		{
			string dir = Path.GetDirectoryName(path)!;
			string file = Path.GetFileName(path);

			string? actualFile = Directory
				.GetFiles(dir)
				.FirstOrDefault(f =>
					string.Equals(
						Path.GetFileName(f),
						file,
						StringComparison.OrdinalIgnoreCase));

			return actualFile != null
				? new FileStream(actualFile, fileMode, fileAccess)
				: throw new FileNotFoundException(actualFile + " not found!");
		}
	}
}