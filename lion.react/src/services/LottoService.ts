import { LottoResult, UserChoice } from '../types';
import { api } from './ApiClient';

// 이번 주 로또 정보 조회 API
export const getThisWeekPrize = async (token: string): Promise<LottoResult> => {
  try {
    const response = await api.post<LottoResult>(
      '/api/lotto/GetThisWeekPrize',
      null,
      token
    );
    
    if (!response.success) {
      throw new Error(response.message || '로또 정보 조회에 실패했습니다.');
    }
    
    return response;
  } catch (error) {
    // 이번 주 로또 정보 조회 실패 - 현재 회차 정보 로드 문제 진단을 위해 로그 유지
    console.error('이번 주 로또 정보 조회 실패:', error);
    throw error;
  }
};

// 특정 회차 로또 정보 조회 API
export const getPrizeBySequenceNo = async (token: string, sequenceNo: number): Promise<LottoResult> => {
  try {
    // 회차 번호 유효성 검사
    if (!sequenceNo || sequenceNo <= 0) {
      throw new Error('유효하지 않은 회차 번호입니다.');
    }
    
    // 현재 최신 회차 정보는 API 서버에서 검증하도록 수정
    // 클라이언트에서 추가 검증은 제거하고 API 응답에 의존
    
    const response = await api.post<LottoResult>(
      '/api/lotto/GetPrizeBySeqNo',
      { sequence_no: sequenceNo },
      token
    );
    
    if (!response.success) {
      // API에서 반환한 오류 메시지 사용
      throw new Error(response.message || '로또 정보 조회에 실패했습니다.');
    }
    
    return response;
  } catch (error) {
    // 특정 회차 로또 정보 조회 실패 - 회차별 정보 로드 문제 진단을 위해 로그 유지
    console.error('특정 회차 로또 정보 조회 실패:', error);
    throw error;
  }
};

// 사용자 선택 번호 저장 API
export const saveUserChoice = async (
  token: string,
  numbers: number[]
): Promise<void> => {
  try {
    if (numbers.length !== 6) {
      throw new Error('6개의 번호를 선택해야 합니다.');
    }
    
    const data = {
      digit1: numbers[0],
      digit2: numbers[1],
      digit3: numbers[2],
      digit4: numbers[3],
      digit5: numbers[4],
      digit6: numbers[5]
    };
    
    const response = await api.post<{ success: boolean; message: string }>(
      '/api/lotto/SaveUserChoice',
      data,
      token
    );
    
    if (!response.success) {
      throw new Error(response.message || '번호 저장에 실패했습니다.');
    }
  } catch (error) {
    // 사용자 선택 번호 저장 실패 - 번호 저장 문제 진단을 위해 로그 유지
    console.error('사용자 선택 번호 저장 실패:', error);
    throw error;
  }
};

// 사용자 선택 번호 조회 API
export const getUserChoices = async (
  token: string,
  sequenceNo?: number
): Promise<UserChoice[]> => {
  try {
    const data = sequenceNo ? { sequence_no: sequenceNo } : {};
    
    const response = await api.post<{ success: boolean; message: string; result: UserChoice[] }>(
      '/api/lotto/GetUserChoices',
      data,
      token
    );
    
    if (!response.success) {
      throw new Error(response.message || '선택 번호 조회에 실패했습니다.');
    }
    
    return response.result || [];
  } catch (error) {
    // 사용자 선택 번호 조회 실패 - 저장된 번호 로드 문제 진단을 위해 로그 유지
    console.error('사용자 선택 번호 조회 실패:', error);
    throw error;
  }
};

// 사용자 선택 번호 삭제 API
export const deleteUserChoice = async (
  token: string,
  choiceNo: string
): Promise<void> => {
  try {
    const response = await api.post<{ success: boolean; message: string }>(
      '/api/lotto/DeleteUserChoice',
      { choiceNo },
      token
    );
    
    if (!response.success) {
      throw new Error(response.message || '번호 삭제에 실패했습니다.');
    }
  } catch (error) {
    // 사용자 선택 번호 삭제 실패 - 번호 삭제 문제 진단을 위해 로그 유지
    console.error('사용자 선택 번호 삭제 실패:', error);
    throw error;
  }
};

// 로또 서비스 객체
export const lottoService = {
  getThisWeekPrize,
  getPrizeBySequenceNo,
  saveUserChoice,
  getUserChoices,
  deleteUserChoice
}; 