using Foundation;
using Lion.XiOS.Libs;
using System;
using UIKit;

namespace Lion.XiOS
{
    public partial class LogoutViewController : UITableViewController
    {
        public LogoutViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.signOutCell.TextLabel.BackgroundColor = UIColor.Clear;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.InternetCheck();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 2;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return 1;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0 && indexPath.Row == 0)
            {
                this.ExcuteLogout();
            }

            if (indexPath.Section == 1 && indexPath.Row == 0)
            {
                ConfigHelper.ShowYesNo("알림", "탈퇴 하시겠습니까?", "탈퇴", "취소", action =>
                {
                    this.ExcuteLeaveMember();
                });                  
            }
        }

        private void ExcuteLeaveMember()
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.UserToken) == false)
                {
                    appDelegate.NetworkInstance.LeaveMember(appDelegate.UserToken);

                    appDelegate.Logined = false;
                    appDelegate.UserToken = "";
                    appDelegate.UserInfo = null;

                    NSUserDefaults.StandardUserDefaults.SetString("", "login_id");
                    NSUserDefaults.StandardUserDefaults.SetString("", "password");
                    NSUserDefaults.StandardUserDefaults.SetBool(true, "is_save_loginid");
                    NSUserDefaults.StandardUserDefaults.SetBool(true, "is_save_password");

                    ConfigHelper.ShowConfirm("알림", "탈퇴하였습니다.", "확인", action =>
                    {
                        this.NavigationController.PopToRootViewController(true);
                    });
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", ex.Message, "확인");
            }
        }

        private void ExcuteLogout()
        {
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            appDelegate.ExcuteLogout();

            ConfigHelper.ShowConfirm("알림", "로그아웃 되었습니다.", "확인", action =>
            {
                this.NavigationController.PopToRootViewController(true);
            });
        }

        private void InternetCheck()
        {
        }
    }
}