using Android.Content;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Lang;
using Java.Net;
using System;
using System.Collections.Generic;

namespace Lion.XDroid.Libs
{
    public class ApiAsnycTask : AsyncTask<Dictionary<string, string>, Java.Lang.Void, string>
    {
        Context pcontext;

        private string target_url;
        private string token_key;

        public event EventHandler<FinishEventArgs> SendFinish;

        public ApiAsnycTask(Context context, string server_url, string token_key = "")
        {
            this.pcontext = context;

            this.target_url = server_url;
            this.token_key = token_key;
        }

        protected override void OnPostExecute(string s)
        {
            if (this.SendFinish != null)
                SendFinish(this, new FinishEventArgs(s));
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] native_parms)
        {
            return base.DoInBackground(native_parms);
        }

        protected override string RunInBackground(params Dictionary<string, string>[] @params)
        {
            var _result = new StringBuilder();

            var _url_connection = (HttpURLConnection)null;
            //Log.Debug(this.GetType().Name, $"Url:{target_url}");

            try
            {
                var _url = new URL(this.target_url);
                {
                    _url_connection = (HttpURLConnection)_url.OpenConnection();
                    _url_connection.SetRequestProperty("Authorization", ("Bearer " + this.token_key));
                    _url_connection.DoInput = true;
                    _url_connection.DoOutput = true;
                    _url_connection.RequestMethod = "POST";
                    _url_connection.ConnectTimeout = 5000;
                }

                if (@params.Length > 0 && @params[0] != null)
                {
                    // 웹 서버로 보낼 매개변수가 있는 경우
                    using (var _os = _url_connection.OutputStream) // 서버로 보내기 위한 출력 스트림
                    {
                        using (var _bw = new BufferedWriter(new OutputStreamWriter(_os, "UTF-8"))) // UTF-8로 전송
                        {
                            _bw.Write(GetPostString(@params[0])); // 매개변수 전송
                            _bw.Flush();

                            _bw.Close();
                        }

                        _os.Close();
                    }
                }

                if (_url_connection.ResponseCode == HttpStatus.Ok)
                {
                    var _buffer = new BufferedReader(new InputStreamReader(_url_connection.InputStream));

                    while (true)
                    {
                        string _line = _buffer.ReadLine();
                        if (_line == null)
                            break;

                        _result.Append(_line).Append("\n");
                    }
                }
                else if (_url_connection.ResponseCode == HttpStatus.Unauthorized)
                {
                    //Log.Debug(this.GetType().Name, "Unauthorized");
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error(this.GetType().Name, ex.Message);

                using (var h = new Handler(Looper.MainLooper))
                {
                    h.Post(() =>
                    {
                        AppDialog.SNG.Alert(this.pcontext, "네트워크를 활성화 해주세요.");
                    });
                }
            }
            finally
            {
                if (_url_connection != null)
                    _url_connection.Disconnect();
            }

            return _result.ToString();
        }

        private string GetPostString(Dictionary<string, string> @params)
        {
            var _result = new StringBuilder();

            foreach (var param in @params)
            {
                if (_result.Length() > 0)
                    _result.Append("&");

                _result.Append(URLEncoder.Encode(param.Key, "UTF-8"));
                _result.Append("=");
                _result.Append(URLEncoder.Encode(param.Value, "UTF-8"));
            }

            return _result.ToString();
        }
    }
}