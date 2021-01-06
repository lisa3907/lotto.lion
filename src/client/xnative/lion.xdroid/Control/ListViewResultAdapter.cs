using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Lion.XDroid.Libs;
using System;
using System.Collections.Generic;

namespace Lion.XDroid.Control
{
    public class ListViewResultAdapter : ArrayAdapter
    {
        int resourceId;
        int selectedSequence = -1;
        Button preBtn;

        public event EventHandler<PositionEventArgs> ChangePosition;

        // ListViewBtnAdapter 생성자. 마지막에 ListBtnClickListener 추가.
        public ListViewResultAdapter(Context context, int resource, List<int> list) 
                : base(context, resource, list)
        {
            // resource id 값 복사. (super로 전달된 resource를 참조할 방법이 없음.)
            this.resourceId = resource;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var _position = position;
            var _pcontext = parent.Context;

            // 생성자로부터 저장된 resourceId(listview_btn_item)에 해당하는 Layout을 inflate하여 convertView 참조 획득.
            if (convertView == null)
            {
                var _layout_inflater = _pcontext.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                convertView = _layout_inflater.Inflate(this.resourceId, parent, false);
            }

            // 화면에 표시될 View(Layout이 inflate된)로부터 위젯에 대한 참조 획득
            var tv_result = convertView.FindViewById<TextView>(Resource.Id.tv_result);
            var btn_center = convertView.FindViewById<Button>(Resource.Id.btn_center);

            var sequence = (int)this.GetItem(position);
            btn_center.Tag = sequence;

            // Data Set(listViewItemList)에서 position에 위치한 데이터 참조 획득
            var value = sequence.ToString();
            {            // 아이템 내 각 위젯에 데이터 반영
                if (value.Equals("0"))
                    value = "Clear";

                tv_result.SetText(value);
            }

            if (this.selectedSequence == sequence)
                btn_center.Background.SetColorFilter(0x10000000);
            else
                btn_center.Background.SetColorFilter(0x00000000);

            btn_center.Invalidate();

            // button1 클릭 시 TextView(textView1)의 내용 변경.
            btn_center.Click -= Btn_Center_Click;
            btn_center.Click += Btn_Center_Click;

            btn_center.Touch -= Btn_Center_Touch;
            btn_center.Touch += Btn_Center_Touch;

            return convertView;
        }

        private void Btn_Center_Touch(object sender, View.TouchEventArgs e)
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

        private void Btn_Center_Click(object sender, EventArgs e)
        {
            var view = (Button)sender;
            if (preBtn != null)
            {
                preBtn.Background.SetColorFilter(0x00000000);
                preBtn.Invalidate();
            }

            view.Background.SetColorFilter(0x10000000);
            view.Invalidate();

            selectedSequence = (int)view.Tag;
            preBtn = view;

            ChangePosition?.Invoke(this, new PositionEventArgs((int)view.Tag));
        }
    }
}