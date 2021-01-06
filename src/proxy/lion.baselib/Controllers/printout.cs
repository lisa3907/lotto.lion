using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Types;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace LottoLion.BaseLib.Controllers
{
    public class PrintOutLottoLion
    {
        private static LConfig __cconfig = new LConfig();

        // The A4 size print measures 8.27 (210mm) x 11.69(297mm) inches
        private const float __page_width = 8.27F * 96F;         // 96DPI *  8.27Inch: Inch당 96Pixel =  793.92

        private const float __page_height = 11.69F * 96F;       // 96DPI * 11.69Inch: Inch당 96Pixel = 1122.24

        // mm
        private const float __top_margin = 10F;                 // 가로 종이 위쪽 마진

        private const float __left_margin = 0F;                 // 가로 종이 좌측 마진

        // slip size print measures 74.80 (190mm) x 32.48 (82.5mm)
        private const float __slip_width = 209.5F;              // 96DPI * 190.0mm / 25.4: Inch당 96Pixel = 718.11 (211.2F)

        private const float __slip_height = 88.6F;              // 96DPI *  82.5mm / 25.4: Inch당 96Pixel = 311.81 ( 88.6F)

        private const float __top_position = 12.0F;             // 로또 종이 기준으로 위쪽 마진 (12.0F)
        private const float __left_position = 47.5F;            // 로또 종이 기준으로 좌측 마진 (48.0F)

        private const float __s_gap = 29.5F;                    // 한장에 5개의 번호가 출력 되고, 그 중 한개의 로또번호가 출력되는 영역의 사이즈
        private const float __y_gap = 6.90F;                   // 한개의 번호에 출력하는 Black-Blank의 Y축 크기 (48.2/(7+0)=6.6)
        private const float __x_gap = 3.70F;                   // 한개의 번호에 출력하는 Black-Blank의 X축 크기 (27.4/(7+1)=3.4)

        private const int __max_slip_per_page = 3;              // A4 한장에는 3개의 로또용지가 출력 합니다.
        private const int __max_number_per_slip = 5;            // 한장의 로또용지는 5개의 번호조합을 출력 합니다.

        private Image __black_marker = null;

        private Image BlackMarker
        {
            get
            {
                if (__black_marker == null)
                    __black_marker = Image.FromFile(Path.GetFullPath(@"images/black_marker.bmp"));
                return __black_marker;
            }
        }

        private Image __slip_paper = null;

        private Image SlipPaper
        {
            get
            {
                if (__slip_paper == null)
                    __slip_paper = Image.FromFile(Path.GetFullPath(@"images/lotto_slip_096_1529_0649.jpg"));
                return __slip_paper;
            }
        }

        private IEnumerable<Bitmap> PrintLottoSheet(TbLionChoice[] lott645_selector, int sequence_no, string login_id)
        {
            var _row_number = 0;

            do
            {
                // bitmap => pixels
                var _destination = new Bitmap((int)__page_width, (int)__page_height);

                var _lotto_sheet = Graphics.FromImage(_destination);
                {
                    _lotto_sheet.PageUnit = GraphicsUnit.Millimeter;
                    _lotto_sheet.Clear(Color.White);
                }

                var _top_margin = __top_margin;

                for (int i = 0; i < __max_slip_per_page; i++)
                {
                    var _rectangle = new RectangleF(__left_margin, _top_margin, __slip_width, __slip_height);
                    _lotto_sheet.DrawImage(SlipPaper, _rectangle);

                    // boxing
                    var _pen = new Pen(Color.Gray, 0.1f);
                    {
                        _pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
                        _pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
                        _pen.DashPattern = new float[] { 5.0f, 15.0f };

                        _lotto_sheet.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        _lotto_sheet.DrawRectangle(_pen, __left_margin, _top_margin, __slip_width, __slip_height);
                    }

                    for (int j = 0; j < __max_number_per_slip; j++)
                    {
                        var _s_row = lott645_selector[_row_number];
                        var _s_number = new short[]
                        {
                            _s_row.Digit1,
                            _s_row.Digit2,
                            _s_row.Digit3,
                            _s_row.Digit4,
                            _s_row.Digit5,
                            _s_row.Digit6
                        };

                        for (int k = 0; k < 6; k++)
                        {
                            var _number = _s_number[k];

                            var _y_position = __top_position + __top_margin + i * __slip_height;
                            var _x_position = __left_position + __left_margin + j * __s_gap;

                            var _y_offset = (_number - 1) / 7;
                            var _x_offset = (_number - _y_offset * 7) - 1;

                            _y_position += __y_gap * _y_offset;
                            _x_position += __x_gap * _x_offset;

                            _rectangle = new RectangleF(_x_position, _y_position, 2.0F, 3.5F);
                            _lotto_sheet.DrawImage(BlackMarker, _rectangle);
                        }

                        if (++_row_number >= lott645_selector.Length)
                        {
                            i = __max_slip_per_page;
                            break;
                        }
                    }

                    _top_margin += __slip_height;
                }

                yield return _destination;
            }
            while (_row_number < lott645_selector.Length);
        }

        private string ServiceName
        {
            get
            {
                return __cconfig.GetAppString("lotto.sender.service.name");
            }
        }

        private string __zip_folder = null;

        private string ZipFolder
        {
            get
            {
                if (__zip_folder == null)
                    __zip_folder = Path.Combine(Path.GetTempPath(), ServiceName);
                return __zip_folder;
            }
        }

        public string SaveLottoSheet(TbLionChoice[] lott645_selector, TChoice choice)
        {
            // location of files to compress
            var _zip_directory = "";
            {
                var _zip_packing = $"{ServiceName}_{choice.login_id}_{choice.sequence_no:0000}";

                var _user_directory = Path.Combine(ZipFolder, choice.login_id);
                {
                    if (Directory.Exists(_user_directory) == true)
                        Directory.Delete(_user_directory, true);

                    Directory.CreateDirectory(_user_directory);
                }

                _zip_directory = Path.Combine(_user_directory, _zip_packing);
                {
                    if (Directory.Exists(_zip_directory) == true)
                        Directory.Delete(_zip_directory, true);

                    Directory.CreateDirectory(_zip_directory);
                }
            }

            // write sheet images
            var _page_no = 1;
            {
                foreach (var _sheet in PrintLottoSheet(lott645_selector, choice.sequence_no, choice.login_id))
                {
                    var _file_name = $"{ServiceName}_{choice.login_id}_{choice.sequence_no:0000}_{_page_no:000}.jpg";
                    {
                        var _img_filepath = Path.Combine(_zip_directory, _file_name);
                        if (File.Exists(_img_filepath))
                            File.Delete(_img_filepath);

                        _sheet.Save(_img_filepath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }

                    _page_no++;
                }
            }

            // write text file
            {
                var _file_name = $"{ServiceName}_{choice.login_id}_{choice.sequence_no:0000}.csv";

                var _txt_filepath = Path.Combine(_zip_directory, _file_name);
                if (File.Exists(_txt_filepath))
                    File.Delete(_txt_filepath);

                using (var _writer = File.OpenWrite(_txt_filepath))
                {
                    using (var _stream = new StreamWriter(_writer))
                    {
                        foreach (var _s in lott645_selector)
                        {
                            var _line = $"{_s.SequenceNo},,"
                                      + $"{_s.Digit1},{_s.Digit2},{_s.Digit3},{_s.Digit4},{_s.Digit5},{_s.Digit6}";

                            _stream.WriteLine(_line);
                        }
                    }
                }
            }

            // create zip file
            var _zip_file = Path.ChangeExtension(_zip_directory, "zip");
            {
                if (File.Exists(_zip_file))
                    File.Delete(_zip_file);

                ZipFile.CreateFromDirectory(_zip_directory, _zip_file);
            }

            return _zip_file;
        }
    }
}