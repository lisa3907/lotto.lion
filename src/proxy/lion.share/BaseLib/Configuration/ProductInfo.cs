using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#pragma warning disable 8632

namespace OdinSdk.BaseLib.Configuration
{
    /// <summary>
    ///
    /// </summary>
    public class ProductInfo
    {
        /// <summary>
        ///
        /// </summary>
        public ProductInfo()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="product_id">제품 ID</param>
        /// <param name="version"></param>
        /// <param name="product_name"></param>
        /// <param name="product_type"></param>
        public ProductInfo(string product_id, string version, string product_name, ProductType? product_type)
        {
            this.productId = product_id;
            this.version = version;
            this.product_name = product_name;
            this.productType = product_type;
        }

        /// <summary>
        ///
        /// </summary>
        public string? productId
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string? version
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string product_name
        {
            get;
            set;
        }

        /// <summary>
        /// type of permit service
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductType? productType
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            var p = (ProductInfo)obj;
            return p.productId == this.productId && p.version == this.version;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}