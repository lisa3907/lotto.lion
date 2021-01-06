namespace LottoLion.BaseLib.Queues
{
    public class QueueNames
    {
#if DEBUG
        public const string QueueName = "debuglion";
#else
        public const string QueueName = "lottolion";
#endif

        public const string AnalysisQName = QueueName + "_analysisQ";
        public const string MemberQName = QueueName + "_memberQ";
        public const string SelectorQName = QueueName + "_selectorQ";
        public const string WinnerQName = QueueName + "_winnerQ";
    }
}