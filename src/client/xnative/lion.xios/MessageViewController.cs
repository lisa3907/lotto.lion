using Foundation;
using Lion.XiOS.Libs;
using Lion.XIOS.Type;
using System;
using System.Collections.Generic;
using UIKit;

namespace Lion.XiOS
{
    public partial class MessageViewController : UITableViewController
    {
        public string __this_time;
        public List<PushMessage> __message_array;

        public MessageViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            __this_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.GetAlertMessage();
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
            this.ClearMsgCount();
        }

        private void GetAlertMessage()
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.UserToken) == false)
                {
                    var responseDict = appDelegate.NetworkInstance.GetMessages(appDelegate.UserToken, __this_time);
                    __message_array = responseDict.result;
                }
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", ex.Message, "확인");
            }
        }

        private void ClearMsgCount()
        {
            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            if (String.IsNullOrEmpty(appDelegate.UserToken) == false)
                appDelegate.NetworkInstance.ClearAlert(appDelegate.UserToken);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return (nint)__message_array.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("Cell", indexPath);

            cell.TextLabel.Text = __message_array[indexPath.Row].message;
            cell.DetailTextLabel.Text = __message_array[indexPath.Row].notifyTime.ToString("yyyy-MM-dd HH:mm:ss");

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ConfigHelper.ShowConfirm("메세지", __message_array[indexPath.Row].message, "확인");
        }

        private void InternetCheck()
        {
        }
    }
}