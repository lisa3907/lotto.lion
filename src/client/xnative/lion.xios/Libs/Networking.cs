using Foundation;
using Lion.XIOS.Type;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UIKit;

namespace Lion.XiOS.Libs
{
    public partial class Constants
    {
        public const string LionApiUrl = "http://lottoapi.odinsoftware.co.kr";
    }

    public class Networking
    {
        private NSData ApiCallAsync(NSUrlRequest request, ref NSUrlResponse response, ref NSError error)
        {
            var _result = (NSData)null;

            var _done = false;
            {
                var _response = (NSUrlResponse)null;
                var _error = (NSError)null;

                var _task = NSUrlSession.SharedSession.CreateDataTask(request, (d, r, e) =>
                {
                    _response = r;
                    _error = e;
                    _result = d;

                    _done = true;
                });

                _task.Resume();

                while (_done == false)
                    NSThread.SleepFor(0);

                response = _response;
                error = _error;
            }

            return _result;
        }

        private ApiResult<T> ApiPostCall<T>(string action_url)
        {
            var _result = new ApiResult<T>()
            {
                success = false,
                message = "",
                result = default(T)
            };

            try
            {
                var _request = new NSMutableUrlRequest(NSUrl.FromString($"{Constants.LionApiUrl}{action_url}"));
                _request.HttpMethod = "POST";

                var _response = (NSUrlResponse)null;
                var _error = (NSError)null;

                var _url_data = this.ApiCallAsync(_request, ref _response, ref _error);
                if (_error != null)
                {
                    NSLogHelper.NSLog($"apiPostCall error: {_error}");
                    _result.message = _error.ToString();
                }
                else
                    _result = JsonConvert.DeserializeObject<ApiResult<T>>(_url_data.ToString());
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", "서버가 응답하지 않습니다.\n 잠시 후 다시 시도해주시기 바랍니다.", "확인");
                _result.message = ex.ToString();
            }

            return _result;
        }

        private ApiResult<T> ApiTokenCall<T>(string guest_token, string action_url, Dictionary<string, object> args = null)
        {
            var _result = new ApiResult<T>()
            {
                success = false,
                message = "",
                result = default(T)
            };

            try
            {
                var _request = new NSMutableUrlRequest(NSUrl.FromString($"{Constants.LionApiUrl}{action_url}"));
                _request.HttpMethod = "POST";

                _request.Headers = NSDictionary.FromObjectsAndKeys(
                            new object[] { $"Bearer {guest_token}" }, 
                            new object[] { "Authorization" }
                        );

                if (args != null)
                {
                    var _request_body = "";
                    foreach (var a in args)
                    {
                        if (_request_body != "")
                            _request_body += "&";
                        _request_body += $"{a.Key}={a.Value}";
                    }

                    _request.Body = _request_body;
                }

                var _response = (NSUrlResponse)null;
                var _error = (NSError)null;

                var _url_data = this.ApiCallAsync(_request, ref _response, ref _error);
                if (_error != null)
                {
                    NSLogHelper.NSLog($"apiTokenCall error: {_error}");
                    _result.message = _error.ToString();
                }
                else
                    _result = JsonConvert.DeserializeObject<ApiResult<T>>(_url_data.ToString());
            }
            catch (Exception ex)
            {
                ConfigHelper.ShowConfirm("오류", "서버가 응답하지 않습니다.\n 잠시 후 다시 시도해주시기 바랍니다.", "확인");
                _result.message = ex.ToString();
            }

            return _result;
        }

        public ApiResult<string> GetGuestToken()
        {
            var _result = ApiPostCall<string>("/api/user/GetGuestToken");
            return _result;
        }

        public ApiResult<NextWeekPrize> GetThisWeekPrize(string guest_token)
        {
            var _params = new Dictionary<string, object>();
            {
            }

            var _result = ApiTokenCall<NextWeekPrize>(guest_token, "/api/lotto/GetThisWeekPrize", _params);
            return _result;
        }

        public ApiResult<WinnerPrize> GetPrizeBySeqNo(string guest_token, int sequence_no)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("sequence_no", sequence_no);
            }

