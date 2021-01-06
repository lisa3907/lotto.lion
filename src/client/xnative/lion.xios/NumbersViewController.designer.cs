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
    [Register ("NumbersViewController")]
    partial class NumbersViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBackward { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnForward { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTurn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView loginView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView myIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel noticeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView numbersTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView seqTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView seqView { get; set; }

        [Action ("SeqforwardAction:")]
        partial void SeqforwardAction (UIKit.UIButton sender);

        [Action ("SeqBackAction:")]
        partial void SeqBackAction (UIKit.UIButton sender);

        [Action ("btnMessageAction:")]
        partial void BtnMessageAction (UIKit.UIButton sender);

        [Action ("btnTurnAction:")]
        partial void BtnTurnAction (UIKit.UIButton sender);

        [Action ("btnBackAction:")]
        partial void BtnBackAction (UIKit.UIButton sender);

        [Action ("btnTurnOkAction:")]
        partial void BtnTurnOkAction (UIKit.UIButton sender);

        [Action ("btnTurnOKAction:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void btnTurnOKAction (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnBackward != null) {
                btnBackward.Dispose ();
                btnBackward = null;
            }

            if (btnForward != null) {
                btnForward.Dispose ();
                btnForward = null;
            }

            if (lblDate != null) {
                lblDate.Dispose ();
                lblDate = null;
            }

            if (lblTurn != null) {
                lblTurn.Dispose ();
                lblTurn = null;
            }

            if (loginView != null) {
                loginView.Dispose ();
                loginView = null;
            }

            if (myIndicator != null) {
                myIndicator.Dispose ();
                myIndicator = null;
            }

            if (noticeLabel != null) {
                noticeLabel.Dispose ();
                noticeLabel = null;
            }

            if (numbersTable != null) {
                numbersTable.Dispose ();
                numbersTable = null;
            }

            if (seqTable != null) {
                seqTable.Dispose ();
                seqTable = null;
            }

            if (seqView != null) {
                seqView.Dispose ();
                seqView = null;
            }
        }
    }
}