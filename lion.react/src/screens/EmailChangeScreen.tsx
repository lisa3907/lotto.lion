import React, { useState, useEffect } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  Alert,
  Stepper,
  Step,
  StepLabel
} from '@mui/material';
import Header from '../components/Header';
import Loading from '../components/Loading';
import { userService } from '../services/UserService';
import { useAuth } from '../contexts/AuthContext';

const steps = ['이메일 입력', '인증 코드 확인'];

const EmailChangeScreen: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [activeStep, setActiveStep] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [email, setEmail] = useState('');
  const [verificationCode, setVerificationCode] = useState('');

  useEffect(() => {
    if (!isAuthenticated) {
      setError('로그인이 필요한 서비스입니다.');
    }
  }, [isAuthenticated]);

  const handleSendVerification = async () => {
    try {
      const userToken = localStorage.getItem('userToken');
      if (!userToken) {
        throw new Error('사용자 토큰이 없습니다.');
      }

      setIsLoading(true);
      setError('');
      await userService.sendMailToCheckMailAddress(userToken, email);
      setActiveStep(1);
      setSuccess('인증 코드가 전송되었습니다.');
    } catch (error) {
      console.error('이메일 인증 코드 전송 실패:', error);
      setError('인증 코드 전송에 실패했습니다.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleVerifyCode = async () => {
    try {
      const userToken = localStorage.getItem('userToken');
      if (!userToken) {
        throw new Error('사용자 토큰이 없습니다.');
      }

      setIsLoading(true);
      setError('');
      await userService.checkMailAddress(userToken, email, verificationCode);
      await userService.updateEmailAddress(userToken, email, verificationCode);
      setSuccess('이메일이 성공적으로 변경되었습니다.');
    } catch (error) {
      console.error('이메일 인증 실패:', error);
      setError('이메일 인증에 실패했습니다.');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <Loading message="처리중..." />;
  }

  return (
    <Box>
      <Header title="이메일 변경" />
      <Container maxWidth="md" sx={{ mt: 3 }}>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {success && (
          <Alert severity="success" sx={{ mb: 2 }}>
            {success}
          </Alert>
        )}

        <Paper elevation={3} sx={{ p: 3 }}>
          <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
            {steps.map((label) => (
              <Step key={label}>
                <StepLabel>{label}</StepLabel>
              </Step>
            ))}
          </Stepper>

          {activeStep === 0 ? (
            <Box>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                변경할 이메일 주소를 입력해 주세요.
              </Typography>
              <TextField
                fullWidth
                label="이메일"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                sx={{ mb: 2 }}
              />
              <Button
                variant="contained"
                fullWidth
                onClick={handleSendVerification}
                disabled={!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)}
              >
                인증 코드 전송
              </Button>
            </Box>
          ) : (
            <Box>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                이메일로 전송된 인증 코드를 입력해 주세요.
              </Typography>
              <TextField
                fullWidth
                label="인증 코드"
                value={verificationCode}
                onChange={(e) => setVerificationCode(e.target.value)}
                sx={{ mb: 2 }}
              />
              <Button
                variant="contained"
                fullWidth
                onClick={handleVerifyCode}
                disabled={!verificationCode}
              >
                확인
              </Button>
              <Button
                variant="text"
                fullWidth
                onClick={() => {
                  setActiveStep(0);
                  setVerificationCode('');
                  setSuccess('');
                }}
                sx={{ mt: 1 }}
              >
                이메일 다시 입력
              </Button>
            </Box>
          )}
        </Paper>
      </Container>
    </Box>
  );
};

export default EmailChangeScreen; 