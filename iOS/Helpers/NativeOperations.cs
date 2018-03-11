using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;


using Foundation;
using UIKit;
using MessageUI;
using CoreGraphics;
using Security;
using StoreKit;


using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using PCLStorage;

using Newtonsoft.Json.Linq;

using DreamDonut.Abstractions;


[assembly: Dependency(typeof(DreamDonut.iOS.NativeOperations))]
namespace DreamDonut.iOS
{
    public class NativeOperations : INativeOperations
	{
        public NativeOperations() {}

		public void OpenUrlExternally(string url) {
			UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
		}

		public bool NetworkActivityIndicator { 
			set{
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
			}
		}

		public string GetAppStoreId() {

			string appIdRequestUrl = "http://itunes.apple.com/lookup?bundleId=" + NSBundle.MainBundle.BundleIdentifier;

			var request = System.Net.HttpWebRequest.Create(appIdRequestUrl);

			request.ContentType = "application/json";
			request.Method = "GET";

			using (System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse)
			{
				if (response.StatusCode != System.Net.HttpStatusCode.OK)
					Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					var content = reader.ReadToEnd();
					if(string.IsNullOrWhiteSpace(content)) {
						Console.Out.WriteLine("Response contained empty body...");
					}
					else {
						Console.Out.WriteLine("Response Body: \r\n {0}", content);

						JObject json = JObject.Parse (content);
						JToken trackId = json.SelectToken("results[0].trackId");
						if (trackId == null) {
							return null;
						}
						string id = trackId.ToString();

						return id;
					}
				}
			}

			return null;
		}

		public string GetAppStoreUrl(string appStoreId) {
            return $"itms-apps://itunes.apple.com/app/id{appStoreId}?action=write-review";
		}

		public void CallPhoneNumber(string phoneNumber) {

            if(!phoneNumber.ToLower().StartsWith("tel:", StringComparison.CurrentCulture))
                phoneNumber = "tel:" + phoneNumber;
            
			UIApplication.SharedApplication.OpenUrl (new NSUrl (phoneNumber));
		}

		public string ExternalStoragePath {
			get {
				return Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			}
		}

		public string AppName {
			get {
				try{
					return NSBundle.MainBundle.InfoDictionary ["CFBundleDisplayName"].ToString ();
				} catch {
					return NSBundle.MainBundle.InfoDictionary ["CFBundleName"].ToString ();
				}
			}
		}

		public string AppVersion {
			get {
				return NSBundle.MainBundle.InfoDictionary ["CFBundleShortVersionString"].ToString ();
			}
		}

		public string AppPackageId
		{
			get
			{
				return  NSBundle.MainBundle.BundleIdentifier;
			}
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

			if (MFMailComposeViewController.CanSendMail)
			{
				MFMailComposeViewController mailController = new MFMailComposeViewController();

                if(recipients != null && recipients.Length > 0)
                    mailController.SetToRecipients(recipients);
                if(!string.IsNullOrEmpty(mailSubject))
                    mailController.SetSubject(mailSubject);
                if(!string.IsNullOrEmpty(mailBody))
				    mailController.SetMessageBody(mailBody, false);

                if(!string.IsNullOrEmpty(attachmentFilePath)) {
                    IFile attachmentFile = await FileSystem.Current.GetFileFromPathAsync (attachmentFilePath);
                    if(attachmentFile != null) {
                        NSData attachmentData = NSData.FromFile (attachmentFilePath);
                        string mimeType = attachmentType;
                        mailController.AddAttachmentData(attachmentData, mimeType, attachmentFile.Name);
                    }
                }

				mailController.Finished += (object s, MFComposeResultEventArgs args) =>
				{
					Console.WriteLine(args.Result.ToString());
					args.Controller.DismissViewController(true, null);

					if (args.Result == MFMailComposeResult.Sent)
                        onSuccesfulSend?.Invoke();
					else if (args.Result == MFMailComposeResult.Failed)
                        onFailedSend?.Invoke();
					else 
                        onSendCancel?.Invoke();
				};
				UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(mailController, true, null);
			}
			else {
                onFailedSend?.Invoke();
				throw new Exception("Error sending mail: No mail client has been configured on this device.");
			}
		}


