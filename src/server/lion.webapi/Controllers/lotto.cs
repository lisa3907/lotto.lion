using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Data.DTO;
using Lion.Share.Data.Models;
using Lion.Share.Options;
using Lion.Share.Pipe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OdinSdk.BaseLib.WebApi;

namespace Lion.WebApi.Controllers
{
    [Route("api/[controller]")]
    public partial class LottoController : Controller
    {
        private readonly UserManager __usermgr;
        private readonly AppDbContext __db_context;
        private readonly WinnerReader __winner_reader;
        private readonly PrizeReader __prize_reader;
        private readonly PipeClient __pipe_client;
        private readonly dForcast __prize_forcast;

        public LottoController(IOptions<JwtIssuerOptions> jwtOptions, AppDbContext dbContext, WinnerReader winnerReader, PrizeReader prizeReader, PipeClient pipeClient, dForcast forcast)
        {
            __usermgr = new UserManager(jwtOptions.Value);
            __db_context = dbContext;

            __winner_reader = winnerReader;
            __prize_reader = prizeReader;            

            __pipe_client = pipeClient;
            __prize_forcast = forcast;
        }

        [Route("GetThisWeekPrize")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> GetThisWeekPrize()
        {
            return await CProxy.UsingAsync(async () =>
            {
                var _result = (success: false, message: "ok");

                if (
                    __prize_forcast.PredictAmount == 0
                    ||
                    DateTime.Now.Subtract(__prize_forcast.LastReadTime).TotalMinutes > __prize_reader.PrizeReadIntervalMinutes
                   )
                {
                    var _forcast = await __prize_reader.ReadPrizeForcast(__winner_reader);

                    __prize_forcast.LastReadTime = _forcast.LastReadTime;
                    __prize_forcast.NextReadTime = _forcast.NextReadTime;
                    __prize_forcast.ReadInterval = _forcast.ReadInterval;
                    __prize_forcast.SequenceNo = _forcast.SequenceNo;
                    __prize_forcast.IssueDate = _forcast.IssueDate;
                    __prize_forcast.PredictAmount = _forcast.PredictAmount;
                    __prize_forcast.SalesAmount = _forcast.SalesAmount;

                    //Console.WriteLine($"prize_forcast: {__prize_forcast.PredictAmount}");
                }

                _result.success = true;

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = __prize_forcast
                });
            });
        }

