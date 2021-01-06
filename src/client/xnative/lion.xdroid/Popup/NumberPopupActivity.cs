using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Lion.XDroid.Control;
using Lion.XDroid.Libs;
using System.Collections.Generic;

namespace Lion.XDroid.Popup
{
    [Activity(Label = "NumberPopupActivity", Theme = "@android:style/Theme.Translucent.NoTitleBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class NumberPopupActivity : Activity
    {
        int selected_number;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_number_popup);

            this.InitData();
            this.InitEvent();
            this.InitSize();
        }

        private void InitSize()
        {
            var btn_ok = FindViewById<Button>(Resource.Id.btn_ok);
            var btn_cancel = FindViewById<Button>(Resource.Id.btn_cancel);

            btn_ok.LayoutParameters = AppCommon.SNG.ResizeDP(this, btn_ok.LayoutParameters);
            btn_cancel.LayoutParameters = AppCommon.SNG.ResizeDP(this, btn_cancel.LayoutParameters);
        }

        private void InitEvent()
        {
            var btn_ok = FindViewById<Button>(Resource.Id.btn_ok);
            btn_ok.Click += ListenerClick;
            btn_ok.Touch += ListenerTouch;

            var btn_cancel = FindViewById<Button>(Resource.Id.btn_cancel);
            btn_cancel.Click += ListenerClick;
            btn_cancel.Touch += ListenerTouch;
        }

        private void ListenerTouch(object sender, Android.Views.View.TouchEventArgs e)
        {
            var _view = (Button)sender;
            if (e.Event.Action == MotionEventActions.Down)
            {
                _view.Background.SetColorFilter(0x10000000);
                _view.Invalidate();
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                _view.Background.SetColorFilter(0x00000000);
                _view.Invalidate();
            }
            else if (e.Event.Action == MotionEventActions.Cancel)
            {
                _view.Background.SetColorFilter(0x00000000);
                _view.Invalidate();
            }

            e.Handled = false;
        }

        private void ListenerClick(object sender, System.EventArgs e)
        {
            var _view = (Button)sender;
            switch (_view.Id)
            {
                case Resource.Id.btn_ok:
                    this.Intent.PutExtra("number", selected_number);
                    SetResult(Result.Ok, this.Intent);
                    Finish();
                    break;
                case Resource.Id.btn_cancel:
                    SetResult(Result.Canceled, this.Intent);
                    Finish();
                    break;
            }
        }

        private void InitData()
        {
            var lv_popup = FindViewById<ListView>(Resource.Id.lv_popup);
            var min = this.Intent.GetIntExtra("min", 1);
            var max = this.Intent.GetIntExtra("max", 10);
            var reverse = this.Intent.GetBooleanExtra("reverse", true);

            var _number_list = new List<int>();
            if (reverse)
            {
                this.selected_number = max;
                for (int i = max; i >= min; i--)
                    _number_list.Add(i);
            }
            else
            {
                this.selected_number = min;
                for (int i = min; i <= max; i++)
                    _number_list.Add(i);
            }

            var adapter = new ListViewResultAdapter(this, Resource.Layout.listview_result_custom, _number_list);
            adapter.ChangePosition += (s, e) =>
            {
                this.selected_number = e.Position;
            };

            lv_popup.Adapter = adapter;
        }
    }
}