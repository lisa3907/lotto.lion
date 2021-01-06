using Foundation;
using Lion.XiOS.Libs;
using System;
using System.Collections;
using UIKit;

namespace Lion.XiOS
{
    public partial class SettingViewController : UITableViewController
    {

        public string UserTokenForSetting { get; set; }

        public int MessageCount { get; set; }

        public UIAlertController FilterView { get; set; }

        public NSDictionary UserInfoDic { get; set; }

        public SettingViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.Cell1.DetailTextLabel.Text = this.Cell2.DetailTextLabel.Text = this.Cell3.DetailTextLabel.Text = this.inboxCell.DetailTextLabel.Text = "로그인 필요";
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.InternetCheck();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.CheckUserToken();
        }

        private void CheckUserToken()
        {
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            if (String.IsNullOrEmpty(appDelegate.UserToken) == true || appDelegate.Logined == false)
            {
                this.Cell1.DetailTextLabel.Text = this.Cell2.DetailTextLabel.Text = this.Cell3.DetailTextLabel.Text = this.inboxCell.DetailTextLabel.Text = "로그인 필요";

                var loginViewCont = this.Storyboard.InstantiateViewController("loginView") as LoginViewController;
                this.NavigationController.PushViewController(loginViewCont, true);
            }
            else
                this.GetMessageCount();

            if (appDelegate.Edited)
            {
                this.SetSettingView();
                appDelegate.Edited = false;
            }
        }

        private void SetSettingView()
        {
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            this.Cell1.DetailTextLabel.Text = $"{appDelegate.UserInfo.maxSelectNumber}";

            var num1 = $"{appDelegate.UserInfo.digit1}";
            var num2 = $"{appDelegate.UserInfo.digit2}";
            var num3 = $"{appDelegate.UserInfo.digit3}";

            if (!num1.Equals("0") && !num2.Equals("0") && !num3.Equals("0"))
                this.Cell2.DetailTextLabel.Text = String.Format("{0}, {1}, {2}", num1, num2, num3);
            else if (!num1.Equals("0") && num2.Equals("0") && num3.Equals("0"))
                this.Cell2.DetailTextLabel.Text = num1;
            else if (!num1.Equals("0") && !num2.Equals("0") && num3.Equals("0"))
                this.Cell2.DetailTextLabel.Text = String.Format("{0}, {1}", num1, num2);
            else
                this.Cell2.DetailTextLabel.Text = "없음";

            this.Cell3.DetailTextLabel.Text = appDelegate.UserInfo.emailAddress;
            this.Cell3.TextLabel.Text = "이메일";
        }