        [Route("GetPrizeBySeqNo")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> GetPrizeBySeqNo(int sequence_no)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _winner = (mWinner)null;
                if (sequence_no >= 1 && sequence_no <= __winner_reader.GetThisWeekSequenceNo())
                {
                    _winner = __db_context.tb_lion_winner
                                              .Where(w => w.SequenceNo == sequence_no)
                                              .SingleOrDefault();

                    if (_winner == null)
                    {
                        _result.message = $"해당 회차'{sequence_no}'의 추첨 정보가 없습니다";

                        _winner = new mWinner()
                        {
                            SequenceNo = sequence_no,
                            IssueDate = __winner_reader.GetIssueDateBySequenceNo(sequence_no),
                            PaymentDate = __winner_reader.GetPaymentDateBySequenceNo(sequence_no)
                        };
                    }
                    else
                        _result.success = true;
                }
                else
                    _result.message = "추첨 회차가 범위를 벗어 났습니다";

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = _winner
                });
            });
        }

        [Route("GetPrizeList")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> GetPrizeList(int sequence_no, int limit = 100)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _winners = new List<mWinner>();
                if (sequence_no >= 1 && sequence_no <= __winner_reader.GetThisWeekSequenceNo())
                {
                    _winners = __db_context.tb_lion_winner
                                              .Where(w => w.SequenceNo <= sequence_no)
                                              .OrderByDescending(w => w.SequenceNo)
                                              .Take(limit)
                                              .ToList();

                    _result.success = true;
                }
                else
                    _result.message = "첫 추첨 회차가 범위를 벗어 났습니다";

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = _winners
                });
            });
        }

        [Route("GetAnalysisBySeqNo")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> GetAnalysisBySeqNo(int sequence_no)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _analysis = (mAnalysis)null;
                if (sequence_no >= 1 && sequence_no <= __winner_reader.GetThisWeekSequenceNo())
                {
                    _analysis = __db_context.tb_lion_analysis
                                              .Where(w => w.SequenceNo == sequence_no)
                                              .SingleOrDefault();

                    if (_analysis == null)
                    {
                        _result.message = $"해당 회차'{sequence_no}'의 분석 정보가 없습니다";

                        _analysis = new mAnalysis()
                        {
                            SequenceNo = sequence_no
                        };
                    }
                    else
                        _result.success = true;
                }
                else
                    _result.message = "분석 회차가 범위를 벗어 났습니다";

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = _analysis
                });
            });
        }

        [Route("GetAnalysisList")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> GetAnalysisList(int skip_no, int limit = 100)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _analysis = __db_context.tb_lion_analysis
                                            .OrderByDescending(p => p.SequenceNo)
                                            .Skip(skip_no)
                                            .Take(limit)
                                            .ToList();

                if (_analysis.Count == 0)
                {
                    _result.message = $"분석 정보가 없습니다";
                }
                else
                    _result.success = true;

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = _analysis
                });
            });
        }

        [Route("GetPercentList")]
        [Authorize(Policy = "LottoLionUsers")]
        [HttpPost]
        public async Task<IActionResult> GetPercentList(int skip_no, int limit = 100)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _analysis = __db_context.tb_lion_percent
                                            .OrderByDescending(p => p.SequenceNo)
                                            .Skip(skip_no)
                                            .Take(limit)
                                            .ToList();

                if (_analysis.Count == 0)
                {
                    _result.message = $"분석 정보가 없습니다";
                }
                else
                    _result.success = true;

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = _analysis
                });
            });
        }

        private List<dJackpot> GetJackpotList(List<mChoice> choices)
        {
            var _result = new List<dJackpot>();

            if (choices.Count > 0)
            {
                var _last_seqno = 0;
                var _winner = (mWinner)null;

                foreach (var _c in choices)
                {
                    var _j = new dJackpot()
                    {
                        SequenceNo = _c.SequenceNo,
                        LoginId = _c.LoginId,
                        ChoiceNo = _c.ChoiceNo,

                        Digit1 = _c.Digit1,
                        Digit2 = _c.Digit2,
                        Digit3 = _c.Digit3,
                        Digit4 = _c.Digit4,
                        Digit5 = _c.Digit5,
                        Digit6 = _c.Digit6,

                        IsMailSent = _c.IsMailSent,
                        Ranking = _c.Ranking,
                        Amount = _c.Amount,
                        Remark = _c.Remark
                    };

                    if (_last_seqno != _c.SequenceNo)
                    {
                        _last_seqno = _c.SequenceNo;

                        _winner = __db_context.tb_lion_winner
                                      .Where(w => w.SequenceNo == _last_seqno)
                                      .SingleOrDefault();
                    }

                    if (_winner != null)
                    {
                        var _winner_digits = new List<short>();
                        {
                            _winner_digits.Add(_winner.Digit1);
                            _winner_digits.Add(_winner.Digit2);
                            _winner_digits.Add(_winner.Digit3);
                            _winner_digits.Add(_winner.Digit4);
                            _winner_digits.Add(_winner.Digit5);
                            _winner_digits.Add(_winner.Digit6);
                        }

                        _j.jackpot1 = _winner_digits.Exists(x => x == _j.Digit1) || (_j.Ranking == 2 && _j.Digit1 == _winner.Digit7);
                        _j.jackpot2 = _winner_digits.Exists(x => x == _j.Digit2) || (_j.Ranking == 2 && _j.Digit2 == _winner.Digit7);
                        _j.jackpot3 = _winner_digits.Exists(x => x == _j.Digit3) || (_j.Ranking == 2 && _j.Digit3 == _winner.Digit7);
                        _j.jackpot4 = _winner_digits.Exists(x => x == _j.Digit4) || (_j.Ranking == 2 && _j.Digit4 == _winner.Digit7);
                        _j.jackpot5 = _winner_digits.Exists(x => x == _j.Digit5) || (_j.Ranking == 2 && _j.Digit5 == _winner.Digit7);
                        _j.jackpot6 = _winner_digits.Exists(x => x == _j.Digit6) || (_j.Ranking == 2 && _j.Digit6 == _winner.Digit7);
                    }

                    _result.Add(_j);
                }
            }

            return _result;
        }

        [Route("GetUserChoices")]
        [Authorize(Policy = "LottoLionMember")]
        [HttpPost]
        public async Task<IActionResult> GetUserChoices(int sequence_no)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _jackpots = new List<dJackpot>();
                {
                    var _choices = new List<mChoice>();

                    var _login_id = __usermgr.GetLoginId(Request);
                    if (String.IsNullOrEmpty(_login_id) == false)
                    {
                        _choices = __db_context.tb_lion_choice
                                            .Where(c => c.LoginId == _login_id && c.SequenceNo == sequence_no)
                                            .OrderByDescending(c => c.Amount)
                                            .ThenBy(c => c.Digit1).ThenBy(c => c.Digit2).ThenBy(c => c.Digit3)
                                            .ThenBy(c => c.Digit4).ThenBy(c => c.Digit5).ThenBy(c => c.Digit6)
                                            .ToList();

                        if (_choices.Count > 0)
                        {
                            _jackpots.AddRange(GetJackpotList(_choices));
                            _result.success = true;
                        }
                        else
                            _result.message = $"회원ID '{_login_id}'님의 '{sequence_no}'회차 추출 번호가 없습니다";
                    }
                    else
                        _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";
                }

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = _jackpots
                });
            });
        }

        [Route("GetChoiceList")]
        [Authorize(Policy = "LottoLionMember")]
        [HttpPost]
        public async Task<IActionResult> GetChoiceList(int skip_no, int limit = 100)
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _count = 0;
                var _limit = 0;

                var _jackpots = new List<dJackpot>();
                {
                    var _login_id = __usermgr.GetLoginId(Request);
                    if (String.IsNullOrEmpty(_login_id) == false)
                    {
                        _count = __db_context.tb_lion_choice
                                                  .Where(c => c.LoginId == _login_id && c.Ranking > 0 && c.Ranking < 6)
                                                  .Count();

                        var _choices = __db_context.tb_lion_choice
                                                    .Where(c => c.LoginId == _login_id && c.Ranking > 0 && c.Ranking < 6)
                                                    .OrderByDescending(c => c.SequenceNo).ThenBy(c => c.ChoiceNo)
                                                    .Skip(skip_no)
                                                    .Take(limit)
                                                    .ToList();

                        _jackpots.AddRange(GetJackpotList(_choices));

                        _limit = _jackpots.Count;
                        if (_limit == 0)
                            _result.message = $"회원ID '{_login_id}'님의 추출 번호는 없습니다";
                        else
                            _result.success = true;
                    }
                    else
                        _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";
                }

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = new
                    {
                        count = _count,
                        limit = _limit,
                        rows = _jackpots
                    }
                });
            });
        }

        [Route("GetUserSequenceNos")]
        [Authorize(Policy = "LottoLionMember")]
        [HttpPost]
        public async Task<IActionResult> GetUserSequenceNos()
        {
            return await CProxy.UsingAsync(() =>
            {
                var _result = (success: false, message: "ok");

                var _sequence_nos = new List<dKeyValue>();
                {
                    var _login_id = __usermgr.GetLoginId(Request);
                    if (String.IsNullOrEmpty(_login_id) == false)
                    {
                        _sequence_nos = __db_context.tb_lion_choice
                                            .Where(c => c.LoginId == _login_id)
                                            .GroupBy(x => x.SequenceNo)
                                            .Select(
                                                y => new dKeyValue
                                                {
                                                    key = y.Key,
                                                    value = y.Count()
                                                }
                                            )
                                            .OrderByDescending(z => z.key)
                                            .ToList();

                        if (_sequence_nos.Count() < 1)
                        {
                            var _today_sequence_no = __winner_reader.GetThisWeekSequenceNo();
                            _sequence_nos.Add(new dKeyValue { key = _today_sequence_no, value = 0 });
                        }

                        _result.success = true;
                    }
                    else
                        _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";
                }

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = _sequence_nos
                });
            });
        }

        [Route("SendChoicedNumbers")]
        [Authorize(Policy = "LottoLionMember")]
        [HttpPost]
        public async Task<IActionResult> SendChoicedNumbers(int sequence_no)
        {
            return await CProxy.UsingAsync(async () =>
            {
                var _result = (success: false, message: "ok", choice: (dChoice)null);

                var _login_id = __usermgr.GetLoginId(Request);
                if (String.IsNullOrEmpty(_login_id) == false)
                {
                    _result.choice = new dChoice()
                    {
                        login_id = _login_id,
                        sequence_no = sequence_no,
                        resend = true
                    };

                    _result.success = await __pipe_client.RequestToChoicerQ(_result.choice);
                }
                else
                    _result.message = "인증 정보에서 회원ID를 찾을 수 없습니다";

                return Ok(new
                {
                    _result.success,
                    _result.message,

                    result = "" //_result.choice
                });
            });
        }
    }
}