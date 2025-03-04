import React, { useState } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  Alert,
  Link,
  Stepper,
  Step,
  StepLabel
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const steps = ['이메일 입력', '인증 코드 확인', '회원 정보 입력'];

const SignUpScreen: React.FC = () => {
  const navigate = useNavigate();
  const { signUp, sendVerificationEmail, verifyEmail, isLoading, error: authError } = useAuth();
  const [activeStep, setActiveStep] = useState(0);
  const [error, setError] = useState('');
  const [formData, setFormData] = useState({
    email: '',
    verificationCode: '',
    name: '',
    password: '',
    confirmPassword: ''
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSendVerification = async () => {
    if (!formData.email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      setError('올바른 이메일 주소를 입력해주세요.');
      return;
    }

    try {
      setError('');
      await sendVerificationEmail(formData.email);
      setActiveStep(1);
    } catch (error) {
      setError('인증 코드 전송에 실패했습니다.');
    }
  };

  const handleVerifyCode = async () => {
    if (!formData.verificationCode) {
      setError('인증 코드를 입력해주세요.');
      return;
    }

    try {
      setError('');
      await verifyEmail(formData.email, formData.verificationCode);
      setActiveStep(2);
    } catch (error) {
      setError('인증 코드가 올바르지 않습니다.');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (formData.password !== formData.confirmPassword) {
      setError('비밀번호가 일치하지 않습니다.');
      return;
    }

    try {
      setError('');
      await signUp({
        email: formData.email,
        name: formData.name,
        password: formData.password,
        verificationCode: formData.verificationCode
      });
      navigate('/login');
    } catch (error) {
      setError('회원가입에 실패했습니다.');
    }
  };

  const renderStepContent = () => {
    switch (activeStep) {
      case 0:
        return (
          <>
            <TextField
              fullWidth
              label="이메일"
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              margin="normal"
              required
            />
            <Button
              variant="contained"
              fullWidth
              size="large"
              onClick={handleSendVerification}
              disabled={isLoading || !formData.email}
              sx={{ mt: 3 }}
            >
              {isLoading ? '전송중...' : '인증 코드 전송'}
            </Button>
          </>
        );
      case 1:
        return (
          <>
            <TextField
              fullWidth
              label="인증 코드"
              name="verificationCode"
              value={formData.verificationCode}
              onChange={handleChange}
              margin="normal"
              required
            />
            <Button
              variant="contained"
              fullWidth
              size="large"
              onClick={handleVerifyCode}
              disabled={isLoading || !formData.verificationCode}
              sx={{ mt: 3 }}
            >
              {isLoading ? '확인중...' : '확인'}
            </Button>
            <Button
              variant="text"
              fullWidth
              onClick={() => setActiveStep(0)}
              sx={{ mt: 1 }}
            >
              이메일 다시 입력
            </Button>
          </>
        );
      case 2:
        return (
          <form onSubmit={handleSubmit}>
            <TextField
              fullWidth
              label="이름"
              name="name"
              value={formData.name}
              onChange={handleChange}
              margin="normal"
              required
            />
            <TextField
              fullWidth
              label="비밀번호"
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              margin="normal"
              required
            />
            <TextField
              fullWidth
              label="비밀번호 확인"
              type="password"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              margin="normal"
              required
              error={formData.password !== formData.confirmPassword && formData.confirmPassword !== ''}
              helperText={
                formData.password !== formData.confirmPassword && formData.confirmPassword !== ''
                  ? '비밀번호가 일치하지 않습니다.'
                  : ''
              }
            />
            <Button
              type="submit"
              variant="contained"
              fullWidth
              size="large"
              disabled={isLoading}
              sx={{ mt: 3 }}
            >
              {isLoading ? '가입중...' : '가입하기'}
            </Button>
          </form>
        );
      default:
        return null;
    }
  };

  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          minHeight: '100vh',
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          py: 3
        }}
      >
        <Paper elevation={3} sx={{ p: 4 }}>
          <Typography variant="h5" component="h1" align="center" gutterBottom>
            회원가입
          </Typography>

          <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
            {steps.map((label) => (
              <Step key={label}>
                <StepLabel>{label}</StepLabel>
              </Step>
            ))}
          </Stepper>

          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}

          {renderStepContent()}

          <Box sx={{ mt: 2, textAlign: 'center' }}>
            <Typography variant="body2">
              이미 계정이 있으신가요?{' '}
              <Link
                component="button"
                variant="body2"
                onClick={() => navigate('/login')}
              >
                로그인
              </Link>
            </Typography>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
};

export default SignUpScreen; 