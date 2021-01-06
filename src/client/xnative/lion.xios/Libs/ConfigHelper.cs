using Foundation;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UIKit;

namespace Lion.XiOS.Libs
{
    public class ConfigHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool IsSimulator
        {
            get
            {
                return ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email_address"></param>
        /// <returns></returns>
        public static bool NSStringIsValidEmail(string email_address)
        {
            var _validator = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                           + "@"
                           + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

            var _regex = new Regex(_validator);
            return _regex.IsMatch(email_address);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email_address"></param>
        /// <returns></returns>
        public static bool ValidateEmail(string email_address)
        {
            var _regex = "[A-Z0-9a-z._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}";

            var _tester = NSPredicate.FromFormat(String.Format("SELF MATCHES {0}", _regex));
            return _tester.EvaluateWithObject(NSObject.FromObject(email_address));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ValidatePassword(string password)
        {
            var _regex = "^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$";

            var _tester = NSPredicate.FromFormat(String.Format("SELF MATCHES {0}", NSObject.FromObject(_regex)));
            return _tester.EvaluateWithObject(NSObject.FromObject(password));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="yes_button"></param>
        /// <param name="no_button"></param>
        /// <returns></returns>
        public static void ShowYesNo(string title, string message, string yes_button, string no_button, Action<UIAlertAction> accept_handler = null)
        {
            var _alerter = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            {
                var _accept = UIAlertAction.Create(yes_button, UIAlertActionStyle.Default, accept_handler);
                _alerter.AddAction(_accept);

                var _cancel = UIAlertAction.Create(no_button, UIAlertActionStyle.Default, action =>
                {
                    _alerter.DismissViewController(true, null);
                });
                _alerter.AddAction(_cancel);
            }

            var _view_controller = UIApplication.SharedApplication.Delegate.GetWindow().RootViewController;
            {
                _view_controller.PresentViewController(_alerter, true, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static void ShowConfirm(string title, string message, string button, Action<UIAlertAction> accept_handler = null)
        {
            var _alerter = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            {
                var _accept = UIAlertAction.Create(button, UIAlertActionStyle.Default, accept_handler);
                _alerter.AddAction(_accept);
            }

            var _view_controller = UIApplication.SharedApplication.Delegate.GetWindow().RootViewController;
            {
                _view_controller.PresentViewController(_alerter, true, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static int ShowAlert(string title, string message, params string[] buttons)
        {
            var _result = -1;

            var _alert_view = new UIAlertView
            {
                Title = title,
                Message = message
            };

            foreach (var _button in buttons)
                _alert_view.AddButton(_button);

            _alert_view.Clicked += (s, e) => _result = (int)e.ButtonIndex;
            _alert_view.Show();

            // Wait for a button press.
            while (_result == -1)
                NSRunLoop.Current.RunUntil(NSDate.FromTimeIntervalSinceNow(0.5));

            return _result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static async Task<nint> ShowAlertAsync(string title, string message, params string[] buttons)
        {
            var _tcs = new TaskCompletionSource<nint>();

            var _alert_view = new UIAlertView
            {
                Title = title,
                Message = message
            };

            foreach (var _button in buttons)
                _alert_view.AddButton(_button);

            _alert_view.Clicked += (s, e) => _tcs.TrySetResult(e.ButtonIndex);
            _alert_view.Show();

            return await _tcs.Task;
        }
    }
}