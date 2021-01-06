using System;
using System.Collections.Generic;
using Android.Content;
using Android.Preferences;
using Lion.XDroid.Type;
using Newtonsoft.Json;

namespace Lion.XDroid.Libs
{
    public class AppPreferences
    {
        private ISharedPreferences appSharedPrefs;
        private ISharedPreferencesEditor appPrefsEditor;
        private Context appContext;

        private string WINNER_HISTORY = "WINNER_HISTORY";
        private string USER_CHOICED_NUMBERS = "USER_CHOICED_NUMBERS";
        private string DEVICE_UNIQUEUE_ID = "DEVICE_UNIQUEUE_ID";
        private string SAVE_LOGINID_CHECK = "SAVE_LOGINID_CHECK";
        private string SAVE_PASSWORD_CHECK = "SAVE_PASSWORD_CHECK";
        private string USER_LOGIN_ID = "USER_LOGIN_ID";
        private string USER_PASSWORD = "USER_PASSWORD";
        private string USER_TOKEN_KEY = "USER_TOKEN_KEY";
        private string GUEST_TOKEN_KEY = "GUEST_TOKEN_KEY";
        private string NEXT_WEEK_SEQUENCE_NO = "NEXT_WEEK_SEQUENCE_NO";

        public AppPreferences(Context context)
        {
            this.appContext = context;

            appSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(appContext);
            appPrefsEditor = appSharedPrefs.Edit();
        }

        public List<WinnerPrize> WinnerHistory
        {
            get
            {
                var _get_seqnos = appSharedPrefs.GetString(this.WINNER_HISTORY, "");
                return JsonConvert.DeserializeObject<List<WinnerPrize>>(_get_seqnos);
            }
            set
            {
                var _put_seqnos = JsonConvert.SerializeObject(value);

                appPrefsEditor.PutString(this.WINNER_HISTORY, _put_seqnos);
                appPrefsEditor.Commit();
            }
        }

        public List<UserChoices> UserChoiceNumbers
        {
            get
            {
                var _get_choices = appSharedPrefs.GetString(this.USER_CHOICED_NUMBERS, "");
                return JsonConvert.DeserializeObject<List<UserChoices>>(_get_choices);
            }
            set
            {
                var _put_choices = JsonConvert.SerializeObject(value);

                appPrefsEditor.PutString(this.USER_CHOICED_NUMBERS, _put_choices);
                appPrefsEditor.Commit();
            }
        }

        public bool SaveLoginIdCheck
        {
            get
            {
                return appSharedPrefs.GetBoolean(this.SAVE_LOGINID_CHECK, true);
            }
            set
            {
                appPrefsEditor.PutBoolean(this.SAVE_LOGINID_CHECK, value);
                appPrefsEditor.Commit();
            }
        }

        public bool PutSaveLoginIdCheck(bool save_loginid_check)
        {
            appPrefsEditor.PutBoolean(this.SAVE_LOGINID_CHECK, save_loginid_check);
            return appPrefsEditor.Commit();
        }

        public bool GetSaveLoginIdCheck()
        {
            return appSharedPrefs.GetBoolean(this.SAVE_LOGINID_CHECK, false);
        }

        public bool SavePasswordCheck
        {
            get
            {
                return appSharedPrefs.GetBoolean(this.SAVE_PASSWORD_CHECK, true);
            }
            set
            {
                appPrefsEditor.PutBoolean(this.SAVE_PASSWORD_CHECK, value);
                appPrefsEditor.Commit();
            }
        }

        public bool PutSavePasswordCheck(bool save_password_check)
        {
            appPrefsEditor.PutBoolean(this.SAVE_PASSWORD_CHECK, save_password_check);
            return appPrefsEditor.Commit();
        }

        public bool GetSavePasswordCheck()
        {
            return appSharedPrefs.GetBoolean(this.SAVE_PASSWORD_CHECK, false);
        }

        public string UserTokenKey
        {
            get
            {
                return appSharedPrefs.GetString(this.USER_TOKEN_KEY, "");
            }
            set
            {
                appPrefsEditor.PutString(this.USER_TOKEN_KEY, value);
                appPrefsEditor.Commit();
            }
        }

        public bool PutUserTokenKey(string user_token)
        {
            appPrefsEditor.PutString(this.USER_TOKEN_KEY, user_token);
            return appPrefsEditor.Commit();
        }

        public string GetUserTokenKey()
        {
            return appSharedPrefs.GetString(this.USER_TOKEN_KEY, "");
        }

        public string GuestTokenKey
        {
            get
            {
                return appSharedPrefs.GetString(this.GUEST_TOKEN_KEY, "");
            }
            set
            {
                appPrefsEditor.PutString(this.GUEST_TOKEN_KEY, value);
                appPrefsEditor.Commit();
            }
        }

        public bool PutGuestTokenKey(string guest_token)
        {
            appPrefsEditor.PutString(this.GUEST_TOKEN_KEY, guest_token);
            return appPrefsEditor.Commit();
        }

        public string GetGuestTokenKey()
        {
            return appSharedPrefs.GetString(this.GUEST_TOKEN_KEY, "");
        }

        public string UserLoginId
        {
            get
            {
                return appSharedPrefs.GetString(this.USER_LOGIN_ID, "");
            }
            set
            {
                appPrefsEditor.PutString(this.USER_LOGIN_ID, value);
                appPrefsEditor.Commit();
            }
        }

        public bool PutUserLoginId(string login_id)
        {
            appPrefsEditor.PutString(this.USER_LOGIN_ID, login_id);
            return appPrefsEditor.Commit();
        }

        public string GetUserLoginId()
        {
            return appSharedPrefs.GetString(this.USER_LOGIN_ID, "");
        }

        public string UserPassword
        {
            get
            {
                return appSharedPrefs.GetString(this.USER_PASSWORD, "");
            }
            set
            {
                appPrefsEditor.PutString(this.USER_PASSWORD, value);
                appPrefsEditor.Commit();
            }
        }

        public bool PutUserPassword(string password)
        {
            appPrefsEditor.PutString(this.USER_PASSWORD, password);
            return appPrefsEditor.Commit();
        }

        public string GetUserPassword()
        {
            return appSharedPrefs.GetString(this.USER_PASSWORD, "");
        }

        public int NextWeekSequenceNo
        {
            get
            {
                return appSharedPrefs.GetInt(this.NEXT_WEEK_SEQUENCE_NO, 1);
            }
            set
            {
                appPrefsEditor.PutInt(this.NEXT_WEEK_SEQUENCE_NO, value);
                appPrefsEditor.Commit();
            }
        }

        public bool PutNextWeekSequenceNo(int sequence_no)
        {
            appPrefsEditor.PutInt(this.NEXT_WEEK_SEQUENCE_NO, sequence_no);
            return appPrefsEditor.Commit();
        }

        public int GetNextWeekSequenceNo()
        {
            return appSharedPrefs.GetInt(this.NEXT_WEEK_SEQUENCE_NO, 1);
        }

        public string DeviceId
        {
            get
            {
                return appSharedPrefs.GetString(this.DEVICE_UNIQUEUE_ID, "");
            }
            set
            {
                appPrefsEditor.PutString(this.DEVICE_UNIQUEUE_ID, value);
                appPrefsEditor.Commit();
            }
        }

        public string GetDeviceId()
        {
            var _device_id = this.DeviceId;

            if (String.IsNullOrEmpty(_device_id) == true)
            {
                _device_id = AppCommon.SNG.GetUniqueueId(appContext).ToString();
                this.DeviceId = _device_id;
            }

            return _device_id;
        }
    }
}