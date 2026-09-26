using System.IO;

namespace Echo.Editor.Utils
{
	/// <summary>
	/// 文件系统工具
	/// </summary>
	public static class FileUtility
	{
		/// <summary>
		/// 判断文件夹是否存在
		/// </summary>
		public static bool DirectoryExists(string directoryPath)
		{
			if (string.IsNullOrWhiteSpace(directoryPath))
				return false;

			return Directory.Exists(directoryPath);
		}

		/// <summary>
		/// 判断文件是否存在
		/// </summary>
		public static bool FileExists(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				return false;

			return File.Exists(filePath);
		}

		/// <summary>
		/// 创建文件夹
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <returns></returns>
		public static bool CreateDirectory(string directoryPath)
		{
			if (string.IsNullOrWhiteSpace(directoryPath))
				return false;

			Directory.CreateDirectory(directoryPath);
			return Directory.Exists(directoryPath);
		}

		/// <summary>
		/// 标准化文件路径
		/// </summary>
		public static string NormalizePath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				return string.Empty;

			return path.Replace("\\", "/");
		}
	}
}
