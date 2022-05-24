using Lion.Share.Data.DTO;

namespace Lion.Share.Controllers
{
    public class PrizeReader
    {
        private static LConfig __cconfig = new LConfig();

        public int PrizeReadIntervalMinutes
        {
            get
            {
                return __cconfig.GetAppInteger("lotto.prize.read.minutes");
            }
        }

        private static HttpClientHandler __http_handler = null;

        private async Task<HtmlAgilityPack.HtmlDocument> GetHtmlDocument(string lotto645Url)
        {
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
                //var _html_bytes = await _client.GetByteArrayAsync(lotto645Url);
                //var _html_text = Encoding.GetEncoding("euc-kr").GetString(_html_bytes, 0, _html_bytes.Length - 1);

                var _html_text = await _client.GetStringAsync(lotto645Url);
                _result.LoadHtml(_html_text);

                if (_result.DocumentNode == null)
                    throw new Exception($"'{lotto645Url}'에서 읽는 중 오류가 발생 하였습니다.");
            }

            return _result;
        }

        private async Task<(decimal predict, decimal sales)> GetPrizeInfo(int version = 3)
        {
            var _result = (predict: 0m, sales: 0m);

            if (version == 2)
            {
                // 2018-12-09: https로 변경 됨
                var _site = @"https://www.nlotto.co.kr/common.do?method=main";
                var _html_document = await GetHtmlDocument(_site);

                var _divisions = _html_document.DocumentNode.SelectNodes("//div");
                if (_divisions == null || _divisions.Count < 1)
                    throw new Exception($"예상 정보 <div> 추출 중 오류가 발생 하였습니다.");

                var _next_game = _divisions
                                    .Where(_d => _d.Attributes.FirstOrDefault(_a => _a.Name == "class" && _a.Value == "next_time") != null)
                                    .FirstOrDefault();

                if (_next_game == null)
                    throw new Exception($"예상 정보 <div class=next_game> 추출 중 오류가 발생 하였습니다.");

                var _predict_amount = _next_game.ChildNodes[5].ChildNodes[1].ChildNodes[1].InnerText.Replace("원", "").Trim().Replace(",", "");
                _result.predict = Convert.ToDecimal(_predict_amount);

                var _sales_amount = _next_game.ChildNodes[5].ChildNodes[3].ChildNodes[1].InnerText.Replace("원", "").Trim().Replace(",", "");
                _result.sales = Convert.ToDecimal(_sales_amount);
            }
            else
            {
                // 2020/04/30 : 연금복권720+ 신상품 출시로 웹사이트가 리뉴얼 되었습니다.
                var _site = @"https://ol.dhlottery.co.kr/olotto/game/game645.do";
                var _html_document = await GetHtmlDocument(_site);
            }

            return _result;
        }

        public async Task<dForcast> ReadPrizeForcast(WinnerReader reader)
        {
            var _now_time = DateTime.Now;

            var _result = new dForcast()
            {
                PredictAmount = 0,
                SalesAmount = 0,

                SequenceNo = reader.GetNextWeekSequenceNo(),
                IssueDate = reader.GetNextWeekIssueDate(),

                LastReadTime = _now_time,
                NextReadTime = _now_time.AddMinutes(PrizeReadIntervalMinutes),
                ReadInterval = PrizeReadIntervalMinutes
            };

            var _prize_info = await GetPrizeInfo();
            {
                _result.PredictAmount = _prize_info.predict;
                _result.SalesAmount = _prize_info.sales;
            }

            return _result;
        }
    }
}