using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lion.Share.Data.Models
{
    [Keyless]
    public class mChoice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SequenceNo
        {
            get; set;
        }

        [Column(TypeName = "nvarchar(32)")]
        public string LoginId
        {
            get; set;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ChoiceNo
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

        public short Digit4
        {
            get; set;
        }

        public short Digit5
        {
            get; set;
        }

        public short Digit6
        {
            get; set;
        }

        public bool IsMailSent
        {
            get; set;
        }

        public short Ranking
        {
            get; set;
        }

        [Column(TypeName = "decimal(12,0)")]
        public decimal Amount
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