
#if __IOS__
using Foundation;
using UIKit;
#endif

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using DreamDonut.Abstractions;

namespace DreamDonut
{
	public class Conditionals : IConditionals
	{

        public async Task<Stream> GetDownloadStream(string url) {
            WebClient client = new WebClient();
            return await client.OpenReadTaskAsync(url);
        }

		public void SetSkipBackupAttrForIOS(string targetFilePath) {
			#if __IOS__
			NSFileManager.SetSkipBackupAttribute (targetFilePath, true);
			#endif
		}
			
		public float UserFontSize
		{
			get {

				#if __ANDROID__

				//android font sizes scale with the system font size setting
				//so we don't have to modify this at all here therefore return 100%
				return 100;
				#elif __IOS__

				float rawFloat = NSUserDefaults.StandardUserDefaults.FloatForKey ("font_size");

				//turns a [0,1] range into a [80,120] range
				float percentageValue = (rawFloat * 40) + 80;

				return percentageValue;
                #else
                                return 0;
                #endif
            }
		}

		public bool IsDebug {
			get { 

				#if DEBUG
				return true;
				#else
				return false;
				#endif
			}
		}

		public bool IsAndroid
		{
			get
			{ 

				#if __ANDROID__
				return true;
				#else
				return false;
				#endif
			}
		}

		public bool IsIOS {
			get { 
				#if __IOS__
				return true;
				#else
				return false;
				#endif
			}
		}

		public bool IsWindowsPhone{
			get { 
				#if SILVERLIGHT
				return true;
				#elif WINDOWS_PHONE
				return true;
				#else
				return false;
				#endif
			}
		}

		public string OSName
		{
			get{

				#if __ANDROID__
				return "Android";
				#elif __IOS__
				return "iOS";
				# else 
				return "(unknown)";
				#endif
			}
		}


		public string OSVersion
		{
			get{

				#if __ANDROID__
				return Android.OS.Build.VERSION.Release;
				#elif __IOS__
				return UIDevice.CurrentDevice.SystemVersion;
				# else 
				return "(unknown)";
				#endif
			}
		}

		public string DeviceModel
		{
			get{
				#if __ANDROID__
				return Android.OS.Build.Model;
				#elif __IOS__
				return UIDevice.CurrentDevice.Model;
				# else 
				return "(unknown)";
				#endif
			}
		}

		public string DeviceManufacturer
		{
			get{
				#if __ANDROID__
				return Android.OS.Build.Manufacturer;
				#elif __IOS__
				return "Apple";
				# else 
				return "(unknown)";
				#endif
			}
		}

		public int AndroidApi 
		{
			get 
			{
				int ApiLevel = 0;
				#if __ANDROID__
				ApiLevel = (int)Android.OS.Build.VERSION.SdkInt;
				#endif
				return ApiLevel;
			}
		}
	}
}

