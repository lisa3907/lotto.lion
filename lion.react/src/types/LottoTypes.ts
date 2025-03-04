export interface LottoPrizeResult {
  sequenceNo: number;
  issueDate: string;
  lastReadTime?: string;
  nextReadTime?: string;
  readInterval?: number;
  predictAmount?: number;
  salesAmount?: number;
  autoSelect?: number;
  paymentDate?: string;
  digit1?: number;
  digit2?: number;
  digit3?: number;
  digit4?: number;
  digit5?: number;
  digit6?: number;
  digit7?: number;
  amount1?: number;
  amount2?: number;
  amount3?: number;
  amount4?: number;
  amount5?: number;
  count1?: number;
  count2?: number;
  count3?: number;
  count4?: number;
  count5?: number;
  remark?: string;
}

export interface LottoPrizeResponse {
  result?: LottoPrizeResult;
  error?: string;
}

export interface UserChoice {
  id: string;
  userId: string;
  sequenceNo: number;
  numbers: number[];
  isAuto: boolean;
  createdAt: string;
}

export interface UserChoicesResponse {
  result?: UserChoice[];
  error?: string;
}

export interface SaveChoiceResponse {
  result?: {
    id: string;
  };
  error?: string;
}

export interface DeleteChoiceResponse {
  result?: boolean;
  error?: string;
} 