        private void GetUserInfo(string user_token)
        {
            try
            {
                if (String.IsNullOrEmpty(user_token) == false)
                {
                    var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

                    var responseDict = appDelegate.NetworkInstance.GetUserInfor(user_token);
                    appDelegate.UserInfo = responseDict.result;
                }
                else
                    this.ExcuteLogout();
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", ex.Message, "확인");
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 4;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            if (section == 0)
            {
                return 1;
            }
            else if (section == 1)
            {
                return 2;
            }
            else if (section == 2)
            {
                return 2;
            }
            else if (section == 3)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0 && indexPath.Row == 0)
            {
                var logoutViewCont = this.Storyboard.InstantiateViewController("logoutView") as LogoutViewController;
                this.NavigationController.PushViewController(logoutViewCont, true);
            }
            else if (indexPath.Section == 1 && indexPath.Row == 0)
            {
                var countViewCont = this.Storyboard.InstantiateViewController("countSet") as CountSetViewController;
                countViewCont.CountArray = new ArrayList { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
                countViewCont.CheckMode = "count";
                var tempArray = new ArrayList();
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

                var num1 = $"{appDelegate.UserInfo.digit1}";
                var num2 = $"{appDelegate.UserInfo.digit2}";
                var num3 = $"{appDelegate.UserInfo.digit3}";
                tempArray.Add(num1);
                tempArray.Add(num2);
                tempArray.Add(num3);

                countViewCont.SelectedRowCountArr = tempArray;
                this.NavigationController.PushViewController(countViewCont, true);
            }
            else if (indexPath.Section == 1 && indexPath.Row == 1)
            {
                var countViewCont = this.Storyboard.InstantiateViewController("countSet") as CountSetViewController;
                countViewCont.CheckMode = "number";

                var tempArray = new ArrayList();
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

                var num1 = $"{appDelegate.UserInfo.digit1}";
                var num2 = $"{appDelegate.UserInfo.digit2}";
                var num3 = $"{appDelegate.UserInfo.digit3}";

                if (!num1.Equals("0"))
                    tempArray.Add(num1);

                if (!num2.Equals("0"))
                    tempArray.Add(num2);

                if (!num3.Equals("0"))
                    tempArray.Add(num3);

                countViewCont.SelectedRowCountArr = tempArray;
                this.NavigationController.PushViewController(countViewCont, true);
            }
            else if (indexPath.Section == 2 && indexPath.Row == 0)
            {
                var newEmailViewCont = this.Storyboard.InstantiateViewController("newEmailView") as NewEmailViewController;
                this.NavigationController.PushViewController(newEmailViewCont, true);
            }
            else if (indexPath.Section == 2 && indexPath.Row == 1)
            {
                var newPasswordViewCont = this.Storyboard.InstantiateViewController("newPasswordView") as NewPasswordViewController;
                this.NavigationController.PushViewController(newPasswordViewCont, true);
            }
            else if (indexPath.Section == 3 && indexPath.Row == 0)
            {
                if (MessageCount != 0)
                {
                    var viewCont = this.Storyboard.InstantiateViewController("messageView") as MessageViewController;
                    this.NavigationController.PushViewController(viewCont, true);
                }
                else
                    ConfigHelper.ShowConfirm("알림", "수신된 메세지가 없습니다.", "확인");
            }

            tableView.DeselectRow(indexPath, true);
        }

        private void GetMessageCount()
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.UserToken) == false)
                {
                    MessageCount = appDelegate.NetworkInstance.GetAlertCount(appDelegate.UserToken).result;
                    if (MessageCount > 0)
                        this.inboxCell.DetailTextLabel.Text = String.Format("{0}", MessageCount);
                    else
                        this.inboxCell.DetailTextLabel.Text = "";
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", ex.Message, "확인");
            }
        }

        private void PrepareForSegueSender(UIStoryboardSegue segue, object sender)
        {
            var countViewCont = segue.DestinationViewController as CountSetViewController;
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

            if (segue.Identifier.Equals("countSet"))
            {
                countViewCont.CountArray = new ArrayList { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
                countViewCont.CheckMode = "count";
            }
            else if (segue.Identifier.Equals("numberSet"))
            {
                countViewCont.CheckMode = "number";

                var tempArray = new ArrayList();
                var num1 = $"{appDelegate.UserInfo.digit1}";
                var num2 = $"{appDelegate.UserInfo.digit2}";
                var num3 = $"{appDelegate.UserInfo.digit3}";

                if (!num1.Equals("0"))
                    tempArray.Add(num1);

                if (!num2.Equals("0"))
                    tempArray.Add(num2);

                if (!num3.Equals("0"))
                    tempArray.Add(num3);

                countViewCont.SelectedRowCountArr = tempArray;
            }
        }

        private void ExcuteLogout()
        {
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            appDelegate.ExcuteLogout();

            ConfigHelper.ShowConfirm("알림", "로그아웃 되었습니다.", "확인", action =>
            {
                this.Cell1.DetailTextLabel.Text = this.Cell2.DetailTextLabel.Text = this.Cell3.DetailTextLabel.Text = "로그인 필요";

                var _login_view = this.Storyboard.InstantiateViewController("loginView") as LoginViewController;
                this.NavigationController.PushViewController(_login_view, true);
            });
        }

        private void InternetCheck()
        {
        }
    }
}