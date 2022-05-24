using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lion.Share.Data.Models
{
    [Keyless]
    public class mNotify
    {
        [Column(TypeName = "nvarchar(32)")]
        public string LoginId
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime NotifyTime
        {
            get; set;
        }

        [Required]
        [Column(TypeName = "nvarchar(256)")]
        public string Message
        {
            get; set;
        }

        public bool IsRead
        {
            get; set;
        }
    }
}