            var _result = ApiTokenCall<WinnerPrize>(guest_token, "/api/lotto/GetPrizeBySeqNo", _params);
            return _result;
        }

        public ApiResult<string> SendMailToCheckMailAddress(string guest_token, string mail_address)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("mail_address", mail_address);
            }

            var _result = ApiTokenCall<string>(guest_token, "/api/user/SendMailToCheckMailAddress", _params);
            return _result;
        }

        public ApiResult<string> CheckMailAddress(string guest_token, string mail_address, string check_number)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("mail_address", mail_address);
                _params.Add("check_number", check_number);
            }

            var _result = ApiTokenCall<string>(guest_token, "/api/user/CheckMailAddress", _params);
            return _result;
        }

        public ApiResult<string> SendMailToChangeMailAddress(string user_token, string mail_address)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("mail_address", mail_address);
            }

            var _result = ApiTokenCall<string>(user_token, "/api/user/SendMailToChangeMailAddress", _params);
            return _result;
        }

        public ApiResult<string> GetTokenByLoginId(string guest_token, string login_id, string password, string device_type, string device_id)
        {
            if (String.IsNullOrWhiteSpace(device_id) == true)
            {
                if (ConfigHelper.IsSimulator)
                    device_id = "e0ca5695e264e7d9ef06f794f66ca37807ad22bda92aa5f48f66ae164568799b";
                else
                    device_id = $"failed to get token: {UIDevice.CurrentDevice.SystemVersion}";
            }

            var _params = new Dictionary<string, object>();
            {
                _params.Add("login_id", login_id);
                _params.Add("password", password);
                _params.Add("device_type", device_type);
                _params.Add("device_id", device_id);
            }

            var _result = ApiTokenCall<string>(guest_token, "/api/user/GetTokenByLoginId", _params);
            return _result;
        }

        public ApiResult<string> AddMemberByLoginId(string guest_token, string login_id, string login_name, string password, string mail_address, string device_id, string device_type, string check_number)
        {
            if (String.IsNullOrWhiteSpace(device_id) == true)
            {
                if (ConfigHelper.IsSimulator)
                    device_id = "e0ca5695e264e7d9ef06f794f66ca37807ad22bda92aa5f48f66ae164568799b";
                else
                    device_id = $"failed to get token: {UIDevice.CurrentDevice.SystemVersion}";
            }

            var _params = new Dictionary<string, object>();
            {
                _params.Add("login_id", login_id);
                _params.Add("login_name", login_name);
                _params.Add("password", password);
                _params.Add("mail_address", mail_address);
                _params.Add("device_type", device_type);
                _params.Add("device_id", device_id);
                _params.Add("check_number", check_number);
            }

            var _result = ApiTokenCall<string>(guest_token, "/api/user/AddMemberByLoginId", _params);
            return _result;
        }

        public ApiResult<UserInfo> GetUserInfor(string user_token)
        {
            var _params = new Dictionary<string, object>();
            {
            }

            var _result = ApiTokenCall<UserInfo>(user_token, "/api/user/GetUserInfor", _params);
            return _result;
        }

        public ApiResult<List<UserChoice>> GetUserChoices(string user_token, int sequence_no)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("sequence_no", sequence_no);
            }

            var _result = ApiTokenCall<List<UserChoice>>(user_token, "/api/lotto/GetUserChoices", _params);
            return _result;
        }

        public ApiResult<List<PushMessage>> GetMessages(string user_token, string notify_time)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("notify_time", notify_time);
            }

            var _result = ApiTokenCall<List<PushMessage>>(user_token, "/api/notify/GetMessages", _params);
            return _result;
        }

        public ApiResult<int> GetAlertCount(string user_token)
        {
            var _params = new Dictionary<string, object>();
            {
            }

            var _result = ApiTokenCall<int>(user_token, "/api/notify/GetCount", _params);
            return _result;
        }

        public ApiResult<string> ClearAlert(string user_token)
        {
            var _params = new Dictionary<string, object>();
            {
            }

            var _result = ApiTokenCall<string>(user_token, "/api/notify/Clear", _params);
            return _result;
        }

        public ApiResult<string> UpdateUserInfor(string user_token, string login_name, short max_select_number, string digit1, string digit2, string digit3)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("login_name", login_name);
                _params.Add("max_select_number", max_select_number);
                _params.Add("digit1", digit1);
                _params.Add("digit2", digit2);
                _params.Add("digit3", digit3);
            }

            var _result = ApiTokenCall<string>(user_token, "/api/user/UpdateUserInfor", _params);
            return _result;
        }

        public ApiResult<string> ChangePassword(string user_token, string new_password)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("password", new_password);
            }

            var _result = ApiTokenCall<string>(user_token, "/api/user/ChangePassword", _params);
            return _result;
        }

        public ApiResult<string> UpdateMailAddress(string user_token, string mail_address, string check_number)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("mail_address", mail_address);
                _params.Add("check_number", check_number);
            }

            var _result = ApiTokenCall<string>(user_token, "/api/user/UpdateMailAddress", _params);
            return _result;
        }

        public ApiResult<string> LeaveMember(string user_token)
        {
            var _params = new Dictionary<string, object>();
            {
            }

            var _result = ApiTokenCall<string>(user_token, "/api/user/LeaveMember", _params);
            return _result;
        }

        public ApiResult<string> SendMailToRecoveryId(string guest_token, string mail_address)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("mail_address", mail_address);
            }

            var _result = ApiTokenCall<string>(guest_token, "/api/user/SendMailToRecoveryId", _params);
            return _result;
        }

        public ApiResult<List<TKeyValue>> GetUserSequenceNos(string user_token)
        {
            var _params = new Dictionary<string, object>();
            {
            }

            var _result = ApiTokenCall<List<TKeyValue>>(user_token, "/api/lotto/GetUserSequenceNos", _params);
            return _result;
        }

        public ApiResult<string> SendChoicedNumbers(string user_token, int sequence_no)
        {
            var _params = new Dictionary<string, object>();
            {
                _params.Add("sequence_no", sequence_no);
            }

            var _result = ApiTokenCall<string>(user_token, "/api/lotto/SendChoicedNumbers", _params);
            return _result;
        }
    }
}