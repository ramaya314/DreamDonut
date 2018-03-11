using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using PCLStorage;

namespace DreamDonut.Abstractions
{
	public interface INativeOperations
	{
		void OpenUrlExternally(string url);
		bool NetworkActivityIndicator { set;}
		string GetAppStoreId();
		string GetAppStoreUrl(string appStoreId);
		void CallPhoneNumber(string phoneNumber);
		string ExternalStoragePath { get; }

		string AppName {get;}
		string AppVersion {get; }
		string AppPackageId {get; }


		void OpenFileExternally(string path, string mimeType);

	
        Task ComposeEmail(string recipient);
        Task ComposeEmail(string recipient, Action onSuccesfulSend, Action onFailedSend, Action onSendCancel);
        Task ComposeEmail(string[] recipients, Action onSuccesfulSend, Action onFailedSend, Action onSendCancel);
		Task ComposeEmail(string[] recipients, string mailSubject, string mailBody, string attachmentFilePath, string attachmentType, Action onSuccesfulSend, Action onFailedSend, Action onSendCancel);
	
		Task<bool> SaveUrlDataToFile(string url, string destinationPath);


        void CheckAndCreateExternaDirectory ();

		void LoadAppRatings ();

		void DoNativeBackButton ();


		bool PackageIsInstalledOnDevice(string packageName);

		byte[] ResizeImage(byte[] imageData, float width, float height, int quality);


		void KillApp();

		void HideKeyboard();

        long GetAvailableStorageSpace();
        long GetAvailableExternalStorageSpace();

        bool ExternalStorageAvailable { get; }

	}

}

