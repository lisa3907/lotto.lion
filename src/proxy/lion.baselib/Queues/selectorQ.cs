using OdinSdk.BaseLib.Queue;

namespace LottoLion.BaseLib.Queues
{
    /// <summary>
    ///
    /// </summary>
    public class SelectorQ : WorkerQ
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="queue_name"></param>
        public SelectorQ(string queue_name = QueueNames.SelectorQName)
            : base(queue_name: queue_name)
        {
        }
    }
}