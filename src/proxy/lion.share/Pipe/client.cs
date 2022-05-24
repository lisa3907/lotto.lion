using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Data.DTO;
using Newtonsoft.Json;
using System.IO.Pipes;
using System.Security.Principal;

namespace Lion.Share.Pipe
{
    public class PipeClient
    {
        private static LConfig __cconfig = new LConfig();

        public string LottoSelectmasterId
        {
            get
            {
                return __cconfig.GetAppString("lotto.select.master.id");
            }
        }

        private async Task<bool> RequestToServer<T>(VmsRequest<T> request, string queueName)
        {
            var _result = false;

            using (var _pipe_client = new NamedPipeClientStream(".", queueName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation))
            {
                _pipe_client.Connect(1000);

                var _reader = new StreamReader(_pipe_client);
                var _writer = new StreamWriter(_pipe_client);

                var _packet = JsonConvert.SerializeObject(request);

                await _writer.WriteLineAsync(_packet);
                await _writer.FlushAsync();

                var _read_line = await _reader.ReadLineAsync();
                _pipe_client.Close();

                var _response = JsonConvert.DeserializeObject<VmsResponse<bool>>(_read_line);
                _result = _response.data;
            }

            return _result;
        }

        public async Task<bool> RequestToAnalystQ(dSelector select)
        {
            return await RequestToServer(new VmsRequest<dSelector>
            {
                command = QCommand.AnalystQ,
                data = select
            },
            PQueue.ReceiveServerPipeName
            );
        }

        public async Task<bool> RequestToChoicerQ(dChoice choice)
        {
            return await RequestToServer(new VmsRequest<dChoice>
            {
                command = QCommand.ChoicerQ,
                data = choice
            },
            PQueue.ReceiveServerPipeName
            );
        }

        public async Task<bool> RequestToSelectQ(dSelector select)
        {
            return await RequestToServer(new VmsRequest<dSelector>
            {
                command = QCommand.SelectorQ,
                data = select
            },
            PQueue.ReceiveServerPipeName
            );
        }

        public async Task<bool> RequestToWinnerQ(dSelector select)
        {
            return await RequestToServer(new VmsRequest<dSelector>
            {
                command = QCommand.WinnerQ,
                data = select
            },
            PQueue.ReceiveServerPipeName
            );
        }

        public async Task<int> SelectMemberAsync(AppDbContext ltctx, WinnerReader reader, int selectNo, bool resend)
        {
            var _result = 0;

            var _today_winner_no = reader.GetThisWeekSequenceNo();

            // 현재의 예상 번호가 select(추출) 되어 있으면, choice(선택) 한다.
            // analysis(분석) 차수와 같은 차수이면 select(추출)이 있다고 가정한다.
            if (_today_winner_no == selectNo - 1)
            {
                var _digits = new List<short> { 0, 0, 0 };
                {
                    var _master = ltctx.tb_lion_member
                                        .Where(m => m.LoginId == LottoSelectmasterId)
                                        .SingleOrDefault();

                    if (_master != null)
                    {
                        var _offset = 0;

                        if (_master.Digit1 > 0)
                        {
                            if (!_digits.Contains(_master.Digit1))
                            {
                                _digits[_offset] = _master.Digit1;
                                _offset++;
                            }
                        }

                        if (_master.Digit2 > 0)
                        {
                            if (!_digits.Contains(_master.Digit2))
                            {
                                _digits[_offset] = _master.Digit2;
                                _offset++;
                            }
                        }

                        if (_master.Digit3 > 0)
                        {
                            if (!_digits.Contains(_master.Digit3))
                            {
                                _digits[_offset] = _master.Digit3;
                                _offset++;
                            }
                        }
                    }
                }

                // 가입 member(회원)들에게 각자에게 할당 된 갯수만큼을 select(추출)테이블에서 choice(선택) 테이블로
                // 복사 후 메일 발송 하도록 큐에 명령을 보냅니다. (여기서 처리 하지 않고 memberQ에서 pop하여 처리 함)
                var _members = ltctx.tb_lion_member
                                .Where(m => m.IsNumberChoice == true && m.IsMailSend == true && m.IsAlive == true && m.MailError == false)
                                .ToList();

                foreach (var _member in _members)
                {
                    var _no_choice = ltctx.tb_lion_choice
                                        .Where(c => c.SequenceNo == selectNo && c.LoginId == _member.LoginId)
                                        .GroupBy(c => c.IsMailSent)
                                        .Select(n => new { b = n.Key, c = n.Count() })
                                        .ToList();

                    var _no_choice_t = _no_choice.Where(c => c.b == true).Sum(c => c.c);
                    var _no_choice_f = _no_choice.Where(c => c.b == false).Sum(c => c.c);

                    // 기 선택 하였으면 skip 함
                    if (_no_choice_t + _no_choice_f <= 0 || _no_choice_f > 0)
                    {
                        var _choice = new dChoice()
                        {
                            sequence_no = selectNo,
                            login_id = _member.LoginId,
                            resend = resend || _no_choice_f > 0,
                            digit1 = _digits[0],
                            digit2 = _digits[1],
                            digit3 = _digits[2]
                        };

                        // member(회원)들에게 메일 발송 하도록 큐에 명령을 보냅니다.
                        await this.RequestToChoicerQ(_choice);

                        _result++;
                    }
                }
            }

            return _result;
        }

        public async Task<int> AnalysisSelectAsync(AppDbContext ltctx, int select_no)
        {
            var _result = 0;

            // 분석 테이블의 마지막 회차+1에 해당하는 회차의 예상 번호를 추출 합니다.
            var _select_no = ltctx.tb_lion_select.Max(x => x.SequenceNo);

            for (_select_no++; _select_no <= select_no + 1; _select_no++)
            {
                var _selection = new dSelector()
                {
                    sequence_no = _select_no,
                    sent_by_queue = true
                };

                await this.RequestToSelectQ(_selection);

                _result++;
            }

            return _result;
        }

        /// <summary>
        /// analysisQ
        /// </summary>
        /// <param name="ltctx"></param>
        /// <param name="winner_no"></param>
        /// <returns></returns>
        public async Task<int> WinnerCollectorAsync(AppDbContext ltctx, int winner_no)
        {
            var _result = 0;

            var _sequence_nos = new List<int>();

            var _analysis_no = ltctx.tb_lion_analysis.Max(x => x.SequenceNo);
            for (_analysis_no++; _analysis_no <= winner_no; _analysis_no++)
                _sequence_nos.Add(_analysis_no);

            var _select_nos = ltctx.tb_lion_select
                                    .Where(s => s.SequenceNo <= winner_no && s.Ranking == 0)
                                    .GroupBy(s => s.SequenceNo)
                                    .Select(s => s.Key)
                                    .ToList();
            foreach (var _s in _select_nos)
            {
                if (_sequence_nos.Exists(x => x == _s) == false)
                    _sequence_nos.Add(_s);
            }

            var _choice_nos = ltctx.tb_lion_choice
                                    .Where(c => c.SequenceNo <= winner_no && c.Ranking == 0)
                                    .GroupBy(c => c.SequenceNo)
                                    .Select(c => c.Key)
                                    .ToList();
            foreach (var _c in _choice_nos)
            {
                if (_sequence_nos.Exists(x => x == _c) == false)
                    _sequence_nos.Add(_c);
            }

            foreach (var _n in _sequence_nos)
            {
                var _selection = new dSelector()
                {
                    sequence_no = _n,
                    sent_by_queue = true
                };

                await RequestToAnalystQ(_selection);

                _result++;
            }

            return _result;
        }
    }
}