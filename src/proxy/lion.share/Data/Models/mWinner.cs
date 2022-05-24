using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lion.Share.Data.Models
{
    public class mWinner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SequenceNo
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime IssueDate
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime PaymentDate
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

        public short Digit7
        {
            get; set;
        }

        public short AutoSelect
        {
            get; set;
        }

        public int Count1
        {
            get; set;
        }

        [Column(TypeName = "decimal(12,0)")]
        public decimal Amount1
        {
            get; set;
        }

        public int Count2
        {
            get; set;
        }

        [Column(TypeName = "decimal(12,0)")]
        public decimal Amount2
        {
            get; set;
        }

        public int Count3
        {
            get; set;
        }

        [Column(TypeName = "decimal(12,0)")]
        public decimal Amount3
        {
            get; set;
        }

        public int Count4
        {
            get; set;
        }

        [Column(TypeName = "decimal(12,0)")]
        public decimal Amount4
        {
            get; set;
        }

        public int Count5
        {
            get; set;
        }

        [Column(TypeName = "decimal(12,0)")]
        public decimal Amount5
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