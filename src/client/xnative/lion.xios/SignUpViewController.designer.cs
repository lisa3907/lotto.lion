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
    [Register ("SignUpViewController")]
    partial class SignUpViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell confirmButtonCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell confirmButtonCell2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableViewCell signUpButtonCell { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfConfirm { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfEmail { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfID { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField tfPassword2nd { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (confirmButtonCell != null) {
                confirmButtonCell.Dispose ();
                confirmButtonCell = null;
            }

            if (confirmButtonCell2 != null) {
                confirmButtonCell2.Dispose ();
                confirmButtonCell2 = null;
            }

            if (signUpButtonCell != null) {
                signUpButtonCell.Dispose ();
                signUpButtonCell = null;
            }

            if (tfConfirm != null) {
                tfConfirm.Dispose ();
                tfConfirm = null;
            }

            if (tfEmail != null) {
                tfEmail.Dispose ();
                tfEmail = null;
            }

            if (tfID != null) {
                tfID.Dispose ();
                tfID = null;
            }

            if (tfName != null) {
                tfName.Dispose ();
                tfName = null;
            }

            if (tfPassword != null) {
                tfPassword.Dispose ();
                tfPassword = null;
            }

            if (tfPassword2nd != null) {
                tfPassword2nd.Dispose ();
                tfPassword2nd = null;
            }
        }
    }
}