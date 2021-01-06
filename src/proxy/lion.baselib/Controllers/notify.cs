using DotNet.Push;
using LottoLion.BaseLib.Models.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LottoLion.BaseLib.Controllers
{
    public class NotifyPushLottoLion
    {
        private static LConfig __cconfig = new LConfig();

        private string IosApnsKeyId
        {
            get
            {
                return __cconfig.GetAppString("ios.apns.keyid");
            }
        }

        private string IosApnsTeamId
        {
            get
            {
                return __cconfig.GetAppString("ios.apns.teamid");
            }
        }

        private string IosApnsAppId
        {
            get
            {
                return __cconfig.GetAppString("ios.apns.appid");
            }
        }

        private string IosApnsAuthFile
        {
            get
            {
                return __cconfig.GetAppString("ios.apns.authfile");
            }
        }

        private string IosApnsSound
        {
            get
            {
                return __cconfig.GetAppString("ios.apns.sound");
            }
        }

        private IosPushNotifyAPNs __ios_apns = null;

        private IosPushNotifyAPNs IosPushNotifyAPNs
        {
            get
            {
                if (__ios_apns == null)
                    __ios_apns = new IosPushNotifyAPNs(IosApnsKeyId, IosApnsTeamId, IosApnsAppId, IosApnsAuthFile);
                return __ios_apns;
            }
        }

        private string DroidFcmApikey
        {
            get
            {
                return __cconfig.GetAppString("droid.fcm.apikey");
            }
        }

        private string DroidFcmServerId
        {
            get
            {
                return __cconfig.GetAppString("droid.fcm.serverid");
            }
        }

        private string DroidFcmAlarmTag
        {
            get
            {
                return __cconfig.GetAppString("droid.fcm.alarmtag");
            }
        }

        private DroidPushNotifyFCM __droid_fcm = null;

        private DroidPushNotifyFCM DroidPushNotifyFCM
        {
            get
            {
                if (__droid_fcm == null)
                    __droid_fcm = new DroidPushNotifyFCM(DroidFcmApikey, DroidFcmServerId, DroidFcmAlarmTag);
                return __droid_fcm;
            }
        }

        private async Task<(bool success, string message)> SaveNotification(LottoLionContext ltctx, TbLionMember member, string message)
        {
            var _result = (success: false, message: "ok");

            try
            {
                var _notify = new TbLionNotify()
                {
                    LoginId = member.LoginId,
                    NotifyTime = DateTime.Now,

                    Message = message,
                    IsRead = false
                };

                ltctx.TbLionNotify.Add(_notify);

                _result.success = await ltctx.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _result.message = ex.Message;
            }

            return _result;
        }

        private async Task<(bool success, string message)> SendNotification(LottoLionContext ltctx, TbLionMember member, string title, string message)
        {
            var _result = (success: false, message: "ok");

            try
            {
                var _notify_count = ltctx.TbLionNotify
                                            .Where(n => n.LoginId == member.LoginId && n.IsRead == false)
                                            .Count();

                _notify_count++;

                if (member.DeviceType == "I")
                {
                    _result = await IosPushNotifyAPNs
                                    .JwtAPNsPush(member.DeviceId, message, _notify_count, IosApnsSound);
                }
                else if (member.DeviceType == "A")
                {
                    _result = await DroidPushNotifyFCM
                                    .SendNotification(member.DeviceId, "high", title, "PUSH_EVENT_ALARM", message, _notify_count, "alarm", "#d32121");
                }
                else if (member.DeviceType == "W")
                {
                }

                if (_result.success == false)
                {
                    member.DeviceType = "U";
                    member.DeviceId = "";

                    await ltctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _result.message = ex.Message;
            }

            return _result;
        }

        public async Task<(bool success, string message)> PushNotification(LottoLionContext ltctx, TbLionMember member, string title, string message)
        {
            var _result = (success: false, message: "ok");

            if (
                    String.IsNullOrEmpty(member.DeviceId) == false &&
                    (member.DeviceType == "I" || member.DeviceType == "A" || member.DeviceType == "W")
               )
            {
                _result = await SaveNotification(ltctx, member, message);

                if (_result.success == true)
                    await SendNotification(ltctx, member, title, message);
            }

            return _result;
        }
    }
}