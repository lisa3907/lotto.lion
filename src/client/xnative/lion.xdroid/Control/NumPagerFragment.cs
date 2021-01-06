using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Lion.XDroid;
using Lion.XDroid.Libs;
using Lion.XDroid.Popup;
using Lion.XDroid.Type;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Lion.XDroid.Control
{
    public class NumPagerFragment : Android.Support.V4.App.Fragment, IPagerFragment
    {
        private const int MAIL_POPUP = 4;

        View rootView;
        int user_curr_seqno;
        ListViewNumAdapter adapter;

        int user_last_seqno;
        List<UserChoices> choice_list;
        int user_first_seqno;
        List<TKeyValues> user_numbers;

        public NumPagerFragment()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            //Log.Debug(this.GetType().Name, $"OnCreate");
            base.OnCreate(savedInstanceState);

            this.user_first_seqno = 1;
            this.user_last_seqno = 1;

            this.choice_list = new List<UserChoices>();
            this.user_numbers = new List<TKeyValues>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //Log.Debug(this.GetType().Name, $"OnCreateView ({((ViewPager)container).CurrentItem})");

            this.rootView = inflater.Inflate(Resource.Layout.fragment_num, container, false);

            this.adapter = new ListViewNumAdapter(this.Context, Resource.Layout.listview_num_custom, new List<UserChoice>());
            {
                var lv_lotto = this.rootView.FindViewById<ListView>(Resource.Id.lv_lotto);
                lv_lotto.Adapter = this.adapter;
            }

            this.InitSize(this.rootView);
            this.InitEvent(this.rootView);
            this.InitData(this.rootView);

            return this.rootView;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            switch (requestCode)
            {
                case MAIL_POPUP:
                    if (resultCode == (int)Result.Ok)
                    {
                        var app_cache = new AppPreferences(this.Context);
                        var _token = app_cache.UserTokenKey;
                        SendChoicedNumbers(_token);
                    }
                    break;
            }
        }

        public void Initialize()
        {
            this.InitData(this.rootView);
        }

        private void InitSize(View view)
        {
            var iv_prefix = view.FindViewById<ImageView>(Resource.Id.iv_pre);
            var iv_postfix = view.FindViewById<ImageView>(Resource.Id.iv_post);
            iv_prefix.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_prefix.LayoutParameters);
            iv_postfix.LayoutParameters = AppCommon.SNG.ResizeDP(this.Context, iv_postfix.LayoutParameters);
        }

        private void InitEvent(View view)
        {
            var iv_prefix = view.FindViewById<ImageView>(Resource.Id.iv_pre);
            iv_prefix.Touch += ListenerTouch;
            iv_prefix.Click += ListenerClick;

            var iv_postfix = view.FindViewById<ImageView>(Resource.Id.iv_post);
            iv_postfix.Touch += ListenerTouch;
            iv_postfix.Click += ListenerClick;

            var iv_mail = view.FindViewById<ImageView>(Resource.Id.iv_mail);
            iv_mail.Touch += ListenerTouch;

            iv_mail.Click += (sender, e) =>
            {
                var intent = new Intent(this.Context, typeof(MailPopupActivity));
                StartActivityForResult(intent, MAIL_POPUP);
            };
        }

        private void InitData(View view)
        {
            if (view == null)
                return;

            var app_cache = new AppPreferences(this.Context);
            var _token = app_cache.UserTokenKey;
            var _login_id = app_cache.UserLoginId;

            // first_seqno
            {
                var _user_number = user_numbers
                                    .Where(n => n.loginId == _login_id)
                                    .SingleOrDefault();

                if (_user_number == null)
                {
                    user_first_seqno = 1;
                    GetUserSequenceNos(_token, _login_id);
                }
                else
                {
                    user_first_seqno = _user_number.numbers.Min(n => n.key);
                }
            }

            // last seqno
            {
                this.user_last_seqno = app_cache.NextWeekSequenceNo;
                this.user_curr_seqno = this.user_last_seqno;

                this.NavigationToggle(view);

                GetUserChoicesWithBuffer(_token, _login_id);
            }
        }

        private void ListenerClick(object sender, System.EventArgs e)
        {
            var app_cache = new AppPreferences(this.Context);
            var _token = app_cache.UserTokenKey;
            var _login_id = app_cache.UserLoginId;

            var _view = (ImageView)sender;
            switch (_view.Id)
            {
                case Resource.Id.iv_pre:
                    user_curr_seqno--;
                    if (user_curr_seqno < user_first_seqno)
                        user_curr_seqno = user_first_seqno;
                    break;

                case Resource.Id.iv_post:
                    user_curr_seqno++;
                    if (user_curr_seqno > user_last_seqno)
                        user_curr_seqno = user_last_seqno;
                    break;
            }

            GetUserChoicesWithBuffer(_token, _login_id);

            NavigationToggle(this.rootView);
        }

        private void ListenerTouch(object sender, View.TouchEventArgs e)
        {
            var _view = (ImageView)sender;

            if (e.Event.Action == MotionEventActions.Down)
            {
                _view.SetColorFilter(0xAA111111);
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                _view.SetColorFilter(0x00000000);
            }

            e.Handled = false;
        }

        private void GetUserChoicesWithBuffer(string user_token, string login_id)
        {
            if (choice_list.Count <= 0)
            {
                var app_cache = new AppPreferences(this.Context);
                var _cache_choice = app_cache.UserChoiceNumbers;
                if (_cache_choice != null)
                    choice_list = _cache_choice
                                    .Where(c => c.sequenceNo != this.user_last_seqno)
                                    .ToList();
            }

            var _choice = choice_list
                                .Where(c => c.sequenceNo == this.user_curr_seqno && c.loginId == login_id)
                                .SingleOrDefault();

            var tv_noting = this.rootView.FindViewById<TextView>(Resource.Id.tv_noting);
            tv_noting.Visibility = ViewStates.Invisible;

            if (_choice != null)
            {
                if (this.user_curr_seqno != this.user_last_seqno && _choice.choice.Where(c => c.ranking <= 0).Count() > 0)
                    GetUserChoices(user_token, login_id);
                else
                {
                    this.adapter.Clear();

                    if (_choice.choice.Count <= 0)
                        tv_noting.Visibility = ViewStates.Visible;
                    else
                        this.adapter.AddAll(_choice.choice);

                    this.adapter.NotifyDataSetChanged();
                }
            }
            else
            {
                GetUserChoices(user_token, login_id);
            }
        }

        private void GetUserChoices(string user_token, string login_id)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("sequence_no", this.user_curr_seqno.ToString());
                }

                var api = new ApiAsnycTask(this.Context, GetString(Resource.String.api_url) + "lotto/GetUserChoices", user_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<List<UserChoice>>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            this.adapter.Clear();
                            this.adapter.AddAll(_result.result);
                            this.adapter.NotifyDataSetChanged();

                            var _choice = new UserChoices()
                            {
                                sequenceNo = this.user_curr_seqno,
                                loginId = login_id,
                                choice = _result.result
                            };

                            choice_list.RemoveAll(x => x.loginId == login_id && x.sequenceNo == this.user_curr_seqno);
                            choice_list.Add(_choice);

                            var app_cache = new AppPreferences(this.Context);
                            app_cache.UserChoiceNumbers = choice_list;

                            var tv_noting = this.rootView.FindViewById<TextView>(Resource.Id.tv_noting);
                            if (_result.result.Count == 0)
                                tv_noting.Visibility = ViewStates.Visible;
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

        private void SendChoicedNumbers(string user_token)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();
                {
                    _parameter.Add("sequence_no", this.user_curr_seqno.ToString());
                }

                var api = new ApiAsnycTask(this.Context, GetString(Resource.String.api_url) + "lotto/SendChoicedNumbers", user_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<string>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                            AppDialog.SNG.Alert(this.Context, "메일이 발송되었습니다.");
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

        private void GetUserSequenceNos(string user_token, string login_id)
        {
            try
            {
                this.Loading(true);

                var _parameter = new Dictionary<string, string>();

                var api = new ApiAsnycTask(this.Context, GetString(Resource.String.api_url) + "lotto/GetUserSequenceNos", user_token);
                api.Execute(_parameter);

                api.SendFinish += (s, e) =>
                {
                    var _result = JsonConvert.DeserializeObject<ApiResult<List<TKeyValue>>>(e.Json);
                    if (_result != null)
                    {
                        if (_result.success == true)
                        {
                            var _new_item = new TKeyValues()
                            {
                                loginId = login_id,
                                numbers = _result.result
                            };

                            user_numbers.RemoveAll(x => x.loginId == login_id);
                            user_numbers.Add(_new_item);

                            user_first_seqno = _result.result.Min(n => n.key);
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

        private void NavigationToggle(View view)
        {
            var tv_seqnece = view.FindViewById<TextView>(Resource.Id.tv_sequence);
            tv_seqnece.SetText(this.user_curr_seqno + "회차 " + (user_curr_seqno == user_last_seqno ? "(예상)" : "(결과)"));

            var iv_prefix = view.FindViewById<ImageView>(Resource.Id.iv_pre);
            var iv_postfix = view.FindViewById<ImageView>(Resource.Id.iv_post);

            if (this.user_curr_seqno <= user_first_seqno)
            {
                iv_prefix.Visibility = ViewStates.Invisible;
                iv_postfix.Visibility = ViewStates.Visible;
            }
            else if (this.user_curr_seqno >= this.user_last_seqno)
            {
                iv_prefix.Visibility = ViewStates.Visible;
                iv_postfix.Visibility = ViewStates.Invisible;
            }
            else
            {
                iv_prefix.Visibility = ViewStates.Visible;
                iv_postfix.Visibility = ViewStates.Visible;
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