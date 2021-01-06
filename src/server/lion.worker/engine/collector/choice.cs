using LottoLion.BaseLib.Controllers;
using LottoLion.BaseLib.Models;
using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Queues;
using LottoLion.BaseLib.Types;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LottoLion.Worker.Engine
{
    /// <summary>
    /// analysis(분석)의 회차번호와 select(추출)의 회차번를 비교하여
    /// 분석은 되었지만 추출 하지 못한 회차의 예상 번호를 추출 합니다.
    /// 이후, member(회원)들에게 메일을 발송 하도록 큐에 명령을 발송 합니다.
    /// </summary>
    public partial class Collector
    {
        private static MemberQ __memberQ = new MemberQ();

        private static async Task<int> SelectMemberAsync(LottoLionContext ltctx, int choice_no, bool resend)
        {
            var _result = 0;

            try
            {
                // 가입 member(회원)들에게 각자에게 할당 된 갯수만큼을 select(추출)테이블에서 choice(선택) 테이블로
                // 복사 후 메일 발송 하도록 큐에 명령을 보냅니다. (여기서 처리 하지 않고 memberQ에서 pop하여 처리 함)
                var _members = ltctx.TbLionMember
                                    .Where(m => m.IsNumberChoice == true && m.IsMailSend == true && m.IsAlive == true && m.MailError == false)
                                    .ToList();

                foreach (var _member in _members)
                {
                    var _no_choice = ltctx.TbLionChoice
                                        .Where(c => c.SequenceNo == choice_no && c.LoginId == _member.LoginId)
                                        .GroupBy(c => c.IsMailSent)
                                        .Select(n => new { b = n.Key, c = n.Count() })
                                        .ToList();

                    var _no_choice_t = _no_choice.Where(c => c.b == true).Sum(c => c.c);
                    var _no_choice_f = _no_choice.Where(c => c.b == false).Sum(c => c.c);

                    // 기 선택 하였으면 skip 함
                    if (_no_choice_t + _no_choice_f <= 0 || _no_choice_f > 0)
                    {
                        var _choice = new TChoice()
                        {
                            sequence_no = choice_no,
                            login_id = _member.LoginId,
                            resend = resend || _no_choice_f > 0
                        };

                        // member(회원)들에게 메일 발송 하도록 큐에 명령을 보냅니다.
                        await __memberQ.SendQAsync(_choice);

                        _result++;
                    }
                }
            }
            catch (Exception ex)
            {
                __clogger.WriteLog(ex);
            }

            return _result;
        }

        public static async Task<int> Select2MemberAsync(LottoLionContext ltctx, int select_no, bool resend = true)
        {
            var _no_member = 0;

            var _today_winner_no = WinnerReader.GetThisWeekSequenceNo();

            // 현재의 예상 번호가 select(추출) 되어 있으면, choice(선택) 한다.
            // analysis(분석) 차수와 같은 차수이면 select(추출)이 있다고 가정한다.
            if (_today_winner_no == select_no - 1)
            {
                _no_member = await SelectMemberAsync(ltctx, select_no, resend);
                if (_no_member > 0)
                    __clogger.WriteDebug($"[send-member] ({select_no}) push member => {_no_member} members");
            }

            return _no_member;
        }

        /// <summary>
        /// 주기적으로 select(추출) 테이블을 생성 합니다.
        /// winner(당첨) 테이블의 마지막 번호를 확인 하여 +1 회차에 해당하는 번호를 추출 합니다.
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">1시간에 한번 번호 발송</param>
        /// <param name="console_out"></param>
        public static async void StartMember(CancellationTokenSource tokenSource, int sleep_seconds = 60 * 60 * 1)
        {
            __clogger.WriteDebug("[send-member] starting collctor memberQ...");

            while (true)
            {
                try
                {
                    using (var _ltctx = LTCX.GetNewContext())
                    {
                        var _select_no = _ltctx.TbLionSelect.Max(x => x.SequenceNo);
                        await Select2MemberAsync(_ltctx, _select_no, false);
                    }
                }
                catch (Exception ex)
                {
                    __clogger.WriteLog(ex);
                }
                finally
                {
                    GC.Collect(2, GCCollectionMode.Optimized);
                }

                var _cancelled = tokenSource.Token.WaitHandle.WaitOne(sleep_seconds * 1000);
                if (_cancelled == true)
                    break;
            }
        }
    }
}