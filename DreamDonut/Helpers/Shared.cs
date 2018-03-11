using System;
using System.Reflection;
using Stream = System.IO.Stream;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using PCLStorage;

using Xamarin.Forms;

using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services;

using DreamDonut.Abstractions;


namespace DreamDonut.Utilities
{

    //Recomended to refactor this class for better documentation

    /// <summary>
    /// Mischelaneous shared functionality and properties.
    /// </summary>
	public static class Shared
	{
        static Assembly assembly = typeof(Shared).GetTypeInfo().Assembly;

		private static readonly string EmbeddedResourcePrefix = "DOMA.";


		public static Stream GetSharedFileStreamForFile(string filePath)
		{
			//m'stream 
			Stream myStream = assembly.GetManifestResourceStream(filePath);
			if (myStream == null)
				myStream = GetEmbeddedResourceStream (filePath);

			return myStream;
		}

		public static bool CheckFolderExists(string folderPath)
		{
			return false;
		}


		public async static Task CopyRequiredAssetsToFolder(string folder)
		{
			await CopyRequiredAssetsToFolder(folder, null);
		}
		public async static Task CopyRequiredAssetsToFolder(string folder, Action onFileInstall)
		{
			await CopyAssetGoupToFolder("RequiredAssets", folder, onFileInstall);
		}

		public async static Task CopyStreamingAssetsToFolder(string folder) 
		{
			await CopyStreamingAssetsToFolder(folder, null);
		}
		public async static Task CopyStreamingAssetsToFolder(string folder, Action onFileInstall)
		{
			await CopyAssetGoupToFolder("StreamingAssets", folder, onFileInstall);
		}

		public async static Task CopyExpansionToFolder(string folder)
		{
			await CopyExpansionToFolder(folder, null);
		}
		public async static Task CopyExpansionToFolder(string folder, Action onFileInstall) 
		{
            await CopyAssetGoupToFolder ("ExpansionAssets", folder, onFileInstall);    
		}

        /// <summary>
        /// Copies a folder on the embedded manifest to a target location
        /// </summary>
        /// <param name="assetGroup">The dot notation path of the folder on the embedded resource manifest.</param>
        /// <param name="folder">The path of the folder where the resources should be copied to.</param>
		public async static Task CopyAssetGoupToFolder(string assetGroup, string folder)
		{
			await CopyAssetGoupToFolder(assetGroup, folder, null);
		}

        /// <summary>
        /// Copies a folder on the embedded manifest to a target location
        /// </summary>
        /// <param name="assetGroup">The dot notation path of the folder on the embedded resource manifest.</param>
        /// <param name="folder">The path of the folder where the resources should be copied to.</param>
        /// <param name="onFileInstall">An action to execute every time a file is succesfully copied.</param>
		public async static Task CopyAssetGoupToFolder(string assetGroup, string folder, Action onFileInstall) 
		{
			string streamingAssetTopFolder = assetGroup + ".";
			string rootEmbeddedPrefix = EmbeddedResourcePrefix + streamingAssetTopFolder;


			string[] names = assembly.GetManifestResourceNames ();
			foreach (string name in names) {
				if (!name.StartsWith (rootEmbeddedPrefix, StringComparison.CurrentCulture))
					continue;
				
				string cleanPath = name.Substring (rootEmbeddedPrefix.Length);

				string workingPart = cleanPath;

				string targetFolder = folder;
				string targetName = string.Empty;
				string targetExtension = string.Empty;
				string targetFile = string.Empty;

				int workingIndex = -1;

				workingIndex = workingPart.LastIndexOf(".", StringComparison.CurrentCulture);
				if (workingIndex >= 0)
				{
					targetExtension = workingPart.Substring(workingIndex);
					workingPart = workingPart.Substring(0, workingIndex);
				}

				workingIndex = workingPart.LastIndexOf(".", StringComparison.CurrentCulture);
				if (workingIndex >= 0)
				{
					targetName = workingPart.Substring(workingPart.LastIndexOf(".", StringComparison.CurrentCulture) + 1);
					workingPart = workingPart.Substring(0, workingPart.LastIndexOf(".", StringComparison.CurrentCulture));

					targetFolder = Path.Combine(folder, workingPart.Replace(".", "/"));
				}
				else {
					targetName = workingPart;

					targetFolder = folder;
				}

				targetFile = targetName + targetExtension;

				//IFolder iFolder = await FileSystem.Current.LocalStorage.CreateFolderAsync(targetFolder, CreationCollisionOption.OpenIfExists);
				//IFile iFile = await iFolder.CreateFileAsync(targetFile, CreationCollisionOption.ReplaceExisting);

				string filePath = targetFolder + "/" + targetFile;
				//if (IsAndroid && !filePath.StartsWith("file://", StringComparison.CurrentCulture))
				//	filePath = "file://" + filePath;

				string targetResourcePath = streamingAssetTopFolder + cleanPath;
				await CopyFileToLocation(targetResourcePath, filePath, true);

				if (onFileInstall != null)
					onFileInstall();

				//conditionals.SetSkipBackupAttrForIOS(filePath);

				Debug.WriteLine("created file: " + filePath);
			}

		}

