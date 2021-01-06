using System.Collections.Generic;
using Android.Support.V4.App;
using Java.Lang;

namespace Lion.XDroid.Control
{
    public class MainPagerAdapter : FragmentPagerAdapter
    {
        private List<Fragment> fragment_list = new List<Fragment>();
        private List<string> title_list = new List<string>();
        private List<bool> anonymous_list = new List<bool>();

        public MainPagerAdapter(FragmentManager fm) 
                : base(fm)
        {
        }

        public void AddFragment(Fragment fragment, string title, bool is_anonymous)
        {
            this.fragment_list.Add(fragment);
            this.title_list.Add(title);
            this.anonymous_list.Add(is_anonymous);
        }

        public bool IsAnonymous(int position)
        {
            return anonymous_list[position];
        }

        public override Fragment GetItem(int position)
        {
            return this.fragment_list[position];
        }

        public override int Count
        {
            get
            {
                return this.fragment_list.Count;
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(title_list[position]);
        }
    }
}