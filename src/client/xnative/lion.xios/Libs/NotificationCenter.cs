using iAd;
using ObjCRuntime;
using System;
using UserNotifications;

namespace Lion.XiOS.Libs
{
    public class MyUNUserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Badge);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, [BlockProxy(typeof(AdAction))] Action completionHandler)
        {
            completionHandler();
        }
    }
}
