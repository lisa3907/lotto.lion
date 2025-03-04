# QLottoLion React 앱

이 프로젝트는 Objective-C로 개발된 iOS 앱 QLottoLion을 React 웹 앱으로 변환한 것입니다.

## 기능

- 사용자 인증 (로그인/회원가입)
- 로또 번호 선택 및 저장
- 당첨 결과 확인
- 알림 메시지 확인
- 사용자 설정 관리

## 기술 스택

- React 19
- TypeScript 4.9
- Material UI 6
- React Router 7
- Axios

## 시작하기

### 필수 조건

- Node.js 14.0.0 이상
- npm 6.0.0 이상

### 설치

1. 저장소 클론

```bash
git clone https://github.com/yourusername/qlottolion-react.git
cd qlottolion-react
```

2. 의존성 설치

```bash
npm install
```

3. 개발 서버 실행

```bash
npm start
```

4. 브라우저에서 확인

```
http://localhost:3000
```

## 빌드

프로덕션 빌드를 생성하려면 다음 명령어를 실행하세요:

```bash
npm run build
```

빌드된 파일은 `build` 디렉토리에 생성됩니다.

## API 연결

API 서버와의 연결은 `src/services/ApiClient.ts` 파일에서 관리됩니다. 프록시 설정은 `package.json` 파일의 `proxy` 필드에서 설정할 수 있습니다.

## 프로젝트 구조

```
src/
  ├── components/       # 재사용 가능한 컴포넌트
  ├── contexts/         # React Context API
  ├── navigation/       # 라우팅 관련 컴포넌트
  ├── screens/          # 화면 컴포넌트
  ├── services/         # API 서비스
  │   ├── ApiClient.ts  # 공통 API 클라이언트
  │   ├── AuthService.ts # 인증 관련 API
  │   ├── LottoService.ts # 로또 관련 API
  │   └── UserService.ts # 사용자 관련 API
  ├── types/            # TypeScript 타입 정의
  ├── utils/            # 유틸리티 함수
  ├── App.tsx           # 메인 앱 컴포넌트
  └── index.tsx         # 앱 진입점
```

## 최근 개선 사항

### 1. 코드 구조 개선
- API 서비스를 도메인별로 분리 (AuthService, LottoService, UserService)
- 공통 API 클라이언트 구현으로 중복 코드 제거
- 불필요한 CRA 기본 파일 제거

### 2. 성능 최적화
- Context API 최적화 (useMemo, useCallback 활용)
- 불필요한 리렌더링 방지
- 에러 처리 개선

### 3. 타입 정의 개선
- 중복 필드 제거 및 일관된 명명 규칙 적용
- 타입 재사용성 향상 (LottoNumbers 인터페이스 등)

### 4. 코드 품질 향상
- 일관된 에러 처리 메커니즘 적용
- 불필요한 콘솔 로그 정리
- 코드 가독성 개선

## 라이센스

이 프로젝트는 MIT 라이센스를 따릅니다.
