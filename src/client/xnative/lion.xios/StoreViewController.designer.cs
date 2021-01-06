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
    [Register ("StoreViewController")]
    partial class StoreViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView myLodingIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIWebView myWebView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (myLodingIndicator != null) {
                myLodingIndicator.Dispose ();
                myLodingIndicator = null;
            }

            if (myWebView != null) {
                myWebView.Dispose ();
                myWebView = null;
            }
        }
    }
}