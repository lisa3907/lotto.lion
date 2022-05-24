using System;

namespace Lion.Share.Pipe
{
    public class VmsRequest<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public QCommand command
        {
            get;
            set;
        }

        public T data
        {
            get;
            set;
        }
    }

    public class VmsResponse<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public QCommand command
        {
            get;
            set;
        }

        public string message
        {
            get;
            set;
        }

        public T data
        {
            get;
            set;
        }
    }
}