using System;
using System.IO;
using System.Threading.Tasks;

namespace DreamDonut.Abstractions
{
	public interface IConditionals
	{
		void SetSkipBackupAttrForIOS(string targetFilePath);
		bool IsDebug { get; }
		bool IsAndroid { get; }
		bool IsIOS { get; }
		bool IsWindowsPhone { get; }
		string OSName {get;}
		string OSVersion { get; }
		string DeviceModel { get; }
		string DeviceManufacturer { get; }
		int AndroidApi { get; }
		float UserFontSize {get; }

        Task<Stream> GetDownloadStream(string url);
    }
}

