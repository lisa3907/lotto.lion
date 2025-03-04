import React, { useState, useEffect } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  Grid,
  Button,
  Alert,
  CircularProgress,
  Chip,
  Divider,
  Fab,
  IconButton
} from '@mui/material';
import Header from '../components/Header';
import LottoBall from '../components/LottoBall';
import { useLotto } from '../contexts/LottoContext';
import { useNavigate } from 'react-router-dom';
import HomeIcon from '@mui/icons-material/Home';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';

const MyNumbersScreen: React.FC = () => {
  const { 
    userChoices, 
    currentSequenceNo,
    isLoading, 
    error: lottoError,
    fetchUserChoices,
    deleteUserChoice,
    fetchPrizeBySequenceNo
  } = useLotto();
  const [error, setError] = useState('');
  const [selectedSequenceNo, setSelectedSequenceNo] = useState<number | null>(null);
  const navigate = useNavigate();

  // 초기 데이터 로드
  useEffect(() => {
    if (currentSequenceNo && !selectedSequenceNo) {
      setSelectedSequenceNo(currentSequenceNo);
    }
  }, [currentSequenceNo, selectedSequenceNo]);

  // 선택한 회차의 데이터 로드 및 사용자 선택 번호 로드를 하나의 useEffect로 통합
  useEffect(() => {
    let isMounted = true;
    
    const loadData = async () => {
      if (selectedSequenceNo && !isLoading) {
        console.log('선택된 회차 데이터 로드 시작:', selectedSequenceNo);
        try {
          await fetchPrizeBySequenceNo(selectedSequenceNo);
          
          // 컴포넌트가 여전히 마운트 상태인지 확인
          if (isMounted) {
            console.log('사용자 선택 번호 로드 시작:', selectedSequenceNo);
            await fetchUserChoices(selectedSequenceNo);
          }
        } catch (error) {
          console.error('데이터 로드 중 오류 발생:', error);
        }
      }
    };
    
    loadData();
    
    // 클린업 함수
    return () => {
      isMounted = false;
    };
  }, [selectedSequenceNo]);

  const handleDeleteNumbers = async (choiceNo: number) => {
    try {
      setError('');
      await deleteUserChoice(choiceNo.toString());
      // 삭제 후 데이터 다시 로드
      if (selectedSequenceNo) {
        console.log('번호 삭제 후 데이터 다시 로드:', selectedSequenceNo);
        // 약간의 지연 후 데이터 다시 로드 (상태 업데이트 시간 고려)
        setTimeout(() => {
          fetchUserChoices(selectedSequenceNo);
        }, 300);
      }
    } catch (error) {
      setError('번호를 삭제하는데 실패했습니다.');
    }
  };

  // 이전 회차로 이동
  const handlePreviousSequence = () => {
    if (selectedSequenceNo && selectedSequenceNo > 1) {
      setSelectedSequenceNo(selectedSequenceNo - 1);
    }
  };

  // 다음 회차로 이동
  const handleNextSequence = () => {
    if (selectedSequenceNo && currentSequenceNo && selectedSequenceNo < currentSequenceNo) {
      setSelectedSequenceNo(selectedSequenceNo + 1);
    }
  };

  // 당첨 등수에 따른 텍스트 반환
  const getRankingText = (ranking: number): string => {
    switch(ranking) {
      case 1: return '1등';
      case 2: return '2등';
      case 3: return '3등';
      case 4: return '4등';
      case 5: return '5등';
      case 6: return '낙첨';
      default: return '미확인';
    }
  };

  // 당첨 등수에 따른 색상 반환
  const getRankingColor = (ranking: number): string => {
    switch(ranking) {
      case 1: return '#FF0000';
      case 2: return '#FF6600';
      case 3: return '#FFCC00';
      case 4: return '#00CC00';
      case 5: return '#0099FF';
      case 6: return '#999999';
      default: return '#999999';
    }
  };

  if (isLoading && !selectedSequenceNo) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '80vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Header title="내 번호 확인" />
      <Container maxWidth="md" sx={{ mt: 3, pb: 8, position: 'relative' }}>
        {(error || lottoError) && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error || lottoError}
          </Alert>
        )}

        {/* 회차 선택 네비게이션 */}
        <Paper elevation={3} sx={{ p: 2, mb: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <IconButton 
            onClick={handlePreviousSequence} 
            disabled={!selectedSequenceNo || selectedSequenceNo <= 1 || isLoading}
            aria-label="이전 회차"
          >
            <ArrowBackIcon />
          </IconButton>
          
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
              {selectedSequenceNo ? `${selectedSequenceNo}회차` : '회차 정보 로딩 중...'}
            </Typography>
            {currentSequenceNo && selectedSequenceNo && (
              <Typography variant="caption" color="text.secondary">
                {selectedSequenceNo === currentSequenceNo 
                  ? '(현재 회차)' 
                  : `(최신: ${currentSequenceNo}회차)`}
              </Typography>
            )}
          </Box>
          
          <IconButton 
            onClick={handleNextSequence} 
            disabled={!selectedSequenceNo || !currentSequenceNo || selectedSequenceNo >= currentSequenceNo || isLoading}
            aria-label="다음 회차"
          >
            <ArrowForwardIcon />
          </IconButton>
        </Paper>

        {isLoading && (
          <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
            <CircularProgress />
          </Box>
        )}

        {!isLoading && (
          <Paper elevation={3} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>
              저장된 번호
            </Typography>

            {userChoices.length > 0 ? (
              userChoices.map((choice, index) => (
                <Box
                  key={`${choice.sequenceNo}-${choice.choiceNo}`}
                  sx={{
                    mb: 3,
                    pb: 2,
                    borderBottom: index < userChoices.length - 1 ? '1px solid #eee' : 'none'
                  }}
                >
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
                    <Typography variant="subtitle1">
                      {choice.sequenceNo}회차 #{choice.choiceNo}
                    </Typography>
                    <Chip 
                      label={getRankingText(choice.ranking)} 
                      sx={{ 
                        bgcolor: getRankingColor(choice.ranking),
                        color: 'white',
                        fontWeight: 'bold'
                      }} 
                    />
                  </Box>
                  
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
                    <Box sx={{ display: 'flex', gap: 1, flexGrow: 1 }}>
                      <LottoBall 
                        number={choice.digit1} 
                        isHighlighted={choice.jackpot1}
                      />
                      <LottoBall 
                        number={choice.digit2} 
                        isHighlighted={choice.jackpot2}
                      />
                      <LottoBall 
                        number={choice.digit3} 
                        isHighlighted={choice.jackpot3}
                      />
                      <LottoBall 
                        number={choice.digit4} 
                        isHighlighted={choice.jackpot4}
                      />
                      <LottoBall 
                        number={choice.digit5} 
                        isHighlighted={choice.jackpot5}
                      />
                      <LottoBall 
                        number={choice.digit6} 
                        isHighlighted={choice.jackpot6}
                      />
                    </Box>
                    <Button
                      variant="outlined"
                      color="error"
                      size="small"
                      onClick={() => handleDeleteNumbers(choice.choiceNo)}
                    >
                      삭제
                    </Button>
                  </Box>
                  
                  {choice.amount > 0 && (
                    <Typography variant="body2" sx={{ color: 'success.main', fontWeight: 'bold' }}>
                      당첨금: {choice.amount.toLocaleString()}원
                    </Typography>
                  )}
                  
                  {choice.remark && (
                    <Typography variant="body2" color="text.secondary">
                      {choice.remark}
                    </Typography>
                  )}
                </Box>
              ))
            ) : (
              <Typography color="text.secondary">
                {selectedSequenceNo ? `${selectedSequenceNo}회차에 저장된 번호가 없습니다.` : '저장된 번호가 없습니다.'}
              </Typography>
            )}
          </Paper>
        )}

        {/* 홈으로 돌아가는 버튼 */}
        <Fab 
          color="primary" 
          aria-label="홈으로" 
          sx={{ 
            position: 'fixed', 
            bottom: 20, 
            right: 20 
          }}
          onClick={() => navigate('/prize')}
        >
          <HomeIcon />
        </Fab>
      </Container>
    </Box>
  );
};

export default MyNumbersScreen; 