		public async Task<bool> SaveUrlDataToFile(string url, string destinationPath) {

			NSUrl nsUrl = new NSUrl (url);
			string fileName = nsUrl.LastPathComponent;

			//load the data 
			NSData pdfData = NSData.FromUrl (nsUrl);
			if (pdfData == null) {
				Console.WriteLine ("failed to fetch data for " + nsUrl.AbsoluteString + ". data came back null.");
				return false;
			}

			//save the data
			NSError dataError;
			pdfData.Save (destinationPath, true, out dataError);
			if (dataError != null) {
				Console.WriteLine ("error when saving data for " + nsUrl.AbsoluteString + ". " + dataError.ToString ());
				return false;
			} else {
				Console.WriteLine ("Data saved succesfully for " + nsUrl.AbsoluteString + " to " + destinationPath);
				//flag the data
				NSFileManager.SetSkipBackupAttribute (destinationPath, true);
				Console.WriteLine ("Setting skip backup attribute for file: " + destinationPath);
				return true;
			}
		}

        public void CheckAndCreateExternaDirectory ()
        {
            throw new NotImplementedException ();
        }

		public void OpenFileExternally(string path, string mimeType) {}
		public void CopyExpansionToFolder(string folder, Action onFileInstall) {}
		public void LoadAppRatings() {
            SKStoreReviewController.RequestReview ();
        }
		public void DoNativeBackButton() {}


		public int GetNumberOfInstallationFilesOnExpansion()
		{
			return 0;
		}

		//TODO:maybe
		public bool PackageIsInstalledOnDevice(string packageName)
		{
			return false;
		}

		public byte[] ResizeImage(byte[] imageData, float width, float height, int quality)
		{
			UIImage originalImage = ImageFromByteArray(imageData);


			float oldWidth = (float)originalImage.Size.Width;
			float oldHeight = (float)originalImage.Size.Height;
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

			//create a 24bit RGB image
			using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
				(int)newWidth, (int)newHeight, 8,
				0, CGColorSpace.CreateDeviceRGB(),
				CGImageAlphaInfo.PremultipliedFirst))
			{

				RectangleF imageRect = new RectangleF(0, 0, newWidth, newHeight);

				// draw the image
				context.DrawImage(imageRect, originalImage.CGImage);

				UIImage resizedImage = UIImage.FromImage(context.ToImage());

				// save the image as a jpeg
				return resizedImage.AsJPEG((float)quality).ToArray();
			}
		}

		public static UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null)
			{
				return null;
			}

			UIImage image;
			try
			{
				image = new UIImage(NSData.FromArray(data));
			}
			catch (Exception e)
			{
				Console.WriteLine("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}


		public void KillApp()
		{
			UIApplication.SharedApplication.PerformSelector(new ObjCRuntime.Selector("terminateWithSuccess"), null, 0f);
		}


		public void InstallCertificate(string certificate)
		{

			if (certificate.StartsWith("http", StringComparison.CurrentCulture))
			{
				OpenUrlExternally(certificate);
			}

			//THIS CASE DOESN"T WORK YET, WE CAN ONLY ADD THE CERT TO THE KEYCHAIN BUT NOT PROMPT FOR TRUST
			else {
				NSData certData = NSData.FromUrl(new NSUrl(certificate));

				SecCertificate secCertificate = new SecCertificate(certData);

				SecRecord secRecord = new SecRecord(SecKind.Certificate);

				secRecord.SetValueRef(secCertificate);


				SecPolicy policy = SecPolicy.CreateSslPolicy(true, "applocker.navy.mil");
				SecTrust secTrust = new SecTrust(secCertificate, policy);
				//SecTrustResult results = secTrust.GetTrustResult();

				SecStatusCode code = SecKeyChain.Add(secRecord);
				Console.WriteLine(code);
			}
		}


		public void HideKeyboard()
		{
        	UIApplication.SharedApplication.KeyWindow.EndEditing(true);
		}

        public long GetAvailableStorageSpace() {
            return (long)NSFileManager.DefaultManager.GetFileSystemAttributes (FileSystem.Current.LocalStorage.Path).FreeSize;
        }

        public long GetAvailableExternalStorageSpace() {
            return 0;
        }

        public bool ExternalStorageAvailable { get { return false; } }

    }
}

