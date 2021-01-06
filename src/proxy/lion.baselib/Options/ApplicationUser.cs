namespace LottoLion.BaseLib.Options
{
    public class ApplicationUser
    {
        /// <summary>
        /// 로그인 아이디
        /// </summary>
        public string login_id
        {
            get; set;
        }

        /// <summary>
        /// 멤버 이름
        /// </summary>
        public string login_name
        {
            get; set;
        }

        /// <summary>
        /// 암호
        /// </summary>
        public string password
        {
            get; set;
        }

        /// <summary>
        /// 전자 메일 주소
        /// </summary>
        public string mail_address
        {
            get; set;
        }

        /// <summary>
        /// 모바일 장치 종류
        /// </summary>
        public string device_type
        {
            get; set;
        }

        /// <summary>
        /// 장치 식별자
        /// </summary>
        public string device_id
        {
            get; set;
        }

        /// <summary>
        /// 회원에게 제공 할 게임 수
        /// </summary>
        public short max_select_number
        {
            get; set;
        }

        /// <summary>
        /// 고정 번호 1
        /// </summary>
        public short digit1
        {
            get; set;
        }

        /// <summary>
        /// 고정 번호 2
        /// </summary>
        public short digit2
        {
            get; set;
        }

        /// <summary>
        /// 고정 번호 3
        /// </summary>
        public short digit3
        {
            get; set;
        }

        /// <summary>
        /// 이메일 검증 번호(6자리)
        /// </summary>
        public string check_number
        {
            get; set;
        }

        /// <summary>
        /// facebook id
        /// </summary>
        public string facebook_id
        {
            get; set;
        }

        /// <summary>
        /// facebook token
        /// </summary>
        public string facebook_token
        {
            get; set;
        }
    }
}