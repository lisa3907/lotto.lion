using Firebase.Core;
using Foundation;
using Google.MobileAds;
using Lion.XiOS.Libs;
using Lion.XIOS.Type;
using System.Text;
using UIKit;
using UserNotifications;

namespace Lion.XiOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        public string UserToken { get; set; }

        public string GuestToken { get; set; }

        public int RecentSeqNum { get; set; }

        public string RecentSeqDate { get; set; }

        public UserInfo UserInfo { get; set; }

        public bool Edited { get; set; }

        public bool Logined { get; set; }

        public Networking NetworkInstance { get; set; }

        public string UserID { get; set; }

        public string UserPW { get; set; }

        public string APNSKey { get; set; }

        public AppDelegate() : base()
        {
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            this.UserSetting();

            this.NetworkInstance = new Networking();
            this.RegisterForRemoteNotifications();

            App.Configure();
            //MobileAds.Configure("ca-app-pub-9319956348219153~7624616801");
            MobileAds.SharedInstance.Start(completionHandler: null);

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.

            //this.Logined = false;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.

            //this.Logined = false;
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            this.GuestToken = this.NetworkInstance.GetGuestToken().result;
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            NSLogHelper.NSLog($"Failed to get token, error: {error}");
        }

        /// <summary>
        /// https://developer.xamarin.com/guides/ios/platform_features/user-notifications/deprecated/remote_notifications_in_ios/
        /// https://developer.apple.com/library/content/documentation/NetworkingInternetWeb/Conceptual/NetworkingOverview/UnderstandingandPreparingfortheIPv6Transition/UnderstandingandPreparingfortheIPv6Transition.html#//apple_ref/doc/uid/TP40010220-CH213-SW16
        /// </summary>
        /// <param name="application"></param>
        /// <param name="deviceToken"></param>
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            var _device_id = new StringBuilder();

            var _pointer = deviceToken.ToByteArray();
            for (int i = 0; i < 32; i++)
                _device_id.Append(_pointer[i].ToString("X2"));

            this.APNSKey = _device_id.ToString();
        }

        private void RegisterForRemoteNotifications()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {
                    if (granted)
                    {
                        InvokeOnMainThread(() =>
                        {
                            UIApplication.SharedApplication.RegisterForRemoteNotifications();
                        });
                    }
                });

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = new MyUNUserNotificationCenterDelegate();
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        private void UserSetting()
        {
            var _defaults = NSDictionary.FromObjectsAndKeys(
                    new object[] {
                        "", "", false, false
                    },
                    new object[] {
                        "login_id", "password", "is_save_loginid", "is_save_password"
                    }
                );

            NSUserDefaults.StandardUserDefaults.RegisterDefaults(_defaults);

            this.Edited = false;
            this.Logined = false;
            this.UserInfo = new UserInfo();

            NSUserDefaults.StandardUserDefaults.SetBool(true, "is_save_loginid");
            NSUserDefaults.StandardUserDefaults.SetBool(true, "is_save_password");
        }

        public void ExcuteLogout()
        {
            this.Logined = false;
            this.UserToken = "";
            this.UserInfo = null;
        }
    }
}