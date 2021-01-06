using Android.App;
using Android.Util;
using Lion.XDroid;
using Firebase.Iid;
using Lion.XDroid.Libs;
using System.Collections.Generic;
using System;

namespace Lion.XDroid.Service
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class RefreshTokenService : FirebaseInstanceIdService
    {
        public override void OnTokenRefresh()
        {
            var _app_cache = new AppPreferences(this.ApplicationContext);
            var _user_token = _app_cache.UserTokenKey;

            if (String.IsNullOrEmpty(_user_token) == false)
            {
                var _refreshed_id = FirebaseInstanceId.Instance.Token;
                this.UpdateDeviceId(_user_token, _refreshed_id);
            }
        }

        private void UpdateDeviceId(string user_token, string device_id)
        {
            try
            {
                var parameter = new Dictionary<string, string>();
                {
                    parameter.Add("device_type", "A");
                    parameter.Add("device_id", device_id);
                }

                var api = new ApiAsnycTask(this, GetString(Resource.String.api_url) + "user/UpdateDeviceId", user_token);
                api.Execute(parameter);

                api.SendFinish += (s, e) =>
                {
                };
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);
            }
        }
    }
}