using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Java.Util;
using Lion.XDroid.Libs;
using Lion.XDroid.Popup;
using Lion.XDroid.Type;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lion.XDroid.Control
{
    public class ResultPagerFragment : Android.Support.V4.App.Fragment, IPagerFragment
    {
        private const int NUMBER_POPUP = 3;

        View rootView;

        int last_week_seqno = 0;
        NextWeekPrize week_prize = new NextWeekPrize();

        List<WinnerPrize> winner_list = new List<WinnerPrize>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            //Log.Debug(this.GetType().Name, $"OnCreate");
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //Log.Debug(this.GetType().Name, $"OnCreateView ({((ViewPager)container).CurrentItem})");

            this.rootView = inflater.Inflate(Resource.Layout.fragment_result, container, false);

            this.InitSize(this.rootView);
            this.InitEvent(this.rootView);
            this.InitData(this.rootView);

            return this.rootView;
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public void Initialize()
        {
            this.InitData(this.rootView);
        }

        private void InitSize(View view)
        {
            var iv_label1 = view.FindViewById<ImageView>(Resource.Id.iv_label1);
            var iv_lotto1 = view.FindViewById<ImageView>(Resource.Id.iv_lotto1);
            var iv_lotto2 = view.FindViewById<ImageView>(Resource.Id.iv_lotto2);
            var iv_lotto3 = view.FindViewById<ImageView>(Resource.Id.iv_lotto3);
            var iv_lotto4 = view.FindViewById<ImageView>(Resource.Id.iv_lotto4);
            var iv_lotto5 = view.FindViewById<ImageView>(Resource.Id.iv_lotto5);
            var iv_lotto6 = view.FindViewById<ImageView>(Resource.Id.iv_lotto6);
            var iv_lotto7 = view.FindViewById<ImageView>(Resource.Id.iv_lotto7);
            var iv_result1 = view.FindViewById<ImageView>(Resource.Id.iv_result1);
            var iv_result2 = view.FindViewById<ImageView>(Resource.Id.iv_result2);
            var iv_result3 = view.FindViewById<ImageView>(Resource.Id.iv_result3);
            var iv_result4 = view.FindViewById<ImageView>(Resource.Id.iv_result4);
            var iv_result5 = view.FindViewById<ImageView>(Resource.Id.iv_result5);
            var iv_line1 = view.FindViewById<ImageView>(Resource.Id.iv_line1);
            var iv_line2 = view.FindViewById<ImageView>(Resource.Id.iv_line2);
            var iv_line3 = view.FindViewById<ImageView>(Resource.Id.iv_line3);
            var iv_line4 = view.FindViewById<ImageView>(Resource.Id.iv_line4);
            var iv_line5 = view.FindViewById<ImageView>(Resource.Id.iv_line5);
            var iv_line6 = view.FindViewById<ImageView>(Resource.Id.iv_line6);
            var iv_line7 = view.FindViewById<ImageView>(Resource.Id.iv_line7);
            var iv_line8 = view.FindViewById<ImageView>(Resource.Id.iv_line8);
            iv_label1.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_label1.LayoutParameters);
            iv_lotto1.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto1.LayoutParameters);
            iv_lotto2.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto2.LayoutParameters);
            iv_lotto3.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto3.LayoutParameters);
            iv_lotto4.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto4.LayoutParameters);
            iv_lotto5.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto5.LayoutParameters);
            iv_lotto6.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto6.LayoutParameters);
            iv_lotto7.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_lotto7.LayoutParameters);
            iv_result1.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_result1.LayoutParameters);
            iv_result2.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_result2.LayoutParameters);
            iv_result3.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_result3.LayoutParameters);
            iv_result4.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_result4.LayoutParameters);
            iv_result5.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_result5.LayoutParameters);
            iv_line1.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_line1.LayoutParameters);
            iv_line2.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_line2.LayoutParameters);
            iv_line3.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_line3.LayoutParameters);
            iv_line4.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_line4.LayoutParameters);
            iv_line5.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_line5.LayoutParameters);
            iv_line6.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_line6.LayoutParameters);
            iv_line7.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_line7.LayoutParameters);
            iv_line8.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_line8.LayoutParameters);
        }

        private void InitEvent(View view)
        {
            var btn_sequence = view.FindViewById<Button>(Resource.Id.btn_sequence);
            btn_sequence.Click += (s, e) =>
            {
                var _view = (Button)s;
                switch (_view.Id)
                {
                    case Resource.Id.btn_sequence:
                        if (last_week_seqno > 1)
                        {
                            var intent = new Intent(this.Context, typeof(NumberPopupActivity));
                            intent.PutExtra("min", 1);
                            intent.PutExtra("max", last_week_seqno);
                            StartActivityForResult(intent, NUMBER_POPUP);
                        }
                        break;
                }
            };
        }

        private void InitData(View view)
        {
            var _guest_token = AppCommon.SNG.GetGuestToken(this.Context);

            this.GetThisWeekPrizeWithBuffer(_guest_token);
            this.GetPrizeBySeqNoWithBuffer(_guest_token, last_week_seqno);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            switch (requestCode)
            {
                case NUMBER_POPUP:
                    if (resultCode == (int)Result.Ok && data != null)
                    {
                        int sequence = data.GetIntExtra("number", 1);

                        var tv_sequence = this.rootView.FindViewById<TextView>(Resource.Id.tv_sequence);
                        tv_sequence.SetText(sequence + "회차");

                        var _guest_token = AppCommon.SNG.GetGuestToken(this.Context);
                        this.GetPrizeBySeqNoWithBuffer(_guest_token, sequence);
                    }
                    break;
            }
        }

        private void InitNextWeekPrize(View view, NextWeekPrize weekPrize)
        {
            var tv_predictDate = view.FindViewById<TextView>(Resource.Id.tv_predictDate);
            var tv_predictSequence = view.FindViewById<TextView>(Resource.Id.tv_predictSequence);
            var tv_predict = view.FindViewById<TextView>(Resource.Id.tv_predict);
            var tv_sales = view.FindViewById<TextView>(Resource.Id.tv_sales);

            var cd_predictDate = AppCommon.SNG.GetDate(weekPrize.IssueDate);
            {
                var date = "{0:0000}-{1:00}-{2:00}";
                tv_predictDate.SetText(String.Format(date,
                                            cd_predictDate.Get(CalendarField.Year),
                                            cd_predictDate.Get(CalendarField.Month) + 1,
                                            cd_predictDate.Get(CalendarField.DayOfMonth)
                                            )
                                      );
            }

            tv_predictSequence.SetText(weekPrize.SequenceNo + "회차");
            tv_predict.SetText($"{weekPrize.PredictAmount:#,##0}원");
            tv_sales.SetText($"{weekPrize.SalesAmount:#,##0}원");
        }

        private void InitWinnerBySeqNo(View view, WinnerPrize prize)
        {
            var tv_date = view.FindViewById<TextView>(Resource.Id.tv_date);
            var tv_sequence = view.FindViewById<TextView>(Resource.Id.tv_sequence);
            var iv_lotto1 = view.FindViewById<ImageView>(Resource.Id.iv_lotto1);
            var tv_lotto1 = view.FindViewById<TextView>(Resource.Id.tv_lotto1);
            var iv_lotto2 = view.FindViewById<ImageView>(Resource.Id.iv_lotto2);
            var tv_lotto2 = view.FindViewById<TextView>(Resource.Id.tv_lotto2);
            var iv_lotto3 = view.FindViewById<ImageView>(Resource.Id.iv_lotto3);
            var tv_lotto3 = view.FindViewById<TextView>(Resource.Id.tv_lotto3);
            var iv_lotto4 = view.FindViewById<ImageView>(Resource.Id.iv_lotto4);
            var tv_lotto4 = view.FindViewById<TextView>(Resource.Id.tv_lotto4);
            var iv_lotto5 = view.FindViewById<ImageView>(Resource.Id.iv_lotto5);
            var tv_lotto5 = view.FindViewById<TextView>(Resource.Id.tv_lotto5);
            var iv_lotto6 = view.FindViewById<ImageView>(Resource.Id.iv_lotto6);
            var tv_lotto6 = view.FindViewById<TextView>(Resource.Id.tv_lotto6);
            var iv_lotto7 = view.FindViewById<ImageView>(Resource.Id.iv_lotto7);
            var tv_lotto7 = view.FindViewById<TextView>(Resource.Id.tv_lotto7);
            var tv_count_result1 = view.FindViewById<TextView>(Resource.Id.tv_count_result1);
            var tv_amount_result1 = view.FindViewById<TextView>(Resource.Id.tv_amount_result1);
            var tv_count_result2 = view.FindViewById<TextView>(Resource.Id.tv_count_result2);
            var tv_amount_result2 = view.FindViewById<TextView>(Resource.Id.tv_amount_result2);
            var tv_count_result3 = view.FindViewById<TextView>(Resource.Id.tv_count_result3);
            var tv_amount_result3 = view.FindViewById<TextView>(Resource.Id.tv_amount_result3);
            var tv_count_result4 = view.FindViewById<TextView>(Resource.Id.tv_count_result4);
            var tv_amount_result4 = view.FindViewById<TextView>(Resource.Id.tv_amount_result4);
            var tv_count_result5 = view.FindViewById<TextView>(Resource.Id.tv_count_result5);
            var tv_amount_result5 = view.FindViewById<TextView>(Resource.Id.tv_amount_result5);

            var _predict_date = AppCommon.SNG.GetDate(prize.issueDate);
            {
                var date = "{0:0000}-{1:00}-{2:00}";
                tv_date.SetText(String.Format(date,
                                    _predict_date.Get(CalendarField.Year),
                                    _predict_date.Get(CalendarField.Month) + 1,
                                    _predict_date.Get(CalendarField.DayOfMonth)
                                    )
                               );
            }

            tv_sequence.SetText(prize.sequenceNo + "회차");

            tv_lotto1.SetText(prize.digit1.ToString());
            tv_lotto2.SetText(prize.digit2.ToString());
            tv_lotto3.SetText(prize.digit3.ToString());
            tv_lotto4.SetText(prize.digit4.ToString());
            tv_lotto5.SetText(prize.digit5.ToString());
            tv_lotto6.SetText(prize.digit6.ToString());
            tv_lotto7.SetText(prize.digit7.ToString());
            this.CheckLottoNum(prize.digit1, iv_lotto1);
            this.CheckLottoNum(prize.digit2, iv_lotto2);
            this.CheckLottoNum(prize.digit3, iv_lotto3);
            this.CheckLottoNum(prize.digit4, iv_lotto4);
            this.CheckLottoNum(prize.digit5, iv_lotto5);
            this.CheckLottoNum(prize.digit6, iv_lotto6);
            this.CheckLottoNum(prize.digit7, iv_lotto7);

            tv_count_result1.SetText($"{prize.count1:#,##0}명");
            tv_count_result2.SetText($"{prize.count2:#,##0}명");
            tv_count_result3.SetText($"{prize.count3:#,##0}명");
            tv_count_result4.SetText($"{prize.count4:#,##0}명");
            tv_count_result5.SetText($"{prize.count5:#,##0}명");

            tv_amount_result1.SetText($"{prize.amount1:#,##0}원");
            tv_amount_result2.SetText($"{prize.amount2:#,##0}원");
            tv_amount_result3.SetText($"{prize.amount3:#,##0}원");
            tv_amount_result4.SetText($"{prize.amount4:#,##0}원");
            tv_amount_result5.SetText($"{prize.amount5:#,##0}원");
        }

        private void CheckLottoNum(int num, ImageView iv)
        {
            if ((num / 40) == 1)
            {
                iv.SetImageResource(Resource.Drawable.circled_g);
            }
            else if ((num / 30) == 1)
            {
                iv.SetImageResource(Resource.Drawable.circled_k);
            }
            else if ((num / 20) == 1)
            {
                iv.SetImageResource(Resource.Drawable.circled_r);
            }
            else if ((num / 10) == 1)
            {
                iv.SetImageResource(Resource.Drawable.circled_b);
            }
            else
            {
                iv.SetImageResource(Resource.Drawable.circled_y);
            }
        }

        private void GetThisWeekPrizeWithBuffer(string guest_token)
        {
            if (week_prize.NextReadTime < DateTime.Now)
                GetThisWeekPrize(guest_token);
            else
                InitNextWeekPrize(rootView, week_prize);
        }

        private void GetThisWeekPrize(string guest_token)
        {
            try
            {
                this.Loading(true);

                var api = new ApiAsnycTask(this.Context, GetString(Resource.String.api_url) + "lotto/GetThisWeekPrize", guest_token);
                var json = api.Execute().Get().ToString();

                var _result = JsonConvert.DeserializeObject<ApiResult<NextWeekPrize>>(json);
                if (_result != null)
                {
                    var app_cache = new AppPreferences(this.Context);
                    app_cache.NextWeekSequenceNo = _result.result.SequenceNo;

                    last_week_seqno = _result.result.SequenceNo - 1;
                    if (_result.success == true)
                    {
                        week_prize = _result.result;
                        InitNextWeekPrize(rootView, _result.result);
                    }
                    else
                        AppDialog.SNG.Alert(this.Context, _result.message);
                }

                this.Loading(false);
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }
        }

        private void GetPrizeBySeqNoWithBuffer(string guest_token, int sequence_no)
        {
            if (winner_list.Count <= 0)
            {
                var app_cache = new AppPreferences(this.Context);
                var _cache_winner = app_cache.WinnerHistory;
                if (_cache_winner != null)
                    winner_list = _cache_winner;
            }

            var _winner = winner_list
                                .Where(w => w.sequenceNo == sequence_no)
                                .SingleOrDefault();

            if (_winner == null)
                GetPrizeBySeqNo(guest_token, sequence_no);
            else
                InitWinnerBySeqNo(rootView, _winner);
        }

        private void GetPrizeBySeqNo(string guest_token, int sequence_no)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("sequence_no", sequence_no.ToString());
                }

                var api = new ApiAsnycTask(this.Context, GetString(Resource.String.api_url) + "lotto/GetPrizeBySeqNo", guest_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<WinnerPrize>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            winner_list.Add(_result.result);

                            var app_cache = new AppPreferences(this.Context);
                            app_cache.WinnerHistory = winner_list;

                            InitWinnerBySeqNo(rootView, _result.result);
                        }
                        else
                            AppDialog.SNG.Alert(this.Context, _result.message);
                    }

                    this.Loading(false);
                };
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }
        }

        private void Loading(bool view)
        {
            var btn_loading = this.rootView.FindViewById<Button>(Resource.Id.btn_loading);
            var pb_loading = this.rootView.FindViewById<ProgressBar>(Resource.Id.pb_loading);
            if (view)
            {
                btn_loading.Visibility = ViewStates.Visible;
                pb_loading.Visibility = ViewStates.Visible;
            }
            else
            {
                btn_loading.Visibility = ViewStates.Invisible;
                pb_loading.Visibility = ViewStates.Invisible;
            }
        }
    }
}