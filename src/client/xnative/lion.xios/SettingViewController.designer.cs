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
    [Register ("SettingViewController")]
    partial class SettingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell Cell1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell Cell2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell Cell3 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell inboxCell { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Cell1 != null) {
                Cell1.Dispose ();
                Cell1 = null;
            }

            if (Cell2 != null) {
                Cell2.Dispose ();
                Cell2 = null;
            }

            if (Cell3 != null) {
                Cell3.Dispose ();
                Cell3 = null;
            }

            if (inboxCell != null) {
                inboxCell.Dispose ();
                inboxCell = null;
            }
        }
    }
}