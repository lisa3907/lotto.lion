using System.Runtime.Serialization;

namespace OdinSdk.BaseLib.WebApi
{
    /// <summary>
    ///
    /// </summary>
    [DataContract]
    public class CApiResult<T>
    {
        /// <summary>
        ///
        /// </summary>
        public CApiResult()
        {
            this.category = "";     // kind of error: error, warning, infor
            this.location = "";     // function name
            this.layer = "";        // 3tier: DT(database), BA(bizapp), WA(webapi)

            this.status = 0;
            this.message = "";
            this.result = false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="init_value"></param>
        public CApiResult(T init_value)
            : this()
        {
            this.value = init_value;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember(Order = 0)]
        public string category
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember(Order = 1)]
        public string location
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember(Order = 2)]
        public string layer
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember(Order = 3)]
        public int status
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember(Order = 4)]
        public string message
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember(Order = 5)]
        public bool result
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember(Order = 6)]
        public T value
        {
            get;
            set;
        }
    }
}