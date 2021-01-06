using OdinSdk.BaseLib.Queue;

namespace LottoLion.BaseLib.Queues
{
    /// <summary>
    ///
    /// </summary>
    public class WinnerQ : WorkerQ
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="queue_name"></param>
        public WinnerQ(string queue_name = QueueNames.WinnerQName)
            : base(queue_name: queue_name)
        {
        }
    }
}