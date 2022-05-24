using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lion.Share.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_lion_analysis",
                columns: table => new
                {
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    Digit1 = table.Column<short>(type: "smallint", nullable: false),
                    Digit2 = table.Column<short>(type: "smallint", nullable: false),
                    Digit3 = table.Column<short>(type: "smallint", nullable: false),
                    Digit4 = table.Column<short>(type: "smallint", nullable: false),
                    Digit5 = table.Column<short>(type: "smallint", nullable: false),
                    Digit6 = table.Column<short>(type: "smallint", nullable: false),
                    Digit7 = table.Column<short>(type: "smallint", nullable: false),
                    WinOdd = table.Column<short>(type: "smallint", nullable: false),
                    WinEven = table.Column<short>(type: "smallint", nullable: false),
                    SumOdd = table.Column<short>(type: "smallint", nullable: false),
                    SumEven = table.Column<short>(type: "smallint", nullable: false),
                    OddEven = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    WinHigh = table.Column<short>(type: "smallint", nullable: false),
                    WinLow = table.Column<short>(type: "smallint", nullable: false),
                    SumHigh = table.Column<short>(type: "smallint", nullable: false),
                    SumLow = table.Column<short>(type: "smallint", nullable: false),
                    HighLow = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    SumFrequence = table.Column<short>(type: "smallint", nullable: false),
                    SumBalance = table.Column<short>(type: "smallint", nullable: false),
                    Group9 = table.Column<short>(type: "smallint", nullable: false),
                    Group18 = table.Column<short>(type: "smallint", nullable: false),
                    Group27 = table.Column<short>(type: "smallint", nullable: false),
                    Group36 = table.Column<short>(type: "smallint", nullable: false),
                    Group45 = table.Column<short>(type: "smallint", nullable: false),
                    HotCold1 = table.Column<short>(type: "smallint", nullable: false),
                    HotCold2 = table.Column<short>(type: "smallint", nullable: false),
                    HotCold3 = table.Column<short>(type: "smallint", nullable: false),
                    HotCold4 = table.Column<short>(type: "smallint", nullable: false),
                    HotCold5 = table.Column<short>(type: "smallint", nullable: false),
                    HotCold6 = table.Column<short>(type: "smallint", nullable: false),
                    HotColdL10 = table.Column<short>(type: "smallint", nullable: false),
                    HotColdSum = table.Column<short>(type: "smallint", nullable: false),
                    HotColdAvg = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    NoWinner3 = table.Column<short>(type: "smallint", nullable: false),
                    NoDigits3 = table.Column<short>(type: "smallint", nullable: false),
                    NoWinner5 = table.Column<short>(type: "smallint", nullable: false),
                    NoDigits5 = table.Column<short>(type: "smallint", nullable: false),
                    NoWinner10 = table.Column<short>(type: "smallint", nullable: false),
                    NoDigits10 = table.Column<short>(type: "smallint", nullable: false),
                    WinChart0 = table.Column<short>(type: "smallint", nullable: false),
                    WinChart1 = table.Column<short>(type: "smallint", nullable: false),
                    WinChart2 = table.Column<short>(type: "smallint", nullable: false),
                    WinChart3 = table.Column<short>(type: "smallint", nullable: false),
                    WinChart4 = table.Column<short>(type: "smallint", nullable: false),
                    SumAppearance = table.Column<short>(type: "smallint", nullable: false),
                    SameJackpot4 = table.Column<short>(type: "smallint", nullable: false),
                    SameJackpot5 = table.Column<short>(type: "smallint", nullable: false),
                    SameJackpot6 = table.Column<short>(type: "smallint", nullable: false),
                    EdgeHighLow = table.Column<short>(type: "smallint", nullable: false),
                    EdgeGroup = table.Column<short>(type: "smallint", nullable: false),
                    EdgeNumber = table.Column<short>(type: "smallint", nullable: false),
                    SeriesNumber = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_lion_analysis", x => x.SequenceNo);
                });

            migrationBuilder.CreateTable(
                name: "tb_lion_choice",
                columns: table => new
                {
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    LoginId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    ChoiceNo = table.Column<int>(type: "int", nullable: false),
                    Digit1 = table.Column<short>(type: "smallint", nullable: false),
                    Digit2 = table.Column<short>(type: "smallint", nullable: false),
                    Digit3 = table.Column<short>(type: "smallint", nullable: false),
                    Digit4 = table.Column<short>(type: "smallint", nullable: false),
                    Digit5 = table.Column<short>(type: "smallint", nullable: false),
                    Digit6 = table.Column<short>(type: "smallint", nullable: false),
                    IsMailSent = table.Column<bool>(type: "bit", nullable: false),
                    Ranking = table.Column<short>(type: "smallint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_lion_choice", x => new { x.SequenceNo, x.LoginId, x.ChoiceNo });
                });

            migrationBuilder.CreateTable(
                name: "tb_lion_factor",
                columns: table => new
                {
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    LNoSampling = table.Column<short>(type: "smallint", nullable: false),
                    LNoCombination = table.Column<int>(type: "int", nullable: false),
                    LNoExtraction = table.Column<int>(type: "int", nullable: false),
                    RNoSampling = table.Column<short>(type: "smallint", nullable: false),
                    RNoCombination = table.Column<int>(type: "int", nullable: false),
                    RNoExtraction = table.Column<int>(type: "int", nullable: false),
                    OddSelection = table.Column<bool>(type: "bit", nullable: false),
                    HighSelection = table.Column<bool>(type: "bit", nullable: false),
                    MinOddEven = table.Column<short>(type: "smallint", nullable: false),
                    MaxOddEven = table.Column<short>(type: "smallint", nullable: false),
                    MinHighLow = table.Column<short>(type: "smallint", nullable: false),
                    MaxHighLow = table.Column<short>(type: "smallint", nullable: false),
                    MaxSumFrequence = table.Column<short>(type: "smallint", nullable: false),
                    MaxGroupBalance = table.Column<short>(type: "smallint", nullable: false),
                    MaxLastNumber = table.Column<short>(type: "smallint", nullable: false),
                    MaxSeriesNumber = table.Column<short>(type: "smallint", nullable: false),
                    MaxSameDigits = table.Column<short>(type: "smallint", nullable: false),
                    MaxSameJackpot = table.Column<short>(type: "smallint", nullable: false),
                    NoJackpot1 = table.Column<int>(type: "int", nullable: false),
                    WinningAmount1 = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    NoJackpot2 = table.Column<int>(type: "int", nullable: false),
                    WinningAmount2 = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    NoJackpot3 = table.Column<int>(type: "int", nullable: false),
                    WinningAmount3 = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    NoJackpot4 = table.Column<int>(type: "int", nullable: false),
                    WinningAmount4 = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    NoJackpot5 = table.Column<int>(type: "int", nullable: false),
                    WinningAmount5 = table.Column<decimal>(type: "decimal(12,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_lion_factor", x => x.SequenceNo);
                });

            migrationBuilder.CreateTable(
                name: "tb_lion_jackpot",
                columns: table => new
                {
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    LoginId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    NoChoice = table.Column<short>(type: "smallint", nullable: false),
                    Digit1 = table.Column<short>(type: "smallint", nullable: false),
                    Digit2 = table.Column<short>(type: "smallint", nullable: false),
                    Digit3 = table.Column<short>(type: "smallint", nullable: false),
                    NoJackpot = table.Column<int>(type: "int", nullable: false),
                    WinningAmount = table.Column<decimal>(type: "decimal(12,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_lion_jackpot", x => new { x.SequenceNo, x.LoginId });
                });

            migrationBuilder.CreateTable(
                name: "tb_lion_member",
                columns: table => new
                {
                    LoginId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    LoginName = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    LoginPassword = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    DeviceType = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    IsAlive = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    MailError = table.Column<bool>(type: "bit", nullable: false),
                    IsMailSend = table.Column<bool>(type: "bit", nullable: false),
                    IsDirectSend = table.Column<bool>(type: "bit", nullable: false),
                    IsNumberChoice = table.Column<bool>(type: "bit", nullable: false),
                    MaxSelectNumber = table.Column<short>(type: "smallint", nullable: false),
                    Digit1 = table.Column<short>(type: "smallint", nullable: false),
                    Digit2 = table.Column<short>(type: "smallint", nullable: false),
                    Digit3 = table.Column<short>(type: "smallint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_lion_member", x => x.LoginId);
                });

            migrationBuilder.CreateTable(
                name: "tb_lion_notify",
                columns: table => new
                {
                    LoginId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    NotifyTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_lion_notify", x => new { x.LoginId, x.NotifyTime });
                });

            migrationBuilder.CreateTable(
                name: "tb_lion_percent",
                columns: table => new
                {
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    LJackpot01 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect01 = table.Column<short>(type: "smallint", nullable: false),
                    LJackpot02 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect02 = table.Column<short>(type: "smallint", nullable: false),
                    LJackpot03 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect03 = table.Column<short>(type: "smallint", nullable: false),
                    LJackpot04 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect04 = table.Column<short>(type: "smallint", nullable: false),
                    LJackpot05 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect05 = table.Column<short>(type: "smallint", nullable: false),
                    LJackpot06 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect06 = table.Column<short>(type: "smallint", nullable: false),
                    LJackpot07 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect07 = table.Column<short>(type: "smallint", nullable: false),
                    LJackpot08 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect08 = table.Column<short>(type: "smallint", nullable: false),
                    LJackpot09 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect09 = table.Column<short>(type: "smallint", nullable: false),
                    LJackpot10 = table.Column<short>(type: "smallint", nullable: false),
                    LSelect10 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot01 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect01 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot02 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect02 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot03 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect03 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot04 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect04 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot05 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect05 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot06 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect06 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot07 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect07 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot08 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect08 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot09 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect09 = table.Column<short>(type: "smallint", nullable: false),
                    RJackpot10 = table.Column<short>(type: "smallint", nullable: false),
                    RSelect10 = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_lion_percent", x => x.SequenceNo);
                });

            migrationBuilder.CreateTable(
                name: "tb_lion_select",
                columns: table => new
                {
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    SelectNo = table.Column<int>(type: "int", nullable: false),
                    Digit1 = table.Column<short>(type: "smallint", nullable: false),
                    Digit2 = table.Column<short>(type: "smallint", nullable: false),
                    Digit3 = table.Column<short>(type: "smallint", nullable: false),
                    Digit4 = table.Column<short>(type: "smallint", nullable: false),
                    Digit5 = table.Column<short>(type: "smallint", nullable: false),
                    Digit6 = table.Column<short>(type: "smallint", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsLeft = table.Column<bool>(type: "bit", nullable: false),
                    Ranking = table.Column<short>(type: "smallint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_lion_select", x => new { x.SequenceNo, x.SelectNo });
                });

            migrationBuilder.CreateTable(
                name: "tb_lion_winner",
                columns: table => new
                {
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Digit1 = table.Column<short>(type: "smallint", nullable: false),
                    Digit2 = table.Column<short>(type: "smallint", nullable: false),
                    Digit3 = table.Column<short>(type: "smallint", nullable: false),
                    Digit4 = table.Column<short>(type: "smallint", nullable: false),
                    Digit5 = table.Column<short>(type: "smallint", nullable: false),
                    Digit6 = table.Column<short>(type: "smallint", nullable: false),
                    Digit7 = table.Column<short>(type: "smallint", nullable: false),
                    AutoSelect = table.Column<short>(type: "smallint", nullable: false),
                    Count1 = table.Column<int>(type: "int", nullable: false),
                    Amount1 = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    Count2 = table.Column<int>(type: "int", nullable: false),
                    Amount2 = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    Count3 = table.Column<int>(type: "int", nullable: false),
                    Amount3 = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    Count4 = table.Column<int>(type: "int", nullable: false),
                    Amount4 = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    Count5 = table.Column<int>(type: "int", nullable: false),
                    Amount5 = table.Column<decimal>(type: "decimal(12,0)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_lion_winner", x => x.SequenceNo);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_lion_analysis");

            migrationBuilder.DropTable(
                name: "tb_lion_choice");

            migrationBuilder.DropTable(
                name: "tb_lion_factor");

            migrationBuilder.DropTable(
                name: "tb_lion_jackpot");

            migrationBuilder.DropTable(
                name: "tb_lion_member");

            migrationBuilder.DropTable(
                name: "tb_lion_notify");

            migrationBuilder.DropTable(
                name: "tb_lion_percent");

            migrationBuilder.DropTable(
                name: "tb_lion_select");

            migrationBuilder.DropTable(
                name: "tb_lion_winner");
        }
    }
}
