using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Util;
using Android.Views;
using Lion.XDroid;
using Java.Text;
using Java.Util;
using Lion.XDroid.Type;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace Lion.XDroid.Libs
{
    public class AppCommon
    {
        private static AppCommon sigleton = null;

        public static AppCommon SNG
        {
            get
            {
                if (sigleton == null)
                    sigleton = new AppCommon();

                return sigleton;
            }
        }

        public UUID GetUniqueueId(Context context)
        {
            var _result = (UUID)null;

            try
            {
                var _android_id = Android.Provider.Settings.Secure.GetString(
                                context.ContentResolver, Android.Provider.Settings.Secure.AndroidId
                            );

                if (_android_id == null)
                {
                    var _telephony_id = "";

                    var _telephony_service = context.GetSystemService(Context.TelephonyService).JavaCast<TelephonyManager>();
                    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        // TODO: Some phones has more than 1 SIM card or may not have a SIM card inserted at all
                        _telephony_id = _telephony_service.GetMeid(0);
                    else
#pragma warning disable CS0618 // Type or member is obsolete
                        _telephony_id = _telephony_service.DeviceId;
#pragma warning restore CS0618 // Type or member is obsolete

                    if (_telephony_id != null)
                        _result = UUID.NameUUIDFromBytes(Encoding.UTF8.GetBytes(_telephony_id));
                    else
                        _result = UUID.RandomUUID();
                }
                else
                {
                    _result = UUID.NameUUIDFromBytes(Encoding.UTF8.GetBytes(_android_id));
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }

            return _result;
        }

        public string GetDate(Calendar p_date)
        {
            var sdFormat = new SimpleDateFormat("yyyy-MM-dd\'T\'HH:mm:ss");
            return sdFormat.Format(p_date.Time);
        }

        public Calendar GetDate(DateTime p_date)
        {
            var _result = Calendar.Instance;

            _result.Set(p_date.Year, p_date.Month - 1, p_date.Day, p_date.Hour, p_date.Minute, p_date.Second);

            return _result;
        }

        public Calendar GetDate(string p_date)
        {
            var _result = Calendar.Instance;

            try
            {
                var sdFormat = new SimpleDateFormat("yyyy-MM-dd\'T\'HH:mm:ss");
                _result.Time = sdFormat.Parse(p_date);
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }

            return _result;
        }

        public bool CheckExpired(Context context, string token)
        {
            var _result = true;

            try
            {
                if (String.IsNullOrEmpty(token) == false)
                {
                    var _jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(token);
                    var _exp = _jwt.Claims.First(claim => claim.Type == "exp").Value;

                    var _value = Convert.ToInt64(_exp) * 1000;
                    //Log.Debug(this.GetType().Name, $"{(_value - Calendar.Instance.TimeInMillis) / 1000}secs");

                    _result = Calendar.Instance.TimeInMillis > _value;
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }

            return _result;
        }

        public void JumpLoginActivity(Context context)
        {
            var _intent = new Intent(context, typeof(LoginActivity));
            _intent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(_intent);
        }

        public string GetStringComma(string str)
        {
            var _result = "";

            if (str.Length > 0)
            {
                var value = Java.Lang.Double.ParseDouble(str);
                var format = new DecimalFormat("###,###");

                _result = format.Format(value);
            }

            return _result;
        }

        public ViewGroup.LayoutParams ResizeDP(Context context, ViewGroup.LayoutParams layout)
        {
            var metrics = new DisplayMetrics();

            var _windows_manager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            _windows_manager.DefaultDisplay.GetMetrics(metrics);

            layout.Width = (int)((layout.Width * Java.Lang.Math.Round(metrics.Density)) / metrics.Density);
            layout.Height = (int)((layout.Height * Java.Lang.Math.Round(metrics.Density)) / metrics.Density);
            if (this.IsTablet(context))
            {
                layout.Width = (int)(layout.Width * (metrics.WidthPixels / 720));
                layout.Height = (int)(layout.Height * (metrics.HeightPixels / 1280));
            }

            return layout;
        }

        public bool IsTablet(Context context)
        {
            var _result = false;

            var metrics = new DisplayMetrics();

            var _windows_manager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            _windows_manager.DefaultDisplay.GetMetrics(metrics);

            var yInches = (metrics.HeightPixels / metrics.Ydpi);
            var xInches = (metrics.WidthPixels / metrics.Xdpi);

            var diagonalInches = Java.Lang.Math.Sqrt(((xInches * xInches) + (yInches * yInches)));
            if (diagonalInches >= 6.5)
                _result = true;     //  6.5inch device or bigger
            else
                _result = false;    //  smaller device

            return _result;
        }

        public string GetGuestToken(Context context)
        {
            var _app_cache = new AppPreferences(context);

            var _result = _app_cache.GuestTokenKey;
            if (this.CheckExpired(context, _result) == true)
            {
                var _new_token = this.GetGuestTokenApi(context, _app_cache);
                if (_new_token != null)
                    _result = _new_token.result;
            }

            return _result;
        }

        public string DoDayOfWeek(int dow)
        {
            string result = "";

            switch (dow)
            {
                case 1:
                    result = "일";
                    break;
                case 2:
                    result = "월";
                    break;
                case 3:
                    result = "화";
                    break;
                case 4:
                    result = "수";
                    break;
                case 5:
                    result = "목";
                    break;
                case 6:
                    result = "금";
                    break;
                case 7:
                    result = "토";
                    break;
            }

            return result;
        }

        public void ResetBadge(Context context)
        {
            var intent = new Intent("android.intent.action.BADGE_COUNT_UPDATE");

            // 패키지 네임과 클래스 네임 설정
            intent.PutExtra("badge_count_package_name", "Lion.XDroid");
            intent.PutExtra("badge_count_class_name", "Lion.XDroid.MainActivity");

            // 업데이트 카운트
            intent.PutExtra("badge_count", 0);
            context.SendBroadcast(intent);
        }

        private ApiResult<string> GetGuestTokenApi(Context context, AppPreferences app_cache)
        {
            var _result = (ApiResult<string>)null;

            try
            {
                var _api = new ApiAsnycTask(context, context.GetString(Resource.String.api_url) + "user/getguesttoken");
                var json = _api.Execute().Get().ToString();

                _result = JsonConvert.DeserializeObject<ApiResult<string>>(json);
                if (_result.success == true)
                    app_cache.GuestTokenKey = _result.result;
                else
                    AppDialog.SNG.Alert(context, _result.message);
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }

            return _result;
        }
    }
}