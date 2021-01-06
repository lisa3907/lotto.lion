using OdinSdk.BaseLib.Queue;

namespace LottoLion.BaseLib.Queues
{
    /// <summary>
    ///
    /// </summary>
    public class AnalysisQ : WorkerQ
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="queue_name"></param>
        public AnalysisQ(string queue_name = QueueNames.AnalysisQName)
            : base(queue_name: queue_name)
        {
        }
    }
}