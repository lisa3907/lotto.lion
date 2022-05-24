using System.Collections.Concurrent;

namespace Lion.Share.Pipe
{
    public class PQueue
    {
        public const string ReceiveServerPipeName = "lotto_lion_pipe_receiveQ";


        private static ConcurrentQueue<string> __analyst_queue = null;

        /// <summary>
        ///
        /// </summary>
        public static ConcurrentQueue<string> QAnalyst
        {
            get
            {
                if (__analyst_queue == null)
                    __analyst_queue = new ConcurrentQueue<string>();

                return __analyst_queue;
            }
        }

        private static ConcurrentQueue<string> __choicer_queue = null;

        /// <summary>
        ///
        /// </summary>
        public static ConcurrentQueue<string> QChoicer
        {
            get
            {
                if (__choicer_queue == null)
                    __choicer_queue = new ConcurrentQueue<string>();

                return __choicer_queue;
            }
        }

        private static ConcurrentQueue<string> __selector_queue = null;

        /// <summary>
        ///
        /// </summary>
        public static ConcurrentQueue<string> QSelector
        {
            get
            {
                if (__selector_queue == null)
                    __selector_queue = new ConcurrentQueue<string>();

                return __selector_queue;
            }
        }

        private static ConcurrentQueue<string> __winner_queue = null;

        /// <summary>
        ///
        /// </summary>
        public static ConcurrentQueue<string> QWinner
        {
            get
            {
                if (__winner_queue == null)
                    __winner_queue = new ConcurrentQueue<string>();

                return __winner_queue;
            }
        }
    }
}