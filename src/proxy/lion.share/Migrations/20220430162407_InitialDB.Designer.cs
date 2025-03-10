﻿// <auto-generated />
using System;
using Lion.Share.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Lion.Share.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220430162407_InitialDB")]
    partial class InitialDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Lion.Share.Data.Models.mAnalysis", b =>
                {
                    b.Property<int>("SequenceNo")
                        .HasColumnType("int");

                    b.Property<short>("Digit1")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit2")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit3")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit4")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit5")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit6")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit7")
                        .HasColumnType("smallint");

                    b.Property<short>("EdgeGroup")
                        .HasColumnType("smallint");

                    b.Property<short>("EdgeHighLow")
                        .HasColumnType("smallint");

                    b.Property<short>("EdgeNumber")
                        .HasColumnType("smallint");

                    b.Property<short>("Group18")
                        .HasColumnType("smallint");

                    b.Property<short>("Group27")
                        .HasColumnType("smallint");

                    b.Property<short>("Group36")
                        .HasColumnType("smallint");

                    b.Property<short>("Group45")
                        .HasColumnType("smallint");

                    b.Property<short>("Group9")
                        .HasColumnType("smallint");

                    b.Property<string>("HighLow")
                        .HasColumnType("nvarchar(32)");

                    b.Property<short>("HotCold1")
                        .HasColumnType("smallint");

                    b.Property<short>("HotCold2")
                        .HasColumnType("smallint");

                    b.Property<short>("HotCold3")
                        .HasColumnType("smallint");

                    b.Property<short>("HotCold4")
                        .HasColumnType("smallint");

                    b.Property<short>("HotCold5")
                        .HasColumnType("smallint");

                    b.Property<short>("HotCold6")
                        .HasColumnType("smallint");

                    b.Property<decimal>("HotColdAvg")
                        .HasColumnType("decimal(5,2)");

                    b.Property<short>("HotColdL10")
                        .HasColumnType("smallint");

                    b.Property<short>("HotColdSum")
                        .HasColumnType("smallint");

                    b.Property<short>("NoDigits10")
                        .HasColumnType("smallint");

                    b.Property<short>("NoDigits3")
                        .HasColumnType("smallint");

                    b.Property<short>("NoDigits5")
                        .HasColumnType("smallint");

                    b.Property<short>("NoWinner10")
                        .HasColumnType("smallint");

                    b.Property<short>("NoWinner3")
                        .HasColumnType("smallint");

                    b.Property<short>("NoWinner5")
                        .HasColumnType("smallint");

                    b.Property<string>("OddEven")
                        .HasColumnType("nvarchar(32)");

                    b.Property<short>("SameJackpot4")
                        .HasColumnType("smallint");

                    b.Property<short>("SameJackpot5")
                        .HasColumnType("smallint");

                    b.Property<short>("SameJackpot6")
                        .HasColumnType("smallint");

                    b.Property<short>("SeriesNumber")
                        .HasColumnType("smallint");

                    b.Property<short>("SumAppearance")
                        .HasColumnType("smallint");

                    b.Property<short>("SumBalance")
                        .HasColumnType("smallint");

                    b.Property<short>("SumEven")
                        .HasColumnType("smallint");

                    b.Property<short>("SumFrequence")
                        .HasColumnType("smallint");

                    b.Property<short>("SumHigh")
                        .HasColumnType("smallint");

                    b.Property<short>("SumLow")
                        .HasColumnType("smallint");

                    b.Property<short>("SumOdd")
                        .HasColumnType("smallint");

                    b.Property<short>("WinChart0")
                        .HasColumnType("smallint");

                    b.Property<short>("WinChart1")
                        .HasColumnType("smallint");

                    b.Property<short>("WinChart2")
                        .HasColumnType("smallint");

                    b.Property<short>("WinChart3")
                        .HasColumnType("smallint");

                    b.Property<short>("WinChart4")
                        .HasColumnType("smallint");

                    b.Property<short>("WinEven")
                        .HasColumnType("smallint");

                    b.Property<short>("WinHigh")
                        .HasColumnType("smallint");

                    b.Property<short>("WinLow")
                        .HasColumnType("smallint");

                    b.Property<short>("WinOdd")
                        .HasColumnType("smallint");

                    b.HasKey("SequenceNo");

                    b.ToTable("tb_lion_analysis");
                });

            modelBuilder.Entity("Lion.Share.Data.Models.mChoice", b =>
                {
                    b.Property<int>("SequenceNo")
                        .HasColumnType("int");

                    b.Property<string>("LoginId")
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("ChoiceNo")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(12,0)");

                    b.Property<short>("Digit1")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit2")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit3")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit4")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit5")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit6")
                        .HasColumnType("smallint");

                    b.Property<bool>("IsMailSent")
                        .HasColumnType("bit");

                    b.Property<short>("Ranking")
                        .HasColumnType("smallint");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("SequenceNo", "LoginId", "ChoiceNo");

                    b.ToTable("tb_lion_choice");
                });

            modelBuilder.Entity("Lion.Share.Data.Models.mFactor", b =>
                {
                    b.Property<int>("SequenceNo")
                        .HasColumnType("int");

                    b.Property<bool>("HighSelection")
                        .HasColumnType("bit");

                    b.Property<int>("LNoCombination")
                        .HasColumnType("int");

                    b.Property<int>("LNoExtraction")
                        .HasColumnType("int");

                    b.Property<short>("LNoSampling")
                        .HasColumnType("smallint");

                    b.Property<short>("MaxGroupBalance")
                        .HasColumnType("smallint");

                    b.Property<short>("MaxHighLow")
                        .HasColumnType("smallint");

                    b.Property<short>("MaxLastNumber")
                        .HasColumnType("smallint");

                    b.Property<short>("MaxOddEven")
                        .HasColumnType("smallint");

                    b.Property<short>("MaxSameDigits")
                        .HasColumnType("smallint");

                    b.Property<short>("MaxSameJackpot")
                        .HasColumnType("smallint");

                    b.Property<short>("MaxSeriesNumber")
                        .HasColumnType("smallint");

                    b.Property<short>("MaxSumFrequence")
                        .HasColumnType("smallint");

                    b.Property<short>("MinHighLow")
                        .HasColumnType("smallint");

                    b.Property<short>("MinOddEven")
                        .HasColumnType("smallint");

                    b.Property<int>("NoJackpot1")
                        .HasColumnType("int");

                    b.Property<int>("NoJackpot2")
                        .HasColumnType("int");

                    b.Property<int>("NoJackpot3")
                        .HasColumnType("int");

                    b.Property<int>("NoJackpot4")
                        .HasColumnType("int");

                    b.Property<int>("NoJackpot5")
                        .HasColumnType("int");

                    b.Property<bool>("OddSelection")
                        .HasColumnType("bit");

                    b.Property<int>("RNoCombination")
                        .HasColumnType("int");

                    b.Property<int>("RNoExtraction")
                        .HasColumnType("int");

                    b.Property<short>("RNoSampling")
                        .HasColumnType("smallint");

                    b.Property<decimal>("WinningAmount1")
                        .HasColumnType("decimal(12,0)");

                    b.Property<decimal>("WinningAmount2")
                        .HasColumnType("decimal(12,0)");

                    b.Property<decimal>("WinningAmount3")
                        .HasColumnType("decimal(12,0)");

                    b.Property<decimal>("WinningAmount4")
                        .HasColumnType("decimal(12,0)");

                    b.Property<decimal>("WinningAmount5")
                        .HasColumnType("decimal(12,0)");

                    b.HasKey("SequenceNo");

                    b.ToTable("tb_lion_factor");
                });

            modelBuilder.Entity("Lion.Share.Data.Models.mJackpot", b =>
                {
                    b.Property<int>("SequenceNo")
                        .HasColumnType("int");

                    b.Property<string>("LoginId")
                        .HasColumnType("nvarchar(32)");

                    b.Property<short>("Digit1")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit2")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit3")
                        .HasColumnType("smallint");

                    b.Property<short>("NoChoice")
                        .HasColumnType("smallint");

                    b.Property<int>("NoJackpot")
                        .HasColumnType("int");

                    b.Property<decimal>("WinningAmount")
                        .HasColumnType("decimal(12,0)");

                    b.HasKey("SequenceNo", "LoginId");

                    b.ToTable("tb_lion_jackpot");
                });

            modelBuilder.Entity("Lion.Share.Data.Models.mMember", b =>
                {
                    b.Property<string>("LoginId")
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("DeviceType")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<short>("Digit1")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit2")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit3")
                        .HasColumnType("smallint");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)");

                    b.Property<bool>("IsAlive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDirectSend")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMailSend")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNumberChoice")
                        .HasColumnType("bit");

                    b.Property<string>("LoginName")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("LoginPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(64)");

                    b.Property<bool>("MailError")
                        .HasColumnType("bit");

                    b.Property<short>("MaxSelectNumber")
                        .HasColumnType("smallint");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("LoginId");

                    b.ToTable("tb_lion_member");
                });

            modelBuilder.Entity("Lion.Share.Data.Models.mNotify", b =>
                {
                    b.Property<string>("LoginId")
                        .HasColumnType("nvarchar(32)");

                    b.Property<DateTime>("NotifyTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("LoginId", "NotifyTime");

                    b.ToTable("tb_lion_notify");
                });

            modelBuilder.Entity("Lion.Share.Data.Models.mPercent", b =>
                {
                    b.Property<int>("SequenceNo")
                        .HasColumnType("int");

                    b.Property<short>("LJackpot01")
                        .HasColumnType("smallint");

                    b.Property<short>("LJackpot02")
                        .HasColumnType("smallint");

                    b.Property<short>("LJackpot03")
                        .HasColumnType("smallint");

                    b.Property<short>("LJackpot04")
                        .HasColumnType("smallint");

                    b.Property<short>("LJackpot05")
                        .HasColumnType("smallint");

                    b.Property<short>("LJackpot06")
                        .HasColumnType("smallint");

                    b.Property<short>("LJackpot07")
                        .HasColumnType("smallint");

                    b.Property<short>("LJackpot08")
                        .HasColumnType("smallint");

                    b.Property<short>("LJackpot09")
                        .HasColumnType("smallint");

                    b.Property<short>("LJackpot10")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect01")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect02")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect03")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect04")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect05")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect06")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect07")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect08")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect09")
                        .HasColumnType("smallint");

                    b.Property<short>("LSelect10")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot01")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot02")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot03")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot04")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot05")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot06")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot07")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot08")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot09")
                        .HasColumnType("smallint");

                    b.Property<short>("RJackpot10")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect01")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect02")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect03")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect04")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect05")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect06")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect07")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect08")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect09")
                        .HasColumnType("smallint");

                    b.Property<short>("RSelect10")
                        .HasColumnType("smallint");

                    b.HasKey("SequenceNo");

                    b.ToTable("tb_lion_percent");
                });

            modelBuilder.Entity("Lion.Share.Data.Models.mSelect", b =>
                {
                    b.Property<int>("SequenceNo")
                        .HasColumnType("int");

                    b.Property<int>("SelectNo")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(12,0)");

                    b.Property<short>("Digit1")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit2")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit3")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit4")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit5")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit6")
                        .HasColumnType("smallint");

                    b.Property<bool>("IsLeft")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<short>("Ranking")
                        .HasColumnType("smallint");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("SequenceNo", "SelectNo");

                    b.ToTable("tb_lion_select");
                });

            modelBuilder.Entity("Lion.Share.Data.Models.mWinner", b =>
                {
                    b.Property<int>("SequenceNo")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount1")
                        .HasColumnType("decimal(12,0)");

                    b.Property<decimal>("Amount2")
                        .HasColumnType("decimal(12,0)");

                    b.Property<decimal>("Amount3")
                        .HasColumnType("decimal(12,0)");

                    b.Property<decimal>("Amount4")
                        .HasColumnType("decimal(12,0)");

                    b.Property<decimal>("Amount5")
                        .HasColumnType("decimal(12,0)");

                    b.Property<short>("AutoSelect")
                        .HasColumnType("smallint");

                    b.Property<int>("Count1")
                        .HasColumnType("int");

                    b.Property<int>("Count2")
                        .HasColumnType("int");

                    b.Property<int>("Count3")
                        .HasColumnType("int");

                    b.Property<int>("Count4")
                        .HasColumnType("int");

                    b.Property<int>("Count5")
                        .HasColumnType("int");

                    b.Property<short>("Digit1")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit2")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit3")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit4")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit5")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit6")
                        .HasColumnType("smallint");

                    b.Property<short>("Digit7")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("IssueDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("SequenceNo");

                    b.ToTable("tb_lion_winner");
                });
#pragma warning restore 612, 618
        }
    }
}
