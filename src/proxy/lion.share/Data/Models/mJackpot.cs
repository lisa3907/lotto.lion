using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lion.Share.Data.Models
{
    [Keyless]
    public class mJackpot
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

        public short NoChoice
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

        public int NoJackpot
        {
            get; set;
        }

        [Column(TypeName = "decimal(12,0)")]
        public decimal WinningAmount
        {
            get; set;
        }
    }
}