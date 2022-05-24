using Lion.Share.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Lion.Share.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
               : base(options)
        {
        }

        public virtual DbSet<mAnalysis> tb_lion_analysis
        {
            get; set;
        }

        public virtual DbSet<mChoice> tb_lion_choice
        {
            get; set;
        }

        public virtual DbSet<mFactor> tb_lion_factor
        {
            get; set;
        }

        public virtual DbSet<mJackpot> tb_lion_jackpot
        {
            get; set;
        }

        public virtual DbSet<mMember> tb_lion_member
        {
            get; set;
        }

        public virtual DbSet<mNotify> tb_lion_notify
        {
            get; set;
        }

        public virtual DbSet<mPercent> tb_lion_percent
        {
            get; set;
        }

        public virtual DbSet<mSelect> tb_lion_select
        {
            get; set;
        }

        public virtual DbSet<mWinner> tb_lion_winner
        {
            get; set;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<mChoice>()
                        .HasKey("SequenceNo", "LoginId", "ChoiceNo");

            modelBuilder.Entity<mJackpot>()
                        .HasKey("SequenceNo", "LoginId");

            modelBuilder.Entity<mNotify>()
                        .HasKey("LoginId", "NotifyTime");

            modelBuilder.Entity<mSelect>()
                        .HasKey("SequenceNo", "SelectNo");
        }
    }
}