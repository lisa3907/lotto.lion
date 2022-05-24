using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lion.Share.Data.Models
{
    public class mMember
    {
        [Key]
        [Column(TypeName = "nvarchar(32)")]
        public string LoginId
        {
            get; set;
        }

        [Required]
        [Column(TypeName = "nvarchar(64)")]
        public string LoginName
        {
            get; set;
        }

        [Required]
        [Column(TypeName = "nvarchar(64)")]
        public string LoginPassword
        {
            get; set;
        }

        [Required]
        [Column(TypeName = "nvarchar(1)")]
        public string DeviceType
        {
            get; set;
        }

        [Required]
        [Column(TypeName = "nvarchar(256)")]
        public string DeviceId
        {
            get; set;
        }

        [Required]
        [Column(TypeName = "nvarchar(256)")]
        public string AccessToken
        {
            get; set;
        }

        public bool IsAlive
        {
            get; set;
        }

        [Required]
        [Column(TypeName = "nvarchar(32)")]
        public string PhoneNumber
        {
            get; set;
        }

        [Required]
        [Column(TypeName = "nvarchar(64)")]
        public string EmailAddress
        {
            get; set;
        }

        public bool MailError
        {
            get; set;
        }

        public bool IsMailSend
        {
            get; set;
        }

        public bool IsDirectSend
        {
            get; set;
        }

        public bool IsNumberChoice
        {
            get; set;
        }

        public short MaxSelectNumber
        {
            get; set;
        }

        public short Digit1
        {
            get; set;
        }

        public short Digit2
        {
            get; set;
        }

        public short Digit3
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime CreateTime
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime UpdateTime
        {
            get; set;
        }

        [Column(TypeName = "nvarchar(256)")]
        public string Remark
        {
            get; set;
        }
    }
}