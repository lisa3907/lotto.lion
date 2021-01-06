using System;
using UIKit;

namespace Lion.XiOS
{
    /// <summary>
    /// https://components.xamarin.com/gettingstarted/firebaseiosadmob
    /// </summary>
    public partial class AdsViewCell : UITableViewCell
    {
        public AdsViewCell(IntPtr handle) : base(handle)
        {
        }
        public void SetSelectedAnimated(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
        }
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
