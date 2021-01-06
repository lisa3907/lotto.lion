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
    [Register ("NewPasswordViewController")]
    partial class NewPasswordViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell changeOKCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfNew { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfOriginal { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (changeOKCell != null) {
                changeOKCell.Dispose ();
                changeOKCell = null;
            }

            if (tfNew != null) {
                tfNew.Dispose ();
                tfNew = null;
            }

            if (tfOriginal != null) {
                tfOriginal.Dispose ();
                tfOriginal = null;
            }
        }
    }
}