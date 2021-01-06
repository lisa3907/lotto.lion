using OdinSdk.BaseLib.Queue;

namespace LottoLion.BaseLib.Queues
{
    /// <summary>
    ///
    /// </summary>
    public class MemberQ : WorkerQ
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="queue_name"></param>
        public MemberQ(string queue_name = QueueNames.MemberQName)
            : base(queue_name: queue_name)
        {
        }
    }
}