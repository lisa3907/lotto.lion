using Lion.XiOS.Libs;
using System;
using UIKit;

namespace Lion.XiOS
{
    public partial class SingUpViewController2 : UIViewController
    {
        protected string __guest_token;
        protected bool __email_check;

        public SingUpViewController2(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            {
                __guest_token = appDelegate.NetworkInstance.GetGuestToken().result;
                __email_check = false;
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        private void SendCheckEmail(string mail_address)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(__guest_token) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.SendMailToCheckMailAddress(__guest_token, mail_address);
                    if (responseDict.success == true)
                    {
                        __email_check = responseDict.success;
                        ConfigHelper.ShowConfirm("알림", "입력하신 이메일 주소로 인증번호를 발송하였습니다.\n받으신 인증번호를 입력해주세요.", "확인");
                    }
                    else
                        ConfigHelper.ShowConfirm("실패", responseDict.message, "확인");
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
                if (String.IsNullOrEmpty(__guest_token) == false)
                {
                    var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

                    var responeCheck = appDelegate.NetworkInstance.CheckMailAddress(__guest_token, email, number);
                    if (responeCheck.success == true)
                    {
                        ConfigHelper.ShowConfirm("알림", "인증완료", "확인");

                        this.tfConfirm.BackgroundColor = UIColor.LightGray;
                        this.tfConfirm.Enabled = false;
                    }
                    else
                        ConfigHelper.ShowConfirm("알림", "인증실패\n확인 후 다시 입력해주세요", "확인");
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
                if (String.IsNullOrEmpty(__guest_token) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.AddMemberByLoginId(__guest_token, login_id, login_name, password, mail_address, device_id, "I", check_number);
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

        private void BtnSendEmailAction(UIButton sender)
        {
            if (String.IsNullOrEmpty(this.tfEmail.Text) == true)
                ConfigHelper.ShowConfirm("알림", "이메일을 입력해주세요.", "확인");
            else if (ConfigHelper.NSStringIsValidEmail(this.tfEmail.Text) == false)
                ConfigHelper.ShowConfirm("알림", "유효한 이메일을 입력해주세요.", "확인");
            else
                this.SendCheckEmail(this.tfEmail.Text);
        }

        private void BtnEmailCheckAction(UIButton sender)
        {
            if (String.IsNullOrEmpty(this.tfConfirm.Text) == true)
                ConfigHelper.ShowConfirm("알림", "이메일을 입력해주세요.", "확인");
            else
                this.SendCheckEmailConfirmNumber(this.tfEmail.Text, this.tfConfirm.Text);
        }

        private void BtnOkAction(UIButton sender)
        {
            if (__email_check == true)
            {
                if (String.IsNullOrEmpty(this.tfID.Text) == true)
                {
                    ConfigHelper.ShowConfirm("알림", "유효한 아이디를 입력해주세요.", "확인");
                }
                else if (ConfigHelper.NSStringIsValidEmail(this.tfEmail.Text))
                {
                    ConfigHelper.ShowConfirm("알림", "유효한 이메일을 입력해주세요.", "확인");
                }
                else if (String.IsNullOrEmpty(this.tfConfirm.Text) == true)
                {
                    ConfigHelper.ShowConfirm("알림", "인증번호를 입력해주세요.", "확인");
                }
                else if (String.IsNullOrEmpty(this.tfName.Text) == true)
                {
                    ConfigHelper.ShowConfirm("알림", "이름을 입력해주세요.", "확인");
                }
                else if (ConfigHelper.ValidatePassword(this.tfPassword.Text))
                {
                    ConfigHelper.ShowConfirm("알림", "유효한 패스워드를 입력해주세요.", "확인");
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

        bool ShouldReturn(UITextField textField)
        {
            textField.ResignFirstResponder();
            return true;
        }
    }
}