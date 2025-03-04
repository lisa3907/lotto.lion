import React, { createContext, useContext, useState, useEffect, useCallback, useMemo, ReactNode } from 'react';
import { useAuth } from './AuthContext';
import { lottoService } from '../services';
import { LottoResult, UserChoice } from '../types';

interface LottoContextType {
  // 상태
  currentSequenceNo: number | null;
  currentPrize: LottoResult | null;
  selectedNumbers: number[];
  maxSelectableNumbers: number;
  userChoices: UserChoice[];
  isLoading: boolean;
  error: string;
  
  // 함수
  selectNumber: (number: number) => void;
  clearSelectedNumbers: () => void;
  saveSelectedNumbers: () => Promise<void>;
  fetchCurrentPrize: () => Promise<void>;
  fetchPrizeBySequenceNo: (sequenceNo: number) => Promise<number | null>;
  fetchUserChoices: (seqNo?: number) => Promise<void>;
  deleteUserChoice: (choiceNo: string) => Promise<void>;
}

const LottoContext = createContext<LottoContextType | undefined>(undefined);

export const useLotto = () => {
  const context = useContext(LottoContext);
  if (!context) {
    throw new Error('useLotto must be used within a LottoProvider');
  }
  return context;
};

interface LottoProviderProps {
  children: ReactNode;
}

