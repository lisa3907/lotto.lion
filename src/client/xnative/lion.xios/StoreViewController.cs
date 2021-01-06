using Foundation;
using System;
using System.Net;
using UIKit;

namespace Lion.XiOS
{
    public partial class StoreViewController : UIViewController
    {
        public StoreViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.myLodingIndicator.StartAnimating();
            this.PerformSelector(new ObjCRuntime.Selector("LoadMap"), this.myLodingIndicator, 0);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.InternetCheck();
        }

        [Export("LoadMap")]
        private void LoadMap()
        {
            var originalString = "로또판매점";

            var escapedString = WebUtility.UrlEncode(originalString);
            var urlString = String.Format("https://m.map.naver.com/search2/search.nhn?query={0}&sm=sug#/map/1", escapedString);

            var theURL = NSUrl.FromString(urlString);
            var urlRequest = NSUrlRequest.FromUrl(theURL);

            this.myWebView.LoadRequest(urlRequest);
            this.myLodingIndicator.StopAnimating();
        }

        private void InternetCheck()
        {
            //if (Reachability.ReachabilityForInternetConnection().CurrentReachabilityStatus() == NotReachable)
            //{
            //    var appDelegate = (AppDelegate)UIApplication.SharedApplication().TheDelegate();
            //    appDelegate.Logined = false;
            //    appDelegate.UserToken = "";
            //    UIAlertController alertController = UIAlertController.AlertControllerWithTitleMessagePreferredStyle("?곌껐 ?ㅻ쪟", "?ㅽ듃?뚰겕 ?곌껐 ?곹깭 ?뺤씤 ??n?ㅼ떆 ?쒕룄?댁＜?몄슂.", UIAlertControllerStyleAlert);
            //    UIAlertAction actionOk = UIAlertAction.ActionWithTitleStyleHandler("?뺤씤", UIAlertActionStyleDefault, null);
            //    alertController.AddAction(actionOk);
            //    var vc = UIApplication.SharedApplication().TheDelegate().Window().RootViewController();
            //    vc.PresentViewControllerAnimatedCompletion(alertController, true, null);
            //}
            //else
            //{
            //}
        }
    }
}