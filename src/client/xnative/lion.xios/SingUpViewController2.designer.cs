// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System.CodeDom.Compiler;
using UIKit;

namespace Lion.XiOS
{
    [Register("SingUpViewController2")]
    partial class SingUpViewController2
    {
        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UITextField tfEmail { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UITextField tfConfirm { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UITextField tfID { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UITextField tfName { get; set; }

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        UITextField tfPassword { get; set; }

        private void ReleaseDesignerOutlets()
        {
            if (tfEmail != null)
            {
                tfConfirm.Dispose();
                tfConfirm = null;
            }

            if (tfConfirm != null)
            {
                tfEmail.Dispose();
                tfEmail = null;
            }

            if (tfID != null)
            {
                tfID.Dispose();
                tfID = null;
            }

            if (tfName != null)
            {
                tfName.Dispose();
                tfName = null;
            }

            if (tfPassword != null)
            {
                tfPassword.Dispose();
                tfPassword = null;
            }
        }
    }
}