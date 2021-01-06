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
    [Register ("LoginViewController")]
    partial class LoginViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell loginButtonCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell renewButtonCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell signUpButtonCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch switchID { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch switchPW { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfID { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfPW { get; set; }

        [Action ("IDSwitchAction:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void IDSwitchAction (UIKit.UISwitch sender);

        [Action ("PWSwitchAction:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PWSwitchAction (UIKit.UISwitch sender);

        void ReleaseDesignerOutlets ()
        {
            if (loginButtonCell != null) {
                loginButtonCell.Dispose ();
                loginButtonCell = null;
            }

            if (renewButtonCell != null) {
                renewButtonCell.Dispose ();
                renewButtonCell = null;
            }

            if (signUpButtonCell != null) {
                signUpButtonCell.Dispose ();
                signUpButtonCell = null;
            }

            if (switchID != null) {
                switchID.Dispose ();
                switchID = null;
            }

            if (switchPW != null) {
                switchPW.Dispose ();
                switchPW = null;
            }

            if (tfID != null) {
                tfID.Dispose ();
                tfID = null;
            }

            if (tfPW != null) {
                tfPW.Dispose ();
                tfPW = null;
            }
        }
    }
}