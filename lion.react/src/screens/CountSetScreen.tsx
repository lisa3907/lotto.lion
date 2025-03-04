import React, { useState, useEffect, useCallback } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  Slider,
  Button,
  Alert
} from '@mui/material';
import Header from '../components/Header';
import Loading from '../components/Loading';
import ErrorView from '../components/ErrorView';
import { userService } from '../services/UserService';
import { useAuth } from '../contexts/AuthContext';

const CountSetScreen: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [maxCount, setMaxCount] = useState<number>(5);

  const fetchUserInfo = useCallback(async () => {
    try {
      const userToken = localStorage.getItem('userToken');
      if (!userToken) {
        throw new Error('사용자 토큰이 없습니다.');
      }

      const userData = await userService.getUserInfo(userToken);
      setMaxCount(parseInt((userData as any).maxSelectNumber) || 5);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : '사용자 정보를 가져오는데 실패했습니다.';
      console.error('사용자 정보 가져오기 실패:', error);
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    if (isAuthenticated) {
      fetchUserInfo();
    } else {
      setIsLoading(false);
    }
  }, [isAuthenticated, fetchUserInfo]);

  const handleSave = async () => {
    if (!isAuthenticated) {
      setError('로그인이 필요합니다.');
      return;
    }

    const userToken = localStorage.getItem('authToken');
    if (!userToken) {
      setError('인증 토큰이 없습니다. 다시 로그인해주세요.');
      return;
    }

    try {
      setIsLoading(true);
      setError('');
      await userService.updateUserInfo(userToken, {
        loginName: '',
        maxSelectNumber: maxCount.toString(),
        digit1: '',
        digit2: '',
        digit3: ''
      });
      setSuccess('최대 선택 개수가 저장되었습니다.');
    } catch (error) {
      setError('설정 저장 중 오류가 발생했습니다.');
      console.error('설정 저장 실패:', error);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <Loading message="설정 로딩중..." />;
  }

  if (error) {
    return <ErrorView message={error} />;
  }

  return (
    <Box>
      <Header title="번호 설정" />
      <Container maxWidth="md" sx={{ mt: 3 }}>
        {success && (
          <Alert severity="success" sx={{ mb: 2 }}>
            {success}
          </Alert>
        )}

        <Paper elevation={3} sx={{ p: 3 }}>
          <Typography variant="h6" gutterBottom>
            최대 선택 번호 개수
          </Typography>
          <Typography variant="body2" color="text.secondary" gutterBottom>
            한 번에 선택할 수 있는 최대 번호 개수를 설정합니다.
          </Typography>
          
          <Box sx={{ mt: 4, mb: 2 }}>
            <Slider
              value={maxCount}
              onChange={(_, value) => setMaxCount(value as number)}
              step={1}
              marks
              min={1}
              max={10}
              valueLabelDisplay="on"
            />
          </Box>

          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            현재 설정: {maxCount}개
          </Typography>

          <Button
            variant="contained"
            fullWidth
            onClick={handleSave}
          >
            저장
          </Button>
        </Paper>
      </Container>
    </Box>
  );
};

export default CountSetScreen; 