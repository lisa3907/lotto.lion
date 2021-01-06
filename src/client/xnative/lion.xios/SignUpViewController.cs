using Foundation;
using Lion.XiOS.Libs;
using System;
using UIKit;

namespace Lion.XiOS
{
    public partial class SignUpViewController : UITableViewController, IUITextFieldDelegate
    {
        protected bool __email_check;

        public SignUpViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.NavigationItem.Title = "회원가입";

            this.confirmButtonCell.TextLabel.BackgroundColor = UIColor.Clear;
            this.confirmButtonCell2.TextLabel.BackgroundColor = UIColor.Clear;
            this.signUpButtonCell.TextLabel.BackgroundColor = UIColor.Clear;

            __email_check = false;

            this.tfEmail.KeyboardType = UIKeyboardType.EmailAddress;

            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            if (String.IsNullOrEmpty(appDelegate.GuestToken) == true)
                this.GetGuestToken();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.InternetCheck();
        }

        private bool ValidatePassword(string _password)
        {
            var isValid = false;

            if (_password.Length >= 6)
                isValid = true;

            return isValid;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 3;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            if (section == 0)
                return 2;
            else if (section == 1)
                return 2;
            else if (section == 2)
                return 5;

            return 0;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0 && indexPath.Row == 1)
            {
                if (ConfigHelper.NSStringIsValidEmail(this.tfEmail.Text) == true)
                {
                    this.SendCheckEmail(this.tfEmail.Text);

                    this.tfConfirm.BackgroundColor = UIColor.White;
                    this.tfConfirm.Enabled = true;
                }
                else
                    ConfigHelper.ShowConfirm("알림", "올바른 이메일 주소를 입력해주세요.", "확인");
            }

            if (indexPath.Section == 1 && indexPath.Row == 1)
            {
                if (this.tfConfirm.Text.Length != 6)
                    ConfigHelper.ShowConfirm("알림", "인증코드를 정확히 입력해주세요.", "확인");
                else
                    this.SendCheckEmailConfirmNumber(this.tfEmail.Text, this.tfConfirm.Text);
            }

            if (indexPath.Section == 2 && indexPath.Row == 4)
            {
                if (__email_check)
                {
                    if (String.IsNullOrEmpty(this.tfID.Text) == true)
                    {
                        ConfigHelper.ShowConfirm("알림", "아이디를 입력해주세요.", "확인");
                    }
                    else if (ConfigHelper.NSStringIsValidEmail(this.tfEmail.Text) == false)
                    {
                        ConfigHelper.ShowConfirm("알림", "유효한 이메일을 입력해주세요.", "확인");
                    }
                    else if (this.tfConfirm.Text.Length != 6)
                    {
                        ConfigHelper.ShowConfirm("알림", "인증번호를 입력해주세요.", "확인");
                    }
                    else if (String.IsNullOrEmpty(this.tfName.Text) == true)
                    {
                        ConfigHelper.ShowConfirm("알림", "이름을 입력해주세요.", "확인");
                    }
                    else if (this.ValidatePassword(this.tfPassword.Text) == false)
                    {
                        ConfigHelper.ShowConfirm("알림", "6글자 이상의 비밀번호를 입력해주세요.", "확인");
                    }
                    else if (this.tfPassword.Text.Equals(this.tfPassword2nd.Text) == false)
                    {
                        ConfigHelper.ShowConfirm("알림", "2개의 패스워드가 일치하지 않습니다.", "확인");
                    }
                    else
                    {
                        var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                        this.RequestSigninLoginNamePasswordEmailDeviceIDCheckNumber(this.tfID.Text, this.tfName.Text, this.tfPassword.Text, this.tfEmail.Text, appDelegate.APNSKey, this.tfConfirm.Text);
                    }
                }
                else
                {
                    ConfigHelper.ShowConfirm("알림", "이메일 인증을 선택해주세요.", "확인");
                }
            }

            tableView.DeselectRow(indexPath, true);
        }

        private void GetGuestToken()
        {
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            appDelegate.GuestToken = appDelegate.NetworkInstance.GetGuestToken().result;
        }

        private void SendCheckEmail(string email)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.GuestToken) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.SendMailToCheckMailAddress(appDelegate.GuestToken, email);
                    if (responseDict.success == true)
                    {
                        ConfigHelper.ShowConfirm("알림", "입력하신 이메일 주소로 인증번호를 발송하였습니다.\n받으신 인증번호를 입력해주세요.", "확인");
                        __email_check = true;
                    }
                    else
                    {
                        ConfigHelper.ShowConfirm("실패", responseDict.message, "확인");
                        __email_check = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", ex.Message, "확인");
            }
        }

        private void SendCheckEmailConfirmNumber(string email, string number)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.GuestToken) == false)
                {
                    var responeCheck = appDelegate.NetworkInstance.CheckMailAddress(appDelegate.GuestToken, email, number);
                    if (responeCheck.success == true)
                    {
                        ConfigHelper.ShowConfirm("알림", "인증완료", "확인");

                        this.tfConfirm.BackgroundColor = UIColor.LightGray;
                        this.tfConfirm.Enabled = false;
                    }
                    else
                        ConfigHelper.ShowConfirm("알림", "인증실패\n확인 후 다시 입력해주세요.", "확인");
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", ex.Message, "확인");
            }
        }

        private void RequestSigninLoginNamePasswordEmailDeviceIDCheckNumber(string login_id, string login_name, string password, string mail_address, string device_id, string check_number)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.GuestToken) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.AddMemberByLoginId(appDelegate.GuestToken, login_id, login_name, password, mail_address, device_id, "I", check_number);
                    if (responseDict.success == true)
                    {
                        this.NavigationController.PopViewController(true);
                        ConfigHelper.ShowConfirm("가입성공", "회원가입이 완료되었습니다.", "확인");
                    }
                    else
                        ConfigHelper.ShowConfirm("가입실패", responseDict.message, "확인");
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