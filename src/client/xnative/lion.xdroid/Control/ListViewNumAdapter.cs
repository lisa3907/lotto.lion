using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Lion.XDroid.Libs;
using Lion.XDroid.Type;
using System.Collections.Generic;

namespace Lion.XDroid.Control
{
    public class ListViewNumAdapter : ArrayAdapter
    {
        int resourceId;

        public ListViewNumAdapter(Context context, int resource, List<UserChoice> list)
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
                this.InitSize(convertView);
            }

            // 화면에 표시될 View(Layout이 inflate된)로부터 위젯에 대한 참조 획득
            var iv_lotto1 = convertView.FindViewById<ImageView>(Resource.Id.iv_lotto1);
            var tv_lotto1 = convertView.FindViewById<TextView>(Resource.Id.tv_lotto1);
            var iv_lotto2 = convertView.FindViewById<ImageView>(Resource.Id.iv_lotto2);
            var tv_lotto2 = convertView.FindViewById<TextView>(Resource.Id.tv_lotto2);
            var iv_lotto3 = convertView.FindViewById<ImageView>(Resource.Id.iv_lotto3);
            var tv_lotto3 = convertView.FindViewById<TextView>(Resource.Id.tv_lotto3);
            var iv_lotto4 = convertView.FindViewById<ImageView>(Resource.Id.iv_lotto4);
            var tv_lotto4 = convertView.FindViewById<TextView>(Resource.Id.tv_lotto4);
            var iv_lotto5 = convertView.FindViewById<ImageView>(Resource.Id.iv_lotto5);
            var tv_lotto5 = convertView.FindViewById<TextView>(Resource.Id.tv_lotto5);
            var iv_lotto6 = convertView.FindViewById<ImageView>(Resource.Id.iv_lotto6);
            var tv_lotto6 = convertView.FindViewById<TextView>(Resource.Id.tv_lotto6);
            var tv_result = convertView.FindViewById<TextView>(Resource.Id.tv_result);
            var tv_amount = convertView.FindViewById<TextView>(Resource.Id.tv_amount);

            var iv_cover = convertView.FindViewById<ImageView>(Resource.Id.iv_cover);

            var _lotto = this.GetItem(position).Cast<UserChoice>();
            tv_lotto1.SetText(_lotto.digit1.ToString());
            tv_lotto2.SetText(_lotto.digit2.ToString());
            tv_lotto3.SetText(_lotto.digit3.ToString());
            tv_lotto4.SetText(_lotto.digit4.ToString());
            tv_lotto5.SetText(_lotto.digit5.ToString());
            tv_lotto6.SetText(_lotto.digit6.ToString());
            this.CheckLottoNumber(_lotto.digit1, iv_lotto1);
            this.CheckLottoNumber(_lotto.digit2, iv_lotto2);
            this.CheckLottoNumber(_lotto.digit3, iv_lotto3);
            this.CheckLottoNumber(_lotto.digit4, iv_lotto4);
            this.CheckLottoNumber(_lotto.digit5, iv_lotto5);
            this.CheckLottoNumber(_lotto.digit6, iv_lotto6);

            if (_lotto.ranking == 0)
            {
                iv_cover.Visibility = ViewStates.Invisible;
                tv_result.SetText("당첨결과: -");
                tv_amount.SetText("당첨금액: -");
            }
            else if (_lotto.ranking == 6)
            {
                iv_cover.Visibility = ViewStates.Visible;
                tv_result.SetText("당첨결과: X");
                tv_amount.SetText("당첨금액: 0원");
            }
            else
            {
                iv_cover.Visibility = ViewStates.Invisible;
                tv_result.SetText("당첨결과: " + _lotto.ranking + "등");
                tv_amount.SetText("당첨금액: " + $"{_lotto.amount:#,##0}원");
            }

            return convertView;
        }

        private void InitSize(View view)
        {
            var iv_lotto1 = view.FindViewById<ImageView>(Resource.Id.iv_lotto1);
            var iv_lotto2 = view.FindViewById<ImageView>(Resource.Id.iv_lotto2);
            var iv_lotto3 = view.FindViewById<ImageView>(Resource.Id.iv_lotto3);
            var iv_lotto4 = view.FindViewById<ImageView>(Resource.Id.iv_lotto4);
            var iv_lotto5 = view.FindViewById<ImageView>(Resource.Id.iv_lotto5);
            var iv_lotto6 = view.FindViewById<ImageView>(Resource.Id.iv_lotto6);
            iv_lotto1.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto1.LayoutParameters);
            iv_lotto2.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto2.LayoutParameters);
            iv_lotto3.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto3.LayoutParameters);
            iv_lotto4.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto4.LayoutParameters);
            iv_lotto5.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto5.LayoutParameters);
            iv_lotto6.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto6.LayoutParameters);
        }

        private void CheckLottoNumber(short number, ImageView iv)
        {
            if ((number / 40) == 1)
            {
                iv.SetImageResource(Resource.Drawable.circled_g);
            }
            else if ((number / 30) == 1)
            {
                iv.SetImageResource(Resource.Drawable.circled_k);
            }
            else if ((number / 20) == 1)
            {
                iv.SetImageResource(Resource.Drawable.circled_r);
            }
            else if ((number / 10) == 1)
            {
                iv.SetImageResource(Resource.Drawable.circled_b);
            }
            else
            {
                iv.SetImageResource(Resource.Drawable.circled_y);
            }
        }
    }
}