        public static string FixFilePathForAndroid(string filePath) {

            //if (IsAndroid && !filePath.StartsWith("file://", StringComparison.CurrentCulture))
            //    filePath = "file://" + filePath;

            return filePath;
        }

        public static string RelativePathToFullPath(string relativePath) {

            if(string.IsNullOrEmpty(relativePath))
                return string.Empty;
            
            if(relativePath.ToLower().Trim().StartsWith("http", StringComparison.CurrentCulture))
                return relativePath;


            //if we accidentally send an absolute path, then no need to append again the root
            if(relativePath.Replace("file://", string.Empty).StartsWith(FileSystem.Current.LocalStorage.Path, StringComparison.CurrentCulture))
                return relativePath;

            return Path.Combine(
                FileSystem.Current.LocalStorage.Path, 
                CleanFilePathForIOCombine(relativePath)
            );
        }

        /// <summary>
        /// Gets the number of required asset files.
        /// </summary>
        /// <returns>The number of required asset files.</returns>
        public static int GetNumberOfRequiredAssetFiles()
        {
            int count = 0;
            string[] names = assembly.GetManifestResourceNames();
            foreach (string name in names)
            {
                if (!name.StartsWith(EmbeddedResourcePrefix + "RequiredAssets", StringComparison.CurrentCulture))
                    continue;
                count++;
            }
            return count;
        }

        /// <summary>
        /// Gets the number of installation files. These is the total of all Required Assets, Streaming Assets, and Expansion Assets.
        /// </summary>
        /// <returns>The number of installation files.</returns>
		public static int GetNumberOfInstallationFiles()
		{
			int count = 0;
			string[] names = assembly.GetManifestResourceNames();

			foreach (string name in names)
			{
				//we don't need to count the documents since those are only copied on demand and thus not part of the initial install
				if (!name.StartsWith(EmbeddedResourcePrefix + "RequiredAssets", StringComparison.CurrentCulture) &&
				   !name.StartsWith(EmbeddedResourcePrefix + "StreamingAssets", StringComparison.CurrentCulture) &&
				   !name.StartsWith(EmbeddedResourcePrefix + "ExpansionAssets", StringComparison.CurrentCulture))
					continue;
				count++;
			}


			return count;
		}

		/// <summary> 
		/// Attempts to find and return the given resource from within the specified assembly. 
		/// </summary> 
		/// <returns>The embedded resource stream.</returns> 
		/// <param name="resourceFileName">Resource file name.</param> 
		public static Stream GetEmbeddedResourceStream(string resourceFileName) 
		{ 
			string[] resourceNamesFiltered = new string[0];
			string[] resourceNames = assembly.GetManifestResourceNames(); 
		
			int filteredCount = 0;
			for(int i=0; i < resourceNames.Length; i++){
				var item = (string)resourceNames.GetValue (i);
				if(item.EndsWith(resourceFileName, StringComparison.CurrentCulture)){
					filteredCount++;
				}
			}

			if(filteredCount > 0){
				int index = 0;
				resourceNamesFiltered = new string[filteredCount];
				for(int i=0; i < resourceNames.Length; i++){
					var item = (string)resourceNames.GetValue (i);
					if(item.EndsWith(resourceFileName, StringComparison.CurrentCulture)){
						resourceNamesFiltered[index] = item;
						index++;
					}
				}
			}

			if (resourceNamesFiltered.Length <= 0) 
			{ 
				throw new Exception(string.Format("Resource ending with {0} not found.", resourceFileName)); 
			} 

			if (resourceNamesFiltered.Length > 1) 
			{ 
				throw new Exception(string.Format("Multiple resources ending with {0} found: {1}{2}", resourceFileName, Environment.NewLine, string.Join(Environment.NewLine, resourceNamesFiltered))); 
			} 

			return assembly.GetManifestResourceStream(resourceNamesFiltered[0]); 
		}


