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
    [Register ("logoutViewController")]
    partial class LogoutViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell signOutCell { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (signOutCell != null) {
                signOutCell.Dispose ();
                signOutCell = null;
            }
        }
    }
}