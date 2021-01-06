using Foundation;
using Lion.XiOS.Libs;
using System;
using UIKit;

namespace Lion.XiOS
{
    public partial class NewPasswordViewController : UITableViewController, IUITextFieldDelegate
    {
        public NewPasswordViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            changeOKCell.TextLabel.BackgroundColor = UIColor.Clear;
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.InternetCheck();
        }

        bool ValidatePassword(string _password)
        {
            bool isValid;
            if (_password.Length >= 6)
                isValid = true;
            else
                isValid = false;

            return isValid;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return 3;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0 && indexPath.Row == 2)
            {
                this.PasswordChangeAlertNewPassword(tfOriginal.Text, tfNew.Text);
            }
        }

        private void PasswordChangeAlertNewPassword(string password1, string password2)
        {
            var check = this.ValidatePassword(password2);
            if (String.IsNullOrEmpty(password1) == true)
            {
                ConfigHelper.ShowConfirm("알림", "비밀번호를 입력해주세요.", "확인");
            }
            else if (String.IsNullOrEmpty(password2) == true)
            {
                ConfigHelper.ShowConfirm("알림", "새로운 비밀번호를 입력해주세요.", "확인");
            }
            else if (!check)
            {
                ConfigHelper.ShowConfirm("알림", "6글자 이상의 비밀번호를 입력해주세요.", "확인");
            }
            else if (!password1.Equals(password2))
            {
                ConfigHelper.ShowConfirm("알림", "2개의 비밀번호가 일치하지 않습니다.", "확인");
            }
            else if (password1.Equals(password2) && check)
            {
                this.ChangeNewPassword(password2);
            }
        }

        private void ChangeNewPassword(string newPassword)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.UserToken) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.ChangePassword(appDelegate.UserToken, newPassword);
                    if (responseDict.success == true)
                    {
                        NSUserDefaults.StandardUserDefaults.SetString("", "login_id");
                        NSUserDefaults.StandardUserDefaults.SetString("", "password");
                        NSUserDefaults.StandardUserDefaults.SetBool(true, "is_save_loginid");
                        NSUserDefaults.StandardUserDefaults.SetBool(true, "is_save_password");

                        appDelegate.ExcuteLogout();
                        this.NavigationController.PopViewController(true);

                        ConfigHelper.ShowConfirm("알림", "변경완료", "확인");
                    }
                    else
                        ConfigHelper.ShowConfirm("알림", responseDict.message, "확인");
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", ex.Message, "확인");
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            this.TableView.EndEditing(true);
        }

        [Export("textFieldShouldReturn:")]
        bool ShouldReturn(UITextField textField)
        {
            textField.ResignFirstResponder();
            return true;
        }

        [Export("textFieldDidEndEditing:")]
        private void EditingEnded(UITextField textField)
        {
            textField.ResignFirstResponder();
        }

        private void InternetCheck()
        {
        }
    }
}