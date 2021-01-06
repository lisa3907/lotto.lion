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
    [Register ("PrizeViewController")]
    partial class PrizeViewController
    {
        [Outlet]
        UIKit.UIImageView imgNum1 { get; set; }

        [Outlet]
        UIKit.UIImageView imgNum2 { get; set; }

        [Outlet]
        UIKit.UIImageView imgNum3 { get; set; }

        [Outlet]
        UIKit.UIImageView imgNum4 { get; set; }

        [Outlet]
        UIKit.UIImageView imgNum5 { get; set; }

        [Outlet]
        UIKit.UIImageView imgNum6 { get; set; }

        [Outlet]
        UIKit.UIImageView imgNum7 { get; set; }

        [Outlet]
        UIKit.UIView internetView { get; set; }

        [Outlet]
        UIKit.UILabel lblDate { get; set; }

        [Outlet]
        UIKit.UILabel lblDate2 { get; set; }

        [Outlet]
        UIKit.UILabel lblNum1 { get; set; }

        [Outlet]
        UIKit.UILabel lblNum2 { get; set; }

        [Outlet]
        UIKit.UILabel lblNum3 { get; set; }

        [Outlet]
        UIKit.UILabel lblNum4 { get; set; }

        [Outlet]
        UIKit.UILabel lblNum5 { get; set; }

        [Outlet]
        UIKit.UILabel lblNum6 { get; set; }

        [Outlet]
        UIKit.UILabel lblNum7 { get; set; }

        [Outlet]
        UIKit.UILabel lblPrize { get; set; }

        [Outlet]
        UIKit.UILabel lblPrize2 { get; set; }

        [Outlet]
        UIKit.UILabel lblSeq { get; set; }

        [Outlet]
        UIKit.UILabel lblSeq2 { get; set; }

        [Outlet]
        UIKit.UIActivityIndicatorView myLoadIndicator { get; set; }

        [Outlet]
        UIKit.UITableView prizeTableView { get; set; }

        [Outlet]
        UIKit.UIView SeqView { get; set; }

        [Action ("btnCloseAction:")]
        partial void BtnCloseAction (UIKit.UIButton sender);

        [Action ("btnOkAction:")]
        partial void BtnOkAction (UIKit.UIButton sender);

        [Action ("btnTurnAction:")]
        partial void BtnTurnAction (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}