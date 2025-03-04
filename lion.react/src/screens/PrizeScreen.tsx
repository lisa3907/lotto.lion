import React, { useState, useEffect, useCallback } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  CircularProgress,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  SelectChangeEvent,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Alert,
  Button,
  Fab
} from '@mui/material';
import Header from '../components/Header';
import LottoBall from '../components/LottoBall';
import { useLotto } from '../contexts/LottoContext';
import { formatPrizeAmount } from '../utils/PrizeUtils';
import { useNavigate } from 'react-router-dom';
import ListAltIcon from '@mui/icons-material/ListAlt';
import { LottoPrizeResult } from '../types/LottoTypes';

// 결과 타입을 확장하여 API 응답의 다양한 필드 형식을 지원
interface ExtendedPrizeResult extends LottoPrizeResult {
  sequence_no?: number;
  date?: string;
  bonus_number?: number;
  numbers?: number[];
  first_prize?: string;
  second_prize?: string;
  third_prize?: string;
  fourth_prize?: string;
  fifth_prize?: string;
  first_winners?: number;
  second_winners?: number;
  third_winners?: number;
  fourth_winners?: number;
  fifth_winners?: number;
  sales_amount?: string;
  predict_amount?: string;
  read_interval?: number;
}

const PrizeScreen: React.FC = () => {
  const { currentSequenceNo, currentPrize, isLoading, error: lottoError, fetchCurrentPrize, fetchPrizeBySequenceNo } = useLotto();
  const [selectedSequenceNo, setSelectedSequenceNo] = useState<string>('');
  const [sequenceOptions, setSequenceOptions] = useState<number[]>([]);
  const navigate = useNavigate();

  // 초기 데이터 로드
  useEffect(() => {
    // 앱 시작 시 현재 회차 정보 로드
    fetchCurrentPrize();
  }, [fetchCurrentPrize]);

  // 현재 회차 정보가 로드되면 선택된 회차 설정
  useEffect(() => {
    if (currentSequenceNo && !selectedSequenceNo) {
      // 현재 회차 번호로 선택된 회차 설정 및 옵션 생성
      setSelectedSequenceNo(currentSequenceNo.toString());
      generateSequenceOptions(currentSequenceNo);
    }
  }, [currentSequenceNo, selectedSequenceNo]);

  // 선택한 회차 정보 가져오기
  useEffect(() => {
    // 선택된 회차가 있고, 로딩 중이 아니고, 현재 표시된 회차와 다를 때만 데이터 가져오기
    if (selectedSequenceNo && !isLoading && currentPrize) {
      const seqNo = parseInt(selectedSequenceNo);
      const currentSeqNo = currentPrize.result?.sequenceNo || (currentPrize.result as ExtendedPrizeResult)?.sequence_no;
      
      // 유효한 회차 번호인지 확인
      if (isNaN(seqNo) || seqNo <= 0) {
        return;
      }
      
      // 현재 표시된 회차와 다를 때만 데이터 가져오기
      if (seqNo !== currentSeqNo) {
        // 선택한 회차의 로또 정보 가져오기
        fetchPrizeBySequenceNo(seqNo).then(resultSeqNo => {
          // 오류 발생으로 다른 회차 번호가 반환된 경우 (롤백)
          if (resultSeqNo !== null && resultSeqNo !== currentSeqNo) {
            setSelectedSequenceNo(resultSeqNo.toString());
          }
        });
      }
    }
  }, [selectedSequenceNo, isLoading, currentPrize, fetchPrizeBySequenceNo]);

  // 오류 발생 시 최신 회차로 리셋
  useEffect(() => {
    if (lottoError && currentSequenceNo) {
      // 오류가 발생하면 선택된 회차를 최신 회차로 리셋하지만, 자동으로 데이터를 다시 가져오지는 않음
      // 사용자가 직접 "최신 회차로 이동" 버튼을 클릭하도록 유도
      setSelectedSequenceNo(currentSequenceNo.toString());
    }
  }, [lottoError, currentSequenceNo]);

  // 최신 회차로 이동하는 함수
  const goToLatestSequence = useCallback(() => {
    if (currentSequenceNo) {
      setSelectedSequenceNo(currentSequenceNo.toString());
      fetchPrizeBySequenceNo(currentSequenceNo);
    }
  }, [currentSequenceNo, fetchPrizeBySequenceNo]);

  // 이전 회차 선택 옵션 생성 (최근 10개)
  const generateSequenceOptions = (seqNo: number) => {
    if (!seqNo || seqNo <= 0) return;
    
    const options = [];
    // 최소 회차는 1회부터
    const startSeq = Math.max(1, seqNo - 9);
    
    for (let i = seqNo; i >= startSeq; i--) {
      options.push(i);
    }
    
    setSequenceOptions(options);
  };

  // 회차 선택 변경 처리
  const handleSequenceChange = (event: SelectChangeEvent<string>) => {
    const newSequenceNo = event.target.value;
    const seqNo = parseInt(newSequenceNo);
    
    // 유효성 검사
    if (isNaN(seqNo) || seqNo <= 0) {
      return;
    }
    
    // 현재 최신 회차보다 큰 회차는 선택 불가
    if (currentSequenceNo && seqNo > currentSequenceNo) {
      return;
    }
    
    // 항상 선택된 회차 번호 업데이트
    setSelectedSequenceNo(newSequenceNo);
  };

  // 당첨 번호 배열 생성
  const getWinningNumbers = () => {
    if (!currentPrize || !currentPrize.result) return [];
    
    // digit1~6 필드가 있는 경우 (GetPrizeBySeqNo API)
    if (currentPrize.result.digit1 !== undefined) {
      const digits = [
        currentPrize.result.digit1,
        currentPrize.result.digit2,
        currentPrize.result.digit3,
        currentPrize.result.digit4,
        currentPrize.result.digit5,
        currentPrize.result.digit6
      ].filter(digit => digit !== undefined) as number[];
      
      return digits;
    }
    
    // numbers 배열이 있는 경우 (이전 API 호환)
    if ((currentPrize.result as ExtendedPrizeResult).numbers) {
      return (currentPrize.result as ExtendedPrizeResult).numbers || [];
    }
    
    return [];
  };

  // 보너스 번호 가져오기
  const getBonusNumber = () => {
    if (!currentPrize || !currentPrize.result) return null;
    
    // digit7 필드가 있는 경우 (GetPrizeBySeqNo API)
    if (currentPrize.result.digit7 !== undefined) {
      return currentPrize.result.digit7;
    }
    
    // bonus_number 필드가 있는 경우 (이전 API 호환)
    if ((currentPrize.result as ExtendedPrizeResult).bonus_number !== undefined) {
      return (currentPrize.result as ExtendedPrizeResult).bonus_number;
    }
    
    return null;
  };

  return (
    <Container maxWidth="md">
      <Header title="로또 당첨 결과" />
      
      {lottoError && (
        <Alert severity="error" sx={{ mt: 2 }}>
          {lottoError}
          {lottoError.includes('범위') || lottoError.includes('유효하지 않은 회차') || lottoError.includes('아직 진행되지 않은 회차') ? (
            <Box sx={{ mt: 1 }}>
              <Typography variant="body2">
                현재 선택 가능한 회차 범위: 1회 ~ {currentSequenceNo}회
              </Typography>
              <Button 
                variant="outlined" 
                size="small" 
                sx={{ mt: 1 }}
                onClick={goToLatestSequence}
              >
                최신 회차로 이동
              </Button>
            </Box>
          ) : null}
        </Alert>
      )}
      
      <Paper elevation={3} sx={{ p: 3, mt: 3 }}>
        {isLoading ? (
          <Box display="flex" justifyContent="center" p={3}>
            <CircularProgress />
          </Box>
        ) : currentPrize && currentPrize.result ? (
          <>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
              <Typography variant="h5" component="h2">
                {currentPrize.result.sequenceNo || (currentPrize.result as ExtendedPrizeResult).sequence_no}회 당첨결과
              </Typography>
              
              <FormControl sx={{ minWidth: 120 }}>
                <InputLabel id="sequence-select-label">회차 선택</InputLabel>
                <Select
                  labelId="sequence-select-label"
                  value={selectedSequenceNo}
                  label="회차 선택"
                  onChange={handleSequenceChange}
                >
                  {sequenceOptions.map((option) => (
                    <MenuItem key={option} value={option.toString()}>
                      {option}회
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Box>
            
            <Typography variant="subtitle1" gutterBottom>
              추첨일: {currentPrize.result.issueDate || (currentPrize.result as ExtendedPrizeResult).date}
            </Typography>
            
            <Box display="flex" justifyContent="center" my={3}>
              {getWinningNumbers().map((number: number, index: number) => (
                <LottoBall key={index} number={number} />
              ))}
              {getBonusNumber() !== null && (
                <>
                  <Typography variant="h6" sx={{ mx: 1, alignSelf: 'center' }}>+</Typography>
                  <LottoBall number={getBonusNumber() as number} isBonus />
                </>
              )}
            </Box>
            
            <TableContainer component={Paper} elevation={1} sx={{ mt: 3 }}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>순위</TableCell>
                    <TableCell>당첨 내용</TableCell>
                    <TableCell align="right">당첨금</TableCell>
                    <TableCell align="right">당첨자 수</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  <TableRow>
                    <TableCell>1등</TableCell>
                    <TableCell>6개 번호 일치</TableCell>
                    <TableCell align="right">
                      {formatPrizeAmount(currentPrize.result.amount1 || (currentPrize.result as ExtendedPrizeResult).first_prize || '0')}
                    </TableCell>
                    <TableCell align="right">
                      {currentPrize.result.count1 || (currentPrize.result as ExtendedPrizeResult).first_winners || 0}명
                    </TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell>2등</TableCell>
                    <TableCell>5개 번호 + 보너스 번호 일치</TableCell>
                    <TableCell align="right">
                      {formatPrizeAmount(currentPrize.result.amount2 || (currentPrize.result as ExtendedPrizeResult).second_prize || '0')}
                    </TableCell>
                    <TableCell align="right">
                      {currentPrize.result.count2 || (currentPrize.result as ExtendedPrizeResult).second_winners || 0}명
                    </TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell>3등</TableCell>
                    <TableCell>5개 번호 일치</TableCell>
                    <TableCell align="right">
                      {formatPrizeAmount(currentPrize.result.amount3 || (currentPrize.result as ExtendedPrizeResult).third_prize || '0')}
                    </TableCell>
                    <TableCell align="right">
                      {currentPrize.result.count3 || (currentPrize.result as ExtendedPrizeResult).third_winners || 0}명
                    </TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell>4등</TableCell>
                    <TableCell>4개 번호 일치</TableCell>
                    <TableCell align="right">
                      {formatPrizeAmount(currentPrize.result.amount4 || (currentPrize.result as ExtendedPrizeResult).fourth_prize || '0')}
                    </TableCell>
                    <TableCell align="right">
                      {currentPrize.result.count4 || (currentPrize.result as ExtendedPrizeResult).fourth_winners || 0}명
                    </TableCell>
                  </TableRow>
                  <TableRow>
                    <TableCell>5등</TableCell>
                    <TableCell>3개 번호 일치</TableCell>
                    <TableCell align="right">
                      {formatPrizeAmount(currentPrize.result.amount5 || (currentPrize.result as ExtendedPrizeResult).fifth_prize || '0')}
                    </TableCell>
                    <TableCell align="right">
                      {currentPrize.result.count5 || (currentPrize.result as ExtendedPrizeResult).fifth_winners || 0}명
                    </TableCell>
                  </TableRow>
                </TableBody>
              </Table>
            </TableContainer>
            
            <Box mt={3}>
              <Typography variant="subtitle2" gutterBottom>
                총 판매금액: {formatPrizeAmount(currentPrize.result.salesAmount || (currentPrize.result as ExtendedPrizeResult).sales_amount || '0')}
              </Typography>
            </Box>
          </>
        ) : (
          <Typography>로또 정보를 불러오는 중입니다...</Typography>
        )}
      </Paper>
      
      <Fab 
        color="primary" 
        sx={{ position: 'fixed', bottom: 16, right: 16 }}
        onClick={() => navigate('/numbers')}
      >
        <ListAltIcon />
      </Fab>
    </Container>
  );
};

export default PrizeScreen; 