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
    /// 분석은 되었지만 추출 하지 못한 회차의 예상 번호가 있으면
    /// 큐를 통해 명령을 발송 합니다.
    /// </summary>
    public partial class Collector
    {
        private static SelectorQ __selectorQ = new SelectorQ();

        private static async Task<int> AnalysisSelectAsync(LottoLionContext ltctx, int select_no)
        {
            var _result = 0;

            try
            {
                // 분석 테이블의 마지막 회차+1에 해당하는 회차의 예상 번호를 추출 합니다.
                var _select_no = ltctx.TbLionSelect.Max(x => x.SequenceNo);

                for (_select_no++; _select_no <= select_no + 1; _select_no++)
                {
                    var _selection = new TSelector()
                    {
                        sequence_no = _select_no,
                        sent_by_queue = true
                    };

                    await __selectorQ.SendQAsync(_selection);

                    _result++;
                }
            }
            catch (Exception ex)
            {
                ltctx.Database.RollbackTransaction();

                __clogger.WriteLog(ex);
            }

            return _result;
        }

        public static async Task<int> Analysis2SelectAsync(LottoLionContext ltctx, int analysis_no)
        {
            var _no_select = await AnalysisSelectAsync(ltctx, analysis_no);
            if (_no_select > 0)
                __clogger.WriteDebug($"[send-select] ({analysis_no}) push select => {_no_select} games");

            return _no_select;
        }

        /// <summary>
        /// 주기적으로 select(추출) 테이블을 생성 합니다.
        /// winner(당첨) 테이블의 마지막 번호를 확인 하여 +1 회차에 해당하는 번호를 추출 합니다.
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="sleep_seconds">60분에 한번 추출 번호 생성</param>
        /// <param name="console_out"></param>
        public static async void StartSelector(CancellationTokenSource tokenSource, int sleep_seconds = 60 * 60 * 1)
        {
            __clogger.WriteDebug("[send-select] starting collctor selectorQ...");

            while (true)
            {
                try
                {
                    using (var _ltctx = LTCX.GetNewContext())
                    {
                        var _analysis_no = _ltctx.TbLionAnalysis.Max(x => x.SequenceNo);

                        // analysis(분석)의 회차번호와 select(추출)의 회차번를 비교하여
                        // 분석은 되었지만 추출 하지 못한 회차의 예상 번호를 처리 합니다.
                        await Analysis2SelectAsync(_ltctx, _analysis_no);
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