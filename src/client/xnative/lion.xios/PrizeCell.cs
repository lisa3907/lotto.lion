using System;
using UIKit;

namespace Lion.XiOS
{
    public partial class PrizeCell : UITableViewCell
    {
        public PrizeCell(IntPtr handle) : base(handle)
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

        public UIKit.UILabel Label1 { get { return this.label1; } }
        public UIKit.UILabel Label2 { get { return this.label2; } }
        public UIKit.UILabel Label3 { get { return this.label3; } }
    }
}
