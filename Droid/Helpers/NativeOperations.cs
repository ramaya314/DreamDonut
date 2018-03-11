using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Android.Content;
using Android.Widget;
using Android.Support.V4.Content;
using Android.OS;
using Android.Graphics;
using Android.Security;
using Android.Util;
using Android.Net.Http;
using Android.App;
using Android.Views.InputMethods;

using Java.Security;
using Java.Security.Cert;
using Java.Net;



using Xamarin.Forms;
using PCLStorage;


using Newtonsoft.Json.Linq;

using DreamDonut.Abstractions;


//all the operations here should be independent from the navigation scheme of the platform
[assembly: Dependency(typeof(DreamDonut.Droid.NativeOperations))]
namespace DreamDonut.Droid
{
	public class NativeOperations : INativeOperations
	{
		public void OpenUrlExternally(string url) {

			Android.Net.Uri uri;

			//might need to check if url.StartsWith("http") to mark it as internalUrl
			if (url.StartsWith("http", StringComparison.CurrentCulture)) {
				uri = Android.Net.Uri.Parse (url);
			} else {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N) {
                    uri = FileProvider.GetUriForFile (MainActivity.Instance.ApplicationContext, MainActivity.Instance.PackageName + ".fileprovider", new Java.IO.File (url));
                } else {
                    uri = Android.Net.Uri.FromFile (new Java.IO.File (url));
                }				
			}

			Intent intent = new Intent(Intent.ActionView, uri);
            intent.SetFlags (ActivityFlags.GrantReadUriPermission);

			try {
				MainActivity.Instance.StartActivity(intent);
			} catch (ActivityNotFoundException) {
				Toast.MakeText(MainActivity.Instance, "Error opening web page: No web client has been configured on this device.", ToastLength.Short).Show();
			}
		}


		public void OpenFileExternally(string path, string mimeType) {

            Java.IO.File file = new Java.IO.File (path);
            file.SetReadable (true, false);
            Android.Net.Uri fileUri;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N) {
                fileUri = FileProvider.GetUriForFile (MainActivity.Instance.ApplicationContext, MainActivity.Instance.PackageName + ".fileprovider", file);
            } else {
                fileUri = Android.Net.Uri.FromFile (file);
            }

            Intent intent = new Intent (Intent.ActionView);
            intent.SetDataAndType (fileUri, mimeType);
            intent.SetFlags (ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);