        /// <summary>
        /// Gets the embedded resource base64 byte string.
        /// </summary>
        /// <returns>The embedded resource base64 byte string.</returns>
        /// <param name="resourceFileName">Resource file name.</param>
		public static string GetEmbeddedResourceBase64ByteString(string resourceFileName) {
			try{
				Stream stream = GetEmbeddedResourceStream(resourceFileName);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					byte[] imageBytes = memoryStream.ToArray();

					string base64String = Convert.ToBase64String(imageBytes);
					return base64String;
				}
			} catch {
				return string.Empty;
			}
		}


        /// <summary>
        /// Converts a file to base64 string.
        /// </summary>
        /// <returns>The file contents in base64.</returns>
        /// <param name="filePath">The path of the file to convert.</param>
		public static async Task<string> ConvertFileToBase64String(string filePath)
		{
			if (string.IsNullOrEmpty(filePath)) return null;
			return await ConvertFileToBase64String(await FileSystem.Current.GetFileFromPathAsync(filePath));
		}

        /// <summary>
        /// Converts a file to base64 string.
        /// </summary>
        /// <returns>The file contents in base64.</returns>
        /// <param name="file">The file to convert.</param>
		public static async Task<string> ConvertFileToBase64String(IFile file)
		{
			if (file == null) return null;

			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					(await file.OpenAsync(FileAccess.Read)).CopyTo(memoryStream);
					byte[] imageBytes = memoryStream.ToArray();

					string base64String = Convert.ToBase64String(imageBytes);
					return base64String;
				}
			}
			catch { return null; }
		}

        /// <summary>
        /// Gets the rext content of the embedded resource file.
        /// </summary>
        /// <returns>The embedded resource text content.</returns>
        /// <param name="resourceFileName">Resource file name.</param>
		public static string GetEmbeddedResourceTextContent(string resourceFileName)
		{
			try{
				Stream stream = GetEmbeddedResourceStream(resourceFileName);
				using(StreamReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			} catch {
				return string.Empty;
			}
		}


        /// <summary>
        /// Copies an embedded file from the resource manifest to the root folder of the local storage
        /// </summary>
        /// <param name="strFileName">The file name on the resource manifest</param>.
        /// <param name="overwrite">If set to <c>true</c> overwrite.</param>
		public async static Task CopyEmbededFileToLocalResources(string strFileName)
		{
			await CopyEmbededFileToLocalResources (strFileName, false);
		}

        /// <summary>
        /// Copies an embedded file from the resource manifest to the root folder of the local storage
        /// </summary>
        /// <param name="strFileName">The file name on the resource manifest</param>.
        /// <param name="overwrite">If set to <c>true</c> overwrite.</param>
		public async static Task CopyEmbededFileToLocalResources(string strFileName, bool overwrite)
		{
			string strDestinationFilePath = Path.Combine (FileSystem.Current.LocalStorage.Path, strFileName);
			await CopyFileToLocation (strFileName, strDestinationFilePath, overwrite);
		}

        /// <summary>
        /// Copies an embedded file from the resource manifest to the given path
        /// </summary>
        /// <param name="strFileName">The file name on the resource manifest</param>.
        /// <param name="strDestinationFilePath">The full path of where the resource should be extracted to.</param>
        /// <param name="overwrite">If set to <c>true</c> overwrite.</param>
		public async static Task CopyFileToLocation(string strFileName, string strDestinationFilePath, bool overwrite)
		{
			await CopyFileToLocation (strFileName, string.Empty, strDestinationFilePath, overwrite);
		}

        /// <summary>
        /// Copies an embedded file from the resource manifest to the given path
        /// </summary>
        /// <param name="strFileName">The file name on the resource manifest</param>.
        /// <param name="strPointerFoler">the folder name on the resource manifest.</param>
        /// <param name="strDestinationFilePath">The full path of where the resource should be extracted to.</param>
        /// <param name="overwrite">If set to <c>true</c> overwrite.</param>
		public async static Task CopyFileToLocation(string strFileName, string strPointerFoler, string strDestinationFilePath, bool overwrite)
		{

			//if the file already exists, then don't bother
			IFile targetFile = await FileSystem.Current.GetFileFromPathAsync (strDestinationFilePath);
			if(targetFile != null && !overwrite)
				return;

			if (strDestinationFilePath.StartsWith(FileSystem.Current.LocalStorage.Path, StringComparison.CurrentCulture) && IsWindowsPhone)
            {
               strDestinationFilePath = strDestinationFilePath.Replace(FileSystem.Current.LocalStorage.Path, string.Empty);
            }

			//Android will fail file creation if the folder is not already present
			string strContaintingFolder = strDestinationFilePath.Substring(0, strDestinationFilePath.LastIndexOf("/", StringComparison.CurrentCulture));
			await FileSystem.Current.LocalStorage.CreateFolderAsync(strContaintingFolder, CreationCollisionOption.OpenIfExists);

			IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync(strDestinationFilePath, CreationCollisionOption.ReplaceExisting);
			try{
				using (StreamReader reader = new StreamReader(GetSharedFileStreamForFile(EmbeddedResourcePrefix + strPointerFoler + strFileName))) {
					Stream readStream = reader.BaseStream; 
					Stream writeStream = await file.OpenAsync (FileAccess.ReadAndWrite);
					ReadWriteStream(readStream, writeStream);
				}

				conditionals.SetSkipBackupAttrForIOS (strDestinationFilePath);
			} catch (Exception e){
				await file.DeleteAsync ();
				throw e;
			}
		}

        /// <summary>
        /// Copies a file to a destination folder
        /// </summary>
        /// <param name="file">The file that should be copied.</param>
        /// <param name="destinationFolder">The destination folder where the file should be copied.</param>
        /// <param name="cancellationToken">A Cancellation token.</param>
        public static async Task CopyFileTo(this IFile file, IFolder destinationFolder, CancellationToken cancellationToken = default(CancellationToken))
        {
            var destinationFile =
                await destinationFolder.CreateFileAsync(file.Name, CreationCollisionOption.ReplaceExisting, cancellationToken);

            using (var outFileStream = await destinationFile.OpenAsync(FileAccess.ReadAndWrite, cancellationToken))
            using (var sourceStream = await file.OpenAsync(FileAccess.Read, cancellationToken))
            {
                await sourceStream.CopyToAsync(outFileStream, 81920, cancellationToken);
            }
        }

        /// <summary>
        /// Writes a stream to another stream
        /// </summary>
        /// <param name="readStream">the stream you need to read.</param>
        /// <param name="writeStream">the stream you want to write to.</param>
		public static void ReadWriteStream(Stream readStream, Stream writeStream)
		{
			int Length = 256;
			Byte[] buffer = new Byte[Length];
			int bytesRead = readStream.Read(buffer, 0, Length);
			// write the required bytes
			while (bytesRead > 0)
			{
				writeStream.Write(buffer, 0, bytesRead);
				bytesRead = readStream.Read(buffer, 0, Length);
			}
			readStream.Dispose();
			writeStream.Dispose();
		}


        /// <summary>
        /// Writes a stream to another stream asynchronously
        /// </summary>
        /// <param name="readStream">the stream you need to read.</param>
        /// <param name="writeStream">the stream you want to write to.</param>
        /// <param name="onPercentageUpdate">Action to take on write percentage update.</param>
        public static async Task ReadWriteStreamAsync(Stream readStream, Stream writeStream, Action<int> onPercentageUpdate) {

            onPercentageUpdate?.Invoke(0);

            int Length = 65536;
            byte[] buffer = new byte[Length];

            long totalBytes = readStream.Length;
            long receivedBytes = 0;

            int i = 0;
            int updateFrequency = 2;
            for (;;)
            {
                int bytesRead;
                if(i % updateFrequency == 0) 
                    bytesRead= await readStream.ReadAsync(buffer, 0, buffer.Length);
                else
                    bytesRead= readStream.Read(buffer, 0, buffer.Length);
                
                if (bytesRead == 0)
                {
                    await Task.Yield();

                    break;
                }

                writeStream.Write(buffer, 0, bytesRead);
                receivedBytes += bytesRead;

                if(i++ % updateFrequency == 0) {
                    onPercentageUpdate?.Invoke((int)(((float)receivedBytes / totalBytes) * 100));
                }
            }

            onPercentageUpdate?.Invoke(100);

            readStream.Dispose();
            writeStream.Dispose();

        }

		static bool skipRoleFilteringForContent;
		public static bool SkipRoleFilteringForContent
		{
			get { return skipRoleFilteringForContent; }
			set{ skipRoleFilteringForContent = value; }
		}

        static string storagePermission = "storage";
        public static string StoragePermission {
            get { return storagePermission; }
        }

        static string calendarPermission = "calendar";
        public static string CalendarPermission {
            get { return calendarPermission; }
        }

        static int requiredPermissionRequestId = 0;
        public static int RequiredPermissionRequestId {
            get { return requiredPermissionRequestId; }
        } 

        static int optionalPermissionRequestId = 1;
        public static int OptionalPermissionRequestId {
            get { return optionalPermissionRequestId; }
        }

        //conditionals on a different project as PCL architecture makes # directives useless.
        public static IConditionals conditionals 	{ get; set; }
        public static bool 	 IsDebug			 	{ get { return conditionals.IsDebug; } }
		public static bool 	 IsAndroid			 	{ get { return conditionals.IsAndroid; } }
		public static bool 	 IsIOS			 		{ get { return conditionals.IsIOS; } }
		public static bool 	 IsWindowsPhone			{ get { return conditionals.IsWindowsPhone; } }
		public static string OSName 			 	{ get { return conditionals.OSName; } }
		public static string OSVersion 			 	{ get { return conditionals.OSVersion; } }
		public static string DeviceModel 		 	{ get { return conditionals.DeviceModel; } }
		public static string DeviceManufacturer  	{ get { return conditionals.DeviceManufacturer; } }
		public static int 	 AndroidApi 		 	{ get { return conditionals.AndroidApi; } }
		public static float  UserFontSize 		 	{ get { return conditionals.UserFontSize; } }

        public async static Task<Stream> GetDownloadStream(string url) {
            return await conditionals.GetDownloadStream(url);
        }

		public static string AppName { 
			get {

				var nativeOperations = Resolver.Resolve<INativeOperations> ();
				return nativeOperations.AppName;
			}
		}

		public static string AppVersion { 
			get {

				var nativeOperations = Resolver.Resolve<INativeOperations> ();
				return nativeOperations.AppVersion;
			}
		}
				



        /// <summary>
        /// Gets a list of integers split from a string containing a list of ranges and pages
        /// e.g. This will split something like 1,2,3-5,9-11,32 to [1,2,3,4,5,9,10,11,32]
        /// </summary>
        /// <returns>The array of pages.</returns>
        /// <param name="pages">The string containing a range of pages.</param>
		public static List<int> GetArrayOfPages(string pages)
		{
			pages = pages.Trim ().Replace(" ", "");
			List<int> arrayOfPages = new List<int>();
			string[] strArrPages = pages.Split(new char[]{',', '|'}, StringSplitOptions.RemoveEmptyEntries);
			for (var i = 0; i < strArrPages.Length; i++) {
				var currentPage = strArrPages[i];
				if(currentPage.IndexOf("-", StringComparison.CurrentCulture) >= 0){
					string[] pageRange = currentPage.Split(new char[]{'-'}, StringSplitOptions.RemoveEmptyEntries);
					if(pageRange.Length == 1){
						arrayOfPages.Add(Convert.ToInt32(pageRange[0]));
						continue;
					}

					for(int j = Convert.ToInt32(pageRange[0]); j <= Convert.ToInt32(pageRange[1]); j++){
						arrayOfPages.Add(j);
					}
				}
				else{
					arrayOfPages.Add(Convert.ToInt32(currentPage));
				}
			}
			return arrayOfPages;
		}

        /// <summary>
        /// Returns all files in a folder that have the given extension
        /// </summary>
        /// <returns>The all file infos in folder by extension.</returns>
        /// <param name="folder">The folder path of where to look for the files.</param>
        /// <param name="extension">The extension of the files to search.</param>
		public async static Task<IList<IFile>> GetAllFileInfosInFolderByExtension (string folder, string extension){
			if (string.IsNullOrEmpty (folder))
				return null;

			IFolder directory = await FileSystem.Current.GetFolderFromPathAsync(folder);
			List<IFile> files = new List<IFile> (await directory.GetFilesAsync ());

			if (!string.IsNullOrEmpty (extension)) {
				for (int i = 0; i < files.Count; i++) {
					IFile currFile = files[i];
					if (!currFile.Path.EndsWith (extension, StringComparison.CurrentCulture)) {
						files.Remove (currFile);
					}
				}
			}

			return files;
		}

        /// <summary>
        /// Strips a string from any non alphanumeric values
        /// </summary>
        /// <returns>The clean phone link.</returns>
        /// <param name="rawPhoneNumber">Raw phone number string.</param>
		public static string GetCleanPhoneLink(string rawPhoneNumber){

			string phoneNumber = "";

			for (int i = 0; i < rawPhoneNumber.Length; i++) {
				char currChar = rawPhoneNumber [i];
				if (currChar >= '0' && currChar <= '9') {
					phoneNumber += currChar;
				}
			}

            return phoneNumber;
		}


        public static bool InterfaceIsTablet => Device.Idiom == TargetIdiom.Tablet;


        public static bool InterfaceIsPhone => Device.Idiom == TargetIdiom.Phone;

		public static string DeviceType
		{
			get
			{
				return InterfaceIsTablet ? "Tablet" : "Phone";
			}
		}

		public static string UrlDecode(string urlEncodedString) {
			if (string.IsNullOrEmpty (urlEncodedString))
				return string.Empty;
			
			return Uri.UnescapeDataString(urlEncodedString).Replace('+', ' ');
		}

		public static string UrlEncode(string decodedString)
		{
			if (string.IsNullOrEmpty(decodedString))
				return string.Empty;

			return Uri.EscapeDataString(decodedString);
		}


		//TODO: PCL implementation
		public static string HtmlDecode(string htmlEncodedString) {
			return htmlEncodedString;
		}

        /// <summary>
        /// Shuffle the specified list.
        /// </summary>
        /// <returns>The shuffled list.</returns>
        /// <param name="list">The list to be shuffled.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> Shuffle<T>(List<T> list)
		{
			Random rng = new Random();
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list;
		}

		public static string MapIntToUpperLetter(int i)
		{
			int targetUnicode = i + 65;
			char unicodeChar = (char)targetUnicode;
			return unicodeChar.ToString();
		}

		public static string CleanFilePathForIOCombine(string path)
		{
			return RemoveLeadingSlash(RemoveLeadingDot(path));
		}

		public static string RemoveLeadingDot(string path)
		{
			return RemoveLeadingChar(path, ".");
		}

		public static string RemoveLeadingSlash(string path)
		{
			return RemoveLeadingChar(path, "/");
		}

		public static string RemoveLeadingChar(string path, string charStr)
		{
			if (string.IsNullOrEmpty(path))
				return string.Empty;

			if (charStr.Length > 1) return path;

			if (path.StartsWith(charStr, StringComparison.CurrentCulture))
				return path.Substring(1);

			return path;
		}

        /// <summary>
        /// Determines whether the user has a valid internet connection available.
        /// </summary>
        /// <returns><c>true</c>, if internet conection was valided, <c>false</c> otherwise.</returns>
		public static bool ValidInternetConectionExists()
		{
			var device = Resolver.Resolve<IDevice>();
			NetworkStatus networkStatus = device.Network.InternetConnectionStatus();
			return networkStatus !=
                NetworkStatus.NotReachable;
		}

        /// <summary>
        /// Determines whether the user is on wifi or not
        /// </summary>
        /// <returns><c>true</c>, if internet conection is on wifi, <c>false</c> otherwise.</returns>
        public static bool ValidWifiConectionExists()
        {
            var device = Resolver.Resolve<IDevice>();
            NetworkStatus networkStatus = device.Network.InternetConnectionStatus();
            return networkStatus == NetworkStatus.ReachableViaWiFiNetwork;
        }

        /// <summary>
        /// Capitalizes the first letter of every word in the given string
        /// </summary>
        /// <returns>The title cased string.</returns>
        /// <param name="str">String to title case.</param>
        public static string ToTitleCase(string str) {
            if(string.IsNullOrEmpty(str))
                return string.Empty;

            IEnumerable<char> CharsToTitleCase (string s)
            {
                bool newWord = true;
                foreach(char c in s)
                {
                    if(newWord) { yield return Char.ToUpper(c); newWord = false; }
                    else yield return Char.ToLower(c);
                    if(c==' ') newWord = true;
                }
            }

            return new string( CharsToTitleCase(str).ToArray() );
        }

	}
}

