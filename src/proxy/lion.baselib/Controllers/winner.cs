using LottoLion.BaseLib.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LottoLion.BaseLib.Controllers
{
    public class WinnerReader
    {
        private static DateTime _start_day = DateTime.MinValue;

        public static DateTime LottoLion_StartDay
        {
            get
            {
                if (_start_day == DateTime.MinValue)
                    _start_day = new DateTime(2002, 12, 7);

                return _start_day;
            }
        }

        public static int GetNextWeekSequenceNo()
        {
            return GetThisWeekSequenceNo() + 1;
        }

        public static int GetThisWeekSequenceNo()
        {
            return GetSequenceNoByDate(DateTime.Now.Date);
        }

        public static int GetSequenceNoByDate(DateTime target_day)
        {
            return (target_day.Subtract(LottoLion_StartDay).Days - 1) / 7 + 1;
        }

        public static DateTime GetThisWeekIssueDate()
        {
            return GetIssueDateBySequenceNo(GetThisWeekSequenceNo());
        }

        public static DateTime GetNextWeekIssueDate()
        {
            return GetIssueDateBySequenceNo(GetNextWeekSequenceNo());
        }

        public static DateTime GetIssueDateBySequenceNo(int sequence_no)
        {
            return LottoLion_StartDay.AddDays((sequence_no - 1) * 7);
        }

        public static DateTime GetThisWeekPaymentDate()
        {
            return GetThisWeekIssueDate().AddYears(1);
        }

        public static DateTime GetNextWeekPaymentDate()
        {
            return GetNextWeekIssueDate().AddYears(1);
        }

        public static DateTime GetPaymentDateBySequenceNo(int sequence_no)
        {
            return GetIssueDateBySequenceNo(sequence_no).AddYears(1);
        }

        private static HttpClientHandler __http_handler = null;

        private async Task<HtmlAgilityPack.HtmlDocument> GetHtmlDocument(int sequnce_no)
        {
            //var _site1 = @"http://www.645lotto.net/Confirm/number.asp?sltSeq=" + sequnce_no;               // 2013-12-04 웹사이트 변경 됨
            //var _site2 = @"http://www.nlotto.co.kr/lotto645Confirm.do?method=byWin&drwNo=" + sequnce_no;
            //var _site3 = @"http://www.nlotto.co.kr/gameResult.do?method=byWin&drwNo=" + sequnce_no;
            //var _site4 = @"https://www.nlotto.co.kr/gameResult.do?method=byWin&drwNo=" + sequnce_no;       // 2018-12-09: https로 변경 됨
            var _site5 = @"https://dhlottery.co.kr/gameResult.do?method=byWin&drwNo={sequnce_no}";           // 2019-03-31: nlotto 사이트가 동작 안함, Html파싱은 site4와 동일

            var _result = new HtmlAgilityPack.HtmlDocument
            {
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true
            };

            //if (__http_handler == null)
            {
                __http_handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    }
                };
            }

            using (var _client = new HttpClient(__http_handler))
            {
                var _html_bytes = await _client.GetByteArrayAsync(_site5);

                var _html_text = Encoding.GetEncoding("euc-kr").GetString(_html_bytes, 0, _html_bytes.Length - 1);
                _result.LoadHtml(_html_text);

                if (_result.DocumentNode == null)
                    throw new Exception($"'{_site5}'에서 {sequnce_no} 회차 정보를 읽는 중 오류가 발생 하였습니다.");
            }

            return _result;
        }

        private List<string> GetWinnerBall(HtmlAgilityPack.HtmlDocument html_document, int sequnce_no)
        {
            var _result = new List<string>();

            var _divs = html_document.DocumentNode.SelectNodes("//div");
            if (_divs == null || _divs.Count < 1)
                throw new Exception($"{sequnce_no} 회차 당첨 번호 <div> 검색 중 오류가 발생 하였습니다.");

            var winner_division = _divs
                            .Where(_d => _d.Attributes.FirstOrDefault(_a => _a.Name == "class" && _a.Value == "win_result") != null)
                            .FirstOrDefault();

            if (winner_division == null)
                throw new Exception($"{sequnce_no} 회차 당첨 번호 <class> 검색 중 오류가 발생 하였습니다.");

            var _seqno = winner_division
                            .ChildNodes.FirstOrDefault(x => x.Name == "h4")
                            .ChildNodes.FirstOrDefault(x => x.Name == "strong")
                            .InnerText;

            if (String.IsNullOrEmpty(_seqno) || Convert.ToInt32(_seqno.Replace("회", "")) != sequnce_no)
                throw new Exception($"{sequnce_no} 회차를 찾을 수 없습니다.");

            var _found_number = 0;

            foreach (var _pnode in winner_division.ChildNodes)
            {
                if (_pnode.Name != "div" || _pnode.Attributes[0].Name != "class" || _pnode.Attributes[0].Value != "nums")
                    continue;

                foreach (var _node in _pnode.ChildNodes)
                {
                    if (_node.Name == "div" && _node.Attributes[0].Name == "class" && _node.Attributes[0].Value == "num win")
                    {
                        var _balls = _node.ChildNodes.Where(n => n.Name == "p").FirstOrDefault();
                        if (_balls != null)
                        {
                            foreach (var _node2 in _balls.ChildNodes)
                            {
                                if (_node2.Name != "span")
                                    continue;

                                var _b = _node2.InnerText;

                                _result.Add(_b);
                                _found_number++;
                            }
                        }
                    }

                    if (_node.Name == "div" && _node.Attributes[0].Name == "class" && _node.Attributes[0].Value == "num bonus")
                    {
                        var _balls = _node.ChildNodes.Where(n => n.Name == "p").FirstOrDefault();
                        if (_balls != null)
                        {
                            foreach (var _node2 in _balls.ChildNodes)
                            {
                                if (_node2.Name != "span")
                                    continue;

                                var _b = _node2.InnerText;

                                _result.Add(_b);
                                _found_number++;
                            }
                        }
                    }
                }
            }

            if (_found_number != 7)
                throw new Exception($"{sequnce_no} 당첨 번호 추출 중 오류가 발생 하였습니다.");

            return _result;
        }

        private List<string> GetWinnerInfo(HtmlAgilityPack.HtmlDocument html_document, int sequnce_no)
        {
            var _result = new List<string>();

            var _tables = html_document.DocumentNode.SelectNodes("//table");
            if (_tables == null || _tables.Count < 1)
                throw new Exception($"{sequnce_no} 당첨 정보 <table> 추출 중 오류가 발생 하였습니다.");

            var _mt40 = _tables
                   .Where(_d => _d.Attributes.FirstOrDefault(_a => _a.Name == "class" && _a.Value == "tbl_data tbl_data_col") != null)
                   .FirstOrDefault();

            if (_mt40 == null)
                throw new Exception($"{sequnce_no} 당첨 정보 <table> 추출 중 오류가 발생 하였습니다.");

            var _winner_order = 1;

            foreach (var _node in _mt40.SelectNodes("//tbody")[0].SelectNodes("//tr"))
            {
                if (_node.ParentNode.Name != "tbody" || _node.ChildNodes.Count < 11)
                    continue;

                var _builder = new List<string>();

                foreach (var _td in _node.ChildNodes)
                {
                    if (_td.Name == "td")
                        _builder.Add(_td.InnerText);
                }

                if (_winner_order == 1)
                {
                    var _autoSelector = 0;

                    if (_builder.Count > 5)
                    {
                        var _remark = _builder[5].ToString().Split(' ');
                        foreach (var _r in _remark)
                        {
                            if (_r.Contains("\t자동") == true)
                            {
                                var _pos = _r.LastIndexOf("자동");
                                _autoSelector = Convert.ToInt32(_r.Substring(_pos + 2));

                                break;
                            }
                        }
                    }

                    _result.Add(_autoSelector.ToString());
                }

                var _noPerson = _builder[2].ToString().Replace(",", "");
                var _noAmount = _builder[3].ToString().Replace(",", "").Replace("원", "");

                for (var i = 0; i < 5; i++)
                    _result.Add(_builder[i]);

                _result.Add(_noPerson);
                _result.Add(_noAmount);

                _winner_order++;
            }

            if (_result.Count() != 36)
                throw new Exception($"{sequnce_no} 당첨 정보 추출 중 오류가 발생 하였습니다.");

            return _result;
        }

        public async Task<TbLionWinner> ReadWinnerBall(int sequnce_no)
        {
            var _result = (TbLionWinner)null;

            var _html_document = await GetHtmlDocument(sequnce_no);
            {
                var _winner_ball = GetWinnerBall(_html_document, sequnce_no);
                var _winner_info = GetWinnerInfo(_html_document, sequnce_no);

                var _issue_day = GetIssueDateBySequenceNo(sequnce_no);

                _result = new TbLionWinner()
                {
                    SequenceNo = sequnce_no,

                    IssueDate = _issue_day,
                    PaymentDate = _issue_day.AddYears(1),

                    Digit1 = Convert.ToInt16(_winner_ball[0]),
                    Digit2 = Convert.ToInt16(_winner_ball[1]),
                    Digit3 = Convert.ToInt16(_winner_ball[2]),
                    Digit4 = Convert.ToInt16(_winner_ball[3]),
                    Digit5 = Convert.ToInt16(_winner_ball[4]),
                    Digit6 = Convert.ToInt16(_winner_ball[5]),
                    Digit7 = Convert.ToInt16(_winner_ball[6]),

                    AutoSelect = Convert.ToInt16(_winner_info[0]),

                    Count1 = Convert.ToInt32(_winner_info[6]),
                    Count2 = Convert.ToInt32(_winner_info[13]),
                    Count3 = Convert.ToInt32(_winner_info[20]),
                    Count4 = Convert.ToInt32(_winner_info[27]),
                    Count5 = Convert.ToInt32(_winner_info[34]),

                    Amount1 = Convert.ToDecimal(_winner_info[7]),
                    Amount2 = Convert.ToDecimal(_winner_info[14]),
                    Amount3 = Convert.ToDecimal(_winner_info[21]),
                    Amount4 = Convert.ToDecimal(_winner_info[28]),
                    Amount5 = Convert.ToDecimal(_winner_info[35]),

                    Remark = ""
                };
            }

            return _result;
        }
    }
}