            try {
                MainActivity.Instance.ApplicationContext.StartActivity(intent);
            } 
            catch (ActivityNotFoundException){
                string fileType = mimeType.Substring (mimeType.IndexOf ("/", StringComparison.CurrentCulture) + 1);
                Toast.MakeText(MainActivity.Instance, "Error opening " + fileType + " file: No " + fileType + " client has been configured on this device.", ToastLength.Long).Show();
            }
		}

		//in iOS this is equivalent to the small loading icon shown on the status bar
		public bool NetworkActivityIndicator {
			set { }
		}

		public string GetAppStoreId() {
			return MainActivity.Instance.PackageName;
		}
		
		public string GetAppStoreUrl(string appStoreId) {
			return "market://details?id=" + appStoreId;
		}

		public void CallPhoneNumber(string phoneNumber) {
            if(!phoneNumber.StartsWith("tel:", StringComparison.CurrentCulture))
                phoneNumber = "tel:" + phoneNumber;
            
			var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(phoneNumber)); 
			MainActivity.Instance.StartActivity(intent);
		}

		public string ExternalStoragePath
		{
			get {
				return Android.OS.Environment.ExternalStorageDirectory.Path + Java.IO.File.Separator + MainActivity.Instance.ApplicationContext.PackageName; 
			}
		}

		public string AppName {
			get {
                return MainActivity.Instance.PackageManager.GetApplicationLabel (MainActivity.Instance.ApplicationInfo);
			}
		}

		public string AppVersion {
			get {
                return MainActivity.Instance.PackageManager.GetPackageInfo (MainActivity.Instance.PackageName, 0).VersionName;
			}
		}

		public string AppPackageId
		{
			get
			{
				return MainActivity.Instance.ApplicationContext.PackageName; 
			}
		}

		public void ShareFileExternally(VisualElement container, string path, ToolbarItem toolbarItem){
			//TODO: maybe implement this.
			//on the legacy code this is only present on iOS since android will always open a pdf on an external app anyway
			//we might not even need to implement this at all for Android.
			//the toolbar item should be the calling button. on iOS this is the button where the little submenu attaches to
		}



        public async Task ComposeEmail(string recipient)
        {
            await ComposeEmail(recipient, null, null, null);
        }
        public async Task ComposeEmail(string recipient, Action onSuccesfulSend, Action onFailedSend, Action onSendCancel)
        {
            await ComposeEmail(new string[] {recipient}, onSuccesfulSend, onFailedSend, onSendCancel);
        }
        public async Task ComposeEmail(string[] recipients, Action onSuccesfulSend, Action onFailedSend, Action onSendCancel)
        {
            await ComposeEmail(recipients, string.Empty, string.Empty, string.Empty, string.Empty, onSuccesfulSend, onFailedSend, onSendCancel);
        }
		public async Task ComposeEmail(string[] recipients, string mailSubject, string mailBody, string attachmentFilePath, string attachmentType, Action onSuccesfulSend, Action onFailedSend, Action onSendCancel)
		{


			var email = new Intent(Intent.ActionSend);
			email.SetType("message/rfc822");
            email.SetFlags (ActivityFlags.GrantReadUriPermission);


            if(recipients != null && recipients.Length > 0)
                email.PutExtra(Intent.ExtraEmail, recipients);
            if(!string.IsNullOrEmpty(mailSubject))
                email.PutExtra(Intent.ExtraSubject, mailSubject);
            if(!string.IsNullOrEmpty(mailBody))
			    email.PutExtra (Intent.ExtraText, mailBody);

            if(!string.IsNullOrEmpty(attachmentFilePath)) {
                var attachmentFile = new Java.IO.File(attachmentFilePath);
                attachmentFile.SetReadable(true, false);
                Android.Net.Uri attachmentUri;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N) {
                    attachmentUri = FileProvider.GetUriForFile (MainActivity.Instance.ApplicationContext, MainActivity.Instance.PackageName + ".fileprovider", attachmentFile);
                } else {
                    attachmentUri = Android.Net.Uri.FromFile (attachmentFile);
                }
                email.PutExtra(Intent.ExtraStream, attachmentUri);
            }

			try 
			{
				var activities = MainActivity.Instance.PackageManager.QueryIntentActivities(email, Android.Content.PM.PackageInfoFlags.Activities);
				if (activities.Count >= 1)
				{
					MainActivity.Instance.StartActivity(Intent.CreateChooser(email, "Choose an Email client :"));
                    onSuccesfulSend?.Invoke();
				}
				else 
				{ 
                    onFailedSend?.Invoke();
					Toast.MakeText(MainActivity.Instance, "Error sending mail: No mail client has been configured on this device.", ToastLength.Long).Show();
				}
			}
			catch(Exception e) {
                onFailedSend?.Invoke();
				Toast.MakeText(MainActivity.Instance, "Error sending mail: " + e, ToastLength.Long).Show();
			}              
		}


		public async Task<bool> SaveUrlDataToFile(string url, string destinationPath) {

			try{
				Uri baseUri = new Uri (url);
				System.Net.Http.HttpClientHandler clientHandler = new System.Net.Http.HttpClientHandler ();
				System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient (clientHandler);
				httpClient.BaseAddress = baseUri;
				byte[] imageBytes = await httpClient.GetByteArrayAsync(baseUri);
				System.IO.File.WriteAllBytes (destinationPath, imageBytes);

				Console.WriteLine("Data saved succesfully for " + url + " to " + destinationPath);
				return true;
			} catch(Exception e) {
				throw new Exception("error when saving data for " + url + ". " + e);
				//return false;
			}

		}


        public void CheckAndCreateExternaDirectory()
        {
            Java.IO.File publicDir = new Java.IO.File (Android.OS.Environment.ExternalStorageDirectory.Path + Java.IO.File.Separator + MainActivity.Instance.ApplicationContext.PackageName);
            if (!publicDir.Exists ()) {
                publicDir.Mkdirs ();
            }
        }

		public void LoadAppRatings()
		{
			var uri = Android.Net.Uri.Parse("market://details?id=" + MainActivity.Instance.PackageName);
			Intent goToMarket = new Intent(Intent.ActionView, uri);
			try
			{
				MainActivity.Instance.StartActivity(goToMarket);
			}
			catch (ActivityNotFoundException)
			{
				MainActivity.Instance.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=" + MainActivity.Instance.PackageName)));
			}
		}

		public void DoNativeBackButton()
		{
			MainActivity.Instance.Finish();
		}

		public bool PackageIsInstalledOnDevice(string packageName)
		{
			Android.Content.PM.PackageManager pm = MainActivity.Instance.ApplicationContext.PackageManager;
			try
			{
				pm.GetPackageInfo(packageName, Android.Content.PM.PackageInfoFlags.Activities);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public byte[] ResizeImage(byte[] imageData, float width, float height, int quality)
		{
			// Load the bitmap
			Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

			float oldWidth = (float)originalImage.Width;
			float oldHeight = (float)originalImage.Height;
			float scaleFactor = 0f;

			if (oldWidth > oldHeight)
			{
				scaleFactor = width / oldWidth;
			}
			else
			{
				scaleFactor = height / oldHeight;
			}

			float newHeight = oldHeight * scaleFactor;
			float newWidth = oldWidth * scaleFactor;

			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
				return ms.ToArray();
			}
		}


		public void KillApp()
		{
			Process.KillProcess(Process.MyPid());
		}


		public void HideKeyboard()
		{
            var inputMethodManager = MainActivity.Instance.ApplicationContext.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && MainActivity.Instance.ApplicationContext is Activity)
			{
                var token = (MainActivity.Instance.ApplicationContext as Activity)?.CurrentFocus.WindowToken;
				inputMethodManager.HideSoftInputFromWindow(token, 0);
			}
		}


        public long GetAvailableStorageSpace() {
            string root = FileSystem.Current.LocalStorage.Path;
            StatFs statFs = new StatFs(root);
            long   free   = statFs.AvailableBlocksLong * statFs.BlockSizeLong;
            return free;
        }


        public long GetAvailableExternalStorageSpace() {
            StatFs statFs = new StatFs(ExternalStoragePath);
            long   free   = statFs.AvailableBlocksLong * statFs.BlockSizeLong;
            return free;
        }


        public bool ExternalStorageAvailable { 
            get { 
                return Android.OS.Environment.ExternalStorageState == Android.OS.Environment.MediaMounted
                              && Android.OS.Environment.IsExternalStorageRemovable;
            } 
        }

    }
}

