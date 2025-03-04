// 사용자 정보 타입
export interface User {
  id: string;
  name: string;
  email: string;
  token: string;
  loginId?: string;
  loginName?: string;
  emailAddress?: string;
  phoneNumber?: string;
  isMailSend?: boolean;
  maxSelectNumber?: number;
  digit1?: number;
  digit2?: number;
  digit3?: number;
}

// 인증 상태 타입
export interface AuthState {
  user: User | null;
  guestToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string;
}

// API 응답 기본 타입
export interface ApiResponse<T = any> {
  success: boolean;
  message: string;
  result: T;
}

// 로또 당첨 결과 타입
export interface LottoResult {
  success: boolean;
  message: string;
  result: {
    // 공통 필드
    sequenceNo: number;
    issueDate: string;
    
    // 이번 주 로또 정보 필드
    lastReadTime?: string;
    nextReadTime?: string;
    readInterval?: number;
    predictAmount?: number;
    salesAmount?: number;
    
    // 당첨 결과 필드
    paymentDate?: string;
    digit1?: number;
    digit2?: number;
    digit3?: number;
    digit4?: number;
    digit5?: number;
    digit6?: number;
    digit7?: number; // 보너스 번호
    autoSelect?: number;
    count1?: number;
    amount1?: number;
    count2?: number;
    amount2?: number;
    count3?: number;
    amount3?: number;
    count4?: number;
    amount4?: number;
    count5?: number;
    amount5?: number;
    remark?: string;
  };
}

// 로또 번호 타입
export interface LottoNumbers {
  digit1: number;
  digit2: number;
  digit3: number;
  digit4: number;
  digit5: number;
  digit6: number;
}

// 사용자 선택 번호 타입
export interface UserChoice extends LottoNumbers {
  sequenceNo: number;
  loginId: string;
  choiceNo: number;
  jackpot1: boolean;
  jackpot2: boolean;
  jackpot3: boolean;
  jackpot4: boolean;
  jackpot5: boolean;
  jackpot6: boolean;
  isMailSent: boolean;
  ranking: number;
  amount: number;
  remark: string;
}

// 당첨 결과 타입
export interface WinningResult {
  matchCount: number;
  bonusMatch: boolean;
  rank: number;
}

// 알림 메시지 타입
export interface AlertMessage {
  id: number;
  message: string;
  createdAt: string;
  isRead: boolean;
} 