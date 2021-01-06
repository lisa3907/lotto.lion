using System;
using UIKit;

namespace Lion.XiOS
{
    public partial class NumbersCell : UITableViewCell
    {
        public NumbersCell(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
        }

        public UIKit.UIImageView ImgNum1 { get { return imgNum1; } }
        public UIKit.UIImageView ImgNum2 { get { return imgNum2; } }
        public UIKit.UIImageView ImgNum3 { get { return imgNum3; } }
        public UIKit.UIImageView ImgNum4 { get { return imgNum4; } }
        public UIKit.UIImageView ImgNum5 { get { return imgNum5; } }
        public UIKit.UIImageView ImgNum6 { get { return imgNum6; } }

        public UIKit.UILabel Label1 { get { return label1; } }
        public UIKit.UILabel Label2 { get { return label2; } }

        public UIKit.UILabel Num1 { get { return num1; } }
        public UIKit.UILabel Num2 { get { return num2; } }
        public UIKit.UILabel Num3 { get { return num3; } }
        public UIKit.UILabel Num4 { get { return num4; } }
        public UIKit.UILabel Num5 { get { return num5; } }
        public UIKit.UILabel Num6 { get { return num6; } }

        public UIKit.UIView WhiteView { get { return whiteView; } }
    }
}
