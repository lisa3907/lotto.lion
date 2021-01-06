using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Java.Util;
using Lion.XDroid.Libs;
using Lion.XDroid.Type;
using System.Collections.Generic;
using System;

namespace Lion.XDroid.Control
{
    public class ListViewPushAdapter : ArrayAdapter
    {

        int resourceId;

        public ListViewPushAdapter(Context context, int resource, List<PushMessage> list) :
                base(context, resource, list)
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
            var tv_date = convertView.FindViewById<TextView>(Resource.Id.tv_date);
            var tv_message = convertView.FindViewById<TextView>(Resource.Id.tv_message);
            var btn_center = convertView.FindViewById<Button>(Resource.Id.btn_center);
            var message = this.GetItem(position).Cast<PushMessage>();

            var cd_date = AppCommon.SNG.GetDate(message.notifyTime);
            {
                var date = "{0:0000}.{1:00}.{2:00} ({3})";
                tv_date.SetText(String.Format(date, 
                                    cd_date.Get(CalendarField.Year), cd_date.Get(CalendarField.Month) + 1,
                                    cd_date.Get(CalendarField.DayOfMonth),
                                    AppCommon.SNG.DoDayOfWeek(cd_date.Get(CalendarField.DayOfWeek))
                                    )
                                );
            }

            tv_message.SetText(message.message);

            if (message.isRead)
            {
                btn_center.Background.SetColorFilter(0x10000000);
            }
            else
            {
                btn_center.Background.SetColorFilter(0x00000000);
            }

            return convertView;
        }
    }
}