export const LottoProvider: React.FC<LottoProviderProps> = ({ children }) => {
  const { guestToken, isAuthenticated, user } = useAuth();
  const [currentSequenceNo, setCurrentSequenceNo] = useState<number | null>(null);
  const [currentPrize, setCurrentPrize] = useState<LottoResult | null>(null);
  const [selectedNumbers, setSelectedNumbers] = useState<number[]>([]);
  const [userChoices, setUserChoices] = useState<UserChoice[]>([]);
  const [maxSelectableNumbers] = useState(6);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  // 현재 회차 로또 정보 조회
  const fetchCurrentPrize = useCallback(async () => {
    if (!guestToken) return;
    
    setIsLoading(true);
    setError('');

    try {
        const prizeData = await lottoService.getThisWeekPrize(guestToken);
        
        setCurrentPrize(prizeData);
        setCurrentSequenceNo(prizeData.result.sequenceNo);
    } catch (error) {
        console.error('현재 회차 로또 정보 조회 실패:', error);
        setError('로또 정보를 불러오는데 실패했습니다.');
    } finally {
        setIsLoading(false); // ✅ 성공/실패 관계없이 실행
    }
  }, [guestToken]);

  // 특정 회차 로또 정보 조회
  const fetchPrizeBySequenceNo = useCallback(async (sequenceNo: number) => {
    if (!guestToken) return null;
    
    // 항상 오류 상태 초기화
    setError('');
    
    try {
      setIsLoading(true);
      
      // 회차 번호 유효성 검사
      if (!sequenceNo || sequenceNo <= 0 || isNaN(sequenceNo)) {
        setIsLoading(false);
        throw new Error('유효하지 않은 회차 번호입니다.');
      }
      
      // 현재 최신 회차보다 큰 회차는 요청하지 않음
      if (currentSequenceNo && sequenceNo > currentSequenceNo) {
        setIsLoading(false);
        throw new Error(`아직 진행되지 않은 회차입니다. 현재 최신 회차는 ${currentSequenceNo}회 입니다.`);
      }
      
      const prizeData = await lottoService.getPrizeBySequenceNo(guestToken, sequenceNo);
      
      setCurrentPrize(prizeData);
      setIsLoading(false);
      return sequenceNo;
    } catch (error) {
      // 특정 회차 로또 정보 조회 실패 - 회차별 정보 로드 문제 진단을 위해 로그 유지
      console.error('특정 회차 로또 정보 조회 실패:', error);
      setIsLoading(false);
      
      // 오류 메시지 설정
      if (error instanceof Error) {
        // 범위 관련 오류 메시지 개선
        if (error.message.includes('범위')) {
          setError(`유효하지 않은 회차입니다. 현재 최신 회차는 ${currentSequenceNo}회 입니다.`);
        } else {
          setError(error.message);
        }
      } else {
        setError('로또 정보를 불러오는데 실패했습니다.');
      }
      
      // 오류 발생 시 현재 회차 번호 반환
      return currentSequenceNo;
    }
  }, [guestToken, currentSequenceNo]);

  // 사용자 선택 번호 조회
  const fetchUserChoices = useCallback(async (seqNo?: number) => {
    if (!isAuthenticated || !user?.token) return;
    
    try {
      setIsLoading(true);
      setError('');
      
      const choices = await lottoService.getUserChoices(user.token, seqNo);
      
      setUserChoices(choices);
      setIsLoading(false);
    } catch (error) {
      // 사용자 선택 번호 조회 실패 - 저장된 번호 로드 문제 진단을 위해 로그 유지
      console.error('사용자 선택 번호 조회 실패:', error);
      setIsLoading(false);
      setError('선택 번호를 불러오는데 실패했습니다.');
    }
  }, [isAuthenticated, user]);

  // 번호 선택
  const selectNumber = useCallback((number: number) => {
    setSelectedNumbers(prev => {
      // 이미 선택된 번호인 경우 제거
      if (prev.includes(number)) {
        return prev.filter(n => n !== number);
      }
      
      // 최대 선택 가능 개수를 초과하는 경우
      if (prev.length >= maxSelectableNumbers) {
        return prev;
      }
      
      // 번호 추가 (오름차순 정렬)
      return [...prev, number].sort((a, b) => a - b);
    });
  }, [maxSelectableNumbers]);

  // 선택 번호 초기화
  const clearSelectedNumbers = useCallback(() => {
    setSelectedNumbers([]);
  }, []);

  // 선택 번호 저장
  const saveSelectedNumbers = useCallback(async () => {
    if (!isAuthenticated || !user?.token) {
      setError('로그인이 필요합니다.');
      return;
    }
    
    if (selectedNumbers.length !== maxSelectableNumbers) {
      setError(`${maxSelectableNumbers}개의 번호를 선택해야 합니다.`);
      return;
    }
    
    try {
      setIsLoading(true);
      setError('');
      
      await lottoService.saveUserChoice(user.token, selectedNumbers);
      
      // 저장 후 선택 번호 초기화 및 목록 새로고침
      setSelectedNumbers([]);
      await fetchUserChoices(currentSequenceNo || undefined);
      
      setIsLoading(false);
    } catch (error) {
      // 선택 번호 저장 실패 - 번호 저장 문제 진단을 위해 로그 유지
      console.error('선택 번호 저장 실패:', error);
      setIsLoading(false);
      setError('번호 저장에 실패했습니다.');
    }
  }, [isAuthenticated, user, selectedNumbers, maxSelectableNumbers, fetchUserChoices, currentSequenceNo]);

  // 선택 번호 삭제
  const deleteUserChoice = useCallback(async (choiceNo: string) => {
    if (!isAuthenticated || !user?.token) {
      setError('로그인이 필요합니다.');
      return;
    }
    
    try {
      setIsLoading(true);
      setError('');
      
      await lottoService.deleteUserChoice(user.token, choiceNo);
      
      // 삭제 후 목록 새로고침
      await fetchUserChoices(currentSequenceNo || undefined);
      
      setIsLoading(false);
    } catch (error) {
      // 선택 번호 삭제 실패 - 번호 삭제 문제 진단을 위해 로그 유지
      console.error('선택 번호 삭제 실패:', error);
      setIsLoading(false);
      setError('번호 삭제에 실패했습니다.');
    }
  }, [isAuthenticated, user, fetchUserChoices, currentSequenceNo]);

  // 초기 데이터 로드
  useEffect(() => {
    if (guestToken) {
      fetchCurrentPrize();
    }
  }, [guestToken, fetchCurrentPrize]);

  // 인증 상태 변경 시 사용자 선택 번호 로드
  useEffect(() => {
    if (isAuthenticated && user?.token && currentSequenceNo) {
      fetchUserChoices(currentSequenceNo);
    }
  }, [isAuthenticated, user, currentSequenceNo, fetchUserChoices]);

  // 컨텍스트 값 메모이제이션
  const contextValue = useMemo(() => ({
    currentSequenceNo,
    currentPrize,
    selectedNumbers,
    maxSelectableNumbers,
    userChoices,
    isLoading,
    error,
    selectNumber,
    clearSelectedNumbers,
    saveSelectedNumbers,
    fetchCurrentPrize,
    fetchPrizeBySequenceNo,
    fetchUserChoices,
    deleteUserChoice
  }), [
    currentSequenceNo,
    currentPrize,
    selectedNumbers,
    maxSelectableNumbers,
    userChoices,
    isLoading,
    error,
    selectNumber,
    clearSelectedNumbers,
    saveSelectedNumbers,
    fetchCurrentPrize,
    fetchPrizeBySequenceNo,
    fetchUserChoices,
    deleteUserChoice
  ]);

  return (
    <LottoContext.Provider value={contextValue}>
      {children}
    </LottoContext.Provider>
  );
};

export default LottoContext; 