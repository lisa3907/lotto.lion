using Foundation;
using Lion.XiOS.Libs;
using System;
using System.Collections;
using System.Diagnostics;
using UIKit;

namespace Lion.XiOS
{
    public partial class CountSetViewController : UITableViewController
    {
        public ArrayList CountArray { get; set; }

        public ArrayList SelectedRowCountArr { get; set; }

        public UIAlertController FilterView { get; set; }

        public string CheckMode { get; set; }

        public short MaxCount { get; set; }

        public string EmailAddress { get; set; }

        public bool EditCheck { get; set; }

        private ArrayList arrayToDelete;
        private ArrayList numbersData;

        public CountSetViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            MaxCount = appDelegate.UserInfo.maxSelectNumber;

            EditCheck = false;

            if (CheckMode.Equals("number"))
            {
                numbersData = new ArrayList();

                for (int i = 1; i < 46; i++)
                {
                    var temp = String.Format("{0}", i);
                    numbersData.Add(temp);
                }

                arrayToDelete = new ArrayList();
                if (SelectedRowCountArr == null)
                {
                    SelectedRowCountArr = new ArrayList();
                }

                //var newAddButton = new UIBarButtonItem("추가", UIBarButtonItemStyle.Plain, this, new ObjCRuntime.Selector("Add"));
                var newAddButton = new UIBarButtonItem("추가", UIBarButtonItemStyle.Plain, Add);

                this.NavigationItem.RightBarButtonItem = newAddButton;
                this.NavigationItem.Title = "고정번호선택";
            }
            else if (CheckMode.Equals("count"))
            {
                this.NavigationItem.Title = "번호생성갯수";
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            this.InternetCheck();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            if (CheckMode.Equals("number") && appDelegate.Edited == true)
            {
                if (SelectedRowCountArr.Count == 0)
                {
                    this.UpdateInfoWithMaxCountNum1Num2Num3(MaxCount, "0", "0", "0");
                }
                else if (SelectedRowCountArr.Count == 1)
                {
                    this.UpdateInfoWithMaxCountNum1Num2Num3(MaxCount, SelectedRowCountArr[0].ToString(), "", "");
                }
                else if (SelectedRowCountArr.Count == 2)
                {
                    this.UpdateInfoWithMaxCountNum1Num2Num3(MaxCount, SelectedRowCountArr[0].ToString(), SelectedRowCountArr[1].ToString(), "");
                }
                else
                {
                    this.UpdateInfoWithMaxCountNum1Num2Num3(MaxCount, SelectedRowCountArr[0].ToString(), SelectedRowCountArr[1].ToString(), SelectedRowCountArr[2].ToString());
                }
            }

        }

        private void Add(object sender, EventArgs e)
        {
            if (SelectedRowCountArr.Count < 3)
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                appDelegate.Edited = true;

                var alertController = UIAlertController.Create("번호선택", "", UIAlertControllerStyle.ActionSheet);
                foreach (string item in numbersData)
                {
                    var defaultAction = UIAlertAction.Create(item, UIAlertActionStyle.Default, action => 
                    {
                        this.DidSelectRowInAlertController(Convert.ToInt16(item));
                    });

                    alertController.AddAction(defaultAction);
                }

                var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, null);
                alertController.AddAction(cancelAction);

                this.FilterView = alertController;
                this.PresentViewController(this.FilterView, true, null);
            }
            else
                ConfigHelper.ShowConfirm("알림", "번호 선택은 최대 3개입니다.", "확인");
        }

        private void DidSelectRowInAlertController(int row)
        {
            var item = String.Format("{0}", (int)row);
            SelectedRowCountArr.Add(item);

            this.TableView.ReloadData();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            if (CheckMode.Equals("count"))
            {
                return CountArray.Count;
            }
            else if (CheckMode.Equals("number"))
            {
                return SelectedRowCountArr.Count;
            }
            else
            {
                return 0;
            }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("Cell", indexPath);

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.Accessory = UITableViewCellAccessory.None;
            if (CheckMode.Equals("count"))
            {
                cell.SelectionStyle = UITableViewCellSelectionStyle.Default;
                cell.TextLabel.Text = String.Format("{0}개", CountArray[indexPath.Row].ToString());
                if (Convert.ToInt32(MaxCount) == Convert.ToInt32(CountArray[indexPath.Row]))
                {
                    cell.Accessory = UITableViewCellAccessory.Checkmark;
                }
            }
            else if (CheckMode.Equals("number"))
            {
                cell.TextLabel.Text = String.Format("{0}", SelectedRowCountArr[indexPath.Row]);
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath);

            if (CheckMode.Equals("count"))
            {
                cell.Accessory = UITableViewCellAccessory.Checkmark;

                this.UpdateInfoWithMaxCountNum1Num2Num3(Convert.ToInt16(CountArray[indexPath.Row]), SelectedRowCountArr[0].ToString(), SelectedRowCountArr[1].ToString(), SelectedRowCountArr[2].ToString());
                this.NavigationController.PopViewController(true);
            }
            else if (CheckMode.Equals("number"))
            {
                arrayToDelete.Add(SelectedRowCountArr[indexPath.Row]);
            }
        }

        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            var cell = tableView.CellAt(indexPath);

            if (CheckMode.Equals("count"))
            {
                cell.Accessory = UITableViewCellAccessory.None;
            }
            else if (CheckMode.Equals("number"))
            {
                cell.Accessory = UITableViewCellAccessory.None;
                arrayToDelete.Remove(SelectedRowCountArr[indexPath.Row]);
            }
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.Delete;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (CheckMode.Equals("number"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            if (CheckMode.Equals("number"))
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                appDelegate.Edited = true;

                SelectedRowCountArr.RemoveAt(indexPath.Row);
                this.TableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
            }
        }

        private void UpdateInfoWithMaxCountNum1Num2Num3(short max_count, string digit1, string digit2, string digit3)
        {
            try
            {
                var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
                if (String.IsNullOrEmpty(appDelegate.UserToken) == false)
                {
                    var userName = appDelegate.UserInfo.loginName;

                    var _response = appDelegate.NetworkInstance.UpdateUserInfor(appDelegate.UserToken, userName, max_count, digit1, digit2, digit3);
                    if (_response.success == true)
                    {
                        appDelegate.Edited = true;
                        this.GetUserInfo(appDelegate.UserToken);
                    }
                    else
                        ConfigHelper.ShowConfirm("알림", "유저정보 변경에 실패했습니다.\n네트워크 환경을 확인해주세요.", "확인");
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

        private void InternetCheck()
        {
        }
    }
}