// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Lion.XiOS
{
    [Register ("NewEmailViewController")]
    partial class NewEmailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell confirmOKCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell confirmSendCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfCode { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfEmail { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (confirmOKCell != null) {
                confirmOKCell.Dispose ();
                confirmOKCell = null;
            }

            if (confirmSendCell != null) {
                confirmSendCell.Dispose ();
                confirmSendCell = null;
            }

            if (tfCode != null) {
                tfCode.Dispose ();
                tfCode = null;
            }

            if (tfEmail != null) {
                tfEmail.Dispose ();
                tfEmail = null;
            }
        }
    }
}