using Foundation;
using Lion.XiOS.Libs;
using System;
using UIKit;

namespace Lion.XiOS
{
    public partial class RenewViewController : UITableViewController, IUITextFieldDelegate
    {
        public RenewViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
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
            return 1;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return 2;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0 && indexPath.Row == 1)
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (ConfigHelper.NSStringIsValidEmail(this.tfEmail.Text) == true)
                    this.SendMailToRecoveryIdWithGusetTokenEmail(appDelegate.GuestToken, this.tfEmail.Text);
                else
                    ConfigHelper.ShowConfirm("알림", "올바른 이메일 주소를 입력해 주세요.", "확인");
            }
        }

        private void SendMailToRecoveryIdWithGusetTokenEmail(string guest_token, string mail_address)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(guest_token) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.SendMailToRecoveryId(guest_token, mail_address);
                    if (responseDict.success == true)
                    {
                        this.NavigationController.PopViewController(true);
                        ConfigHelper.ShowConfirm("알림", $"{mail_address}로 메일을 발송했습니다.", "확인");
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