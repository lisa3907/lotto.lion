import React, { useState } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  Alert
} from '@mui/material';
import Header from '../components/Header';
import Loading from '../components/Loading';
import { useAuth } from '../contexts/AuthContext';

const PasswordChangeScreen: React.FC = () => {
  const { updateProfile, isLoading: authLoading, error: authError } = useAuth();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [formData, setFormData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (formData.newPassword !== formData.confirmPassword) {
      setError('새 비밀번호가 일치하지 않습니다.');
      return;
    }

    try {
      setIsLoading(true);
      setError('');
      await updateProfile({ password: formData.newPassword });
      setSuccess('비밀번호가 성공적으로 변경되었습니다.');
      setFormData({
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      });
    } catch (error) {
      console.error('비밀번호 변경 실패:', error);
      setError('비밀번호 변경에 실패했습니다.');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading || authLoading) {
    return <Loading message="비밀번호 변경중..." />;
  }

  return (
    <Box>
      <Header title="비밀번호 변경" />
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
          <form onSubmit={handleSubmit}>
            <TextField
              fullWidth
              type="password"
              label="현재 비밀번호"
              name="currentPassword"
              value={formData.currentPassword}
              onChange={handleChange}
              sx={{ mb: 2 }}
            />
            <TextField
              fullWidth
              type="password"
              label="새 비밀번호"
              name="newPassword"
              value={formData.newPassword}
              onChange={handleChange}
              sx={{ mb: 2 }}
            />
            <TextField
              fullWidth
              type="password"
              label="새 비밀번호 확인"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              sx={{ mb: 3 }}
              error={formData.newPassword !== formData.confirmPassword && formData.confirmPassword !== ''}
              helperText={
                formData.newPassword !== formData.confirmPassword && formData.confirmPassword !== ''
                  ? '비밀번호가 일치하지 않습니다.'
                  : ''
              }
            />
            <Button
              type="submit"
              variant="contained"
              fullWidth
              disabled={
                !formData.currentPassword ||
                !formData.newPassword ||
                !formData.confirmPassword ||
                formData.newPassword !== formData.confirmPassword
              }
            >
              변경
            </Button>
          </form>
        </Paper>
      </Container>
    </Box>
  );
};

export default PasswordChangeScreen; 