﻿namespace OdinSdk.BaseLib.Configuration
{
    /// <summary>
    ///
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// 프레임워크가 실행하는 서비스
        /// </summary>
        sdk,

        /// <summary>
        /// 여러가지의 Product,service등을 조합하여 한개의 솔루션을 만듭니다.
        /// 사용자에 맞게 customizing을 수행 할 수도 있습니다.
        /// </summary>
        solution,

        /// <summary>
        /// 단품으로 판매 및 설치가 가능한 단위 모듈
        /// 단타 판매 하는 것으로 사후관리를 하지 않습니다.
        /// </summary>
        product,

        /// <summary>
        /// 개발사가 직접 운영하며, 사용자는 유료 가입 하여
        /// 용역을 제공 받습니다.
        /// </summary>
        service,

        /// <summary>
        /// 일시적으로 개발되어지며 다른 사용자에게는 설치가 불가능한 모듈
        /// pilot project와 같이 일시적인 것
        /// </summary>
        project,

        /// <summary>
        /// 개발자가 테스트용으로 잠시 만드는 형태
        /// </summary>
        testing
    }
}