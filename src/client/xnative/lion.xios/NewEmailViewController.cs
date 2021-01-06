using Foundation;
using Lion.XiOS.Libs;
using System;
using UIKit;

namespace Lion.XiOS
{
    public partial class NewEmailViewController : UITableViewController, IUITextFieldDelegate
    {
        private bool __mail_checked = false;

        public NewEmailViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.confirmSendCell.TextLabel.BackgroundColor = UIColor.Clear;
            this.confirmOKCell.TextLabel.BackgroundColor = UIColor.Clear;

            __mail_checked = false;
            tfEmail.KeyboardType = UIKeyboardType.EmailAddress;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.InternetCheck();
        }


        public override nint NumberOfSections(UITableView tableView)
        {
            return 2;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return 2;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0 && indexPath.Row == 1)
            {
                this.EmailCheckAlert(tfEmail.Text);
            }
            else if (indexPath.Section == 1 && indexPath.Row == 1)
            {
                if (__mail_checked == true)
                {
                    if (String.IsNullOrEmpty(this.tfCode.Text) == true)
                        ConfigHelper.ShowConfirm("알림", "인증 번호를 입력해주세요.", "확인");
                    else if (tfCode.Text.Length != 6)
                        ConfigHelper.ShowConfirm("알림", "인증 번호를 정확히 입력해주세요.", "확인");
                    else
                        this.SendCheckEmailConfirmNumber(tfEmail.Text, tfCode.Text);
                }
                else
                    ConfigHelper.ShowConfirm("알림", "올바른 이메일 주소를 입력해주세요.", "확인");
            }
        }

        private void EmailCheckAlert(string mail_address)
        {
            if (String.IsNullOrEmpty(mail_address) == false)
            {
                var _validation = ConfigHelper.NSStringIsValidEmail(mail_address);
                if (_validation == true)
                {
                    this.SendCheckEmail(mail_address);
                    __mail_checked = true;

                    ConfigHelper.ShowConfirm("알림", $"{mail_address}\n인증 번호를 발송했습니다.", "확인");
                }
                else
                    ConfigHelper.ShowConfirm("알림", "올바른 이메일 주소를 입력해주세요.", "확인");
            }
            else
                ConfigHelper.ShowConfirm("알림", "이메일 주소를 입력해주세요.", "확인");
        }

        private void SendCheckEmailConfirmNumber(string email, string number)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.GuestToken) == false)
                {
                    var responeCheck = appDelegate.NetworkInstance.CheckMailAddress(appDelegate.UserToken, email, number);
                    if (responeCheck.success == true)
                        this.UpdateMailAddressConfirmNumber(email, number);
                    else
                        ConfigHelper.ShowConfirm("알림", "인증실패\n확인 후 다시 입력해주세요.", "확인");
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", ex.Message, "확인");
            }
        }

        private void UpdateMailAddressConfirmNumber(string newMailAddress, string confirmNumber)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.GuestToken) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.UpdateMailAddress(appDelegate.UserToken, newMailAddress, confirmNumber);
                    if (responseDict.success == true)
                    {
                        appDelegate.Edited = true;
                        this.GetUserInfo(appDelegate.UserToken);

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

        private void SendCheckEmail(string mail_address)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.GuestToken) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.SendMailToChangeMailAddress(appDelegate.UserToken, mail_address);
                    if (responseDict.success == false)
                        ConfigHelper.ShowConfirm("알림", responseDict.message, "확인");
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", ex.Message, "확인");
            }
        }

        private void GetUserInfo(string user_token)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.UserToken) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.GetUserInfor(appDelegate.UserToken);
                    appDelegate.UserInfo = responseDict.result;
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
        public bool ShouldReturn(UITextField textField)
        {
            textField.ResignFirstResponder();
            return true;
        }

        [Export("textFieldDidEndEditing:")]
        public void EditingEnded(UITextField textField)
        {
            textField.ResignFirstResponder();
        }

        private void InternetCheck()
        {
        }
    }
}