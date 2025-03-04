import React, { useState, useEffect } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  List,
  ListItemButton,
  ListItemText,
  ListItemIcon,
  Divider,
  Alert,
  CircularProgress
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import Header from '../components/Header';
import { useAuth } from '../contexts/AuthContext';
import EmailIcon from '@mui/icons-material/Email';
import LockIcon from '@mui/icons-material/Lock';
import NumbersIcon from '@mui/icons-material/Numbers';
import LogoutIcon from '@mui/icons-material/Logout';

interface UserInfo {
  email: string;
  name: string;
  maxSelectNumber: number;
}

const SettingsScreen: React.FC = () => {
  const navigate = useNavigate();
  const { user, isLoading, error: authError } = useAuth();
  const [error, setError] = useState('');

  const userInfo = user ? {
    email: user.email,
    name: user.name,
    maxSelectNumber: 5
  } : null;

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '80vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Header title="설정" />
      <Container maxWidth="md" sx={{ mt: 3 }}>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {userInfo && (
          <Paper elevation={3} sx={{ mb: 3 }}>
            <Box sx={{ p: 3, textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>
                {userInfo.name}님
              </Typography>
              <Typography color="text.secondary">
                {userInfo.email}
              </Typography>
            </Box>
          </Paper>
        )}

        <Paper elevation={3}>
          <List>
            <ListItemButton onClick={() => navigate('/email-change')}>
              <ListItemIcon>
                <EmailIcon />
              </ListItemIcon>
              <ListItemText
                primary="이메일 변경"
                secondary="계정에 연결된 이메일 주소를 변경합니다."
              />
            </ListItemButton>
            
            <Divider />
            
            <ListItemButton onClick={() => navigate('/password-change')}>
              <ListItemIcon>
                <LockIcon />
              </ListItemIcon>
              <ListItemText
                primary="비밀번호 변경"
                secondary="계정 비밀번호를 변경합니다."
              />
            </ListItemButton>
            
            <Divider />
            
            <ListItemButton onClick={() => navigate('/count-set')}>
              <ListItemIcon>
                <NumbersIcon />
              </ListItemIcon>
              <ListItemText
                primary="번호 설정"
                secondary={`한 번에 선택할 수 있는 최대 번호 개수: ${userInfo?.maxSelectNumber || 5}개`}
              />
            </ListItemButton>
            
            <Divider />
            
            <ListItemButton onClick={() => navigate('/logout')} sx={{ color: 'error.main' }}>
              <ListItemIcon sx={{ color: 'inherit' }}>
                <LogoutIcon />
              </ListItemIcon>
              <ListItemText
                primary="로그아웃"
                secondary="계정에서 로그아웃합니다."
              />
            </ListItemButton>
          </List>
        </Paper>
      </Container>
    </Box>
  );
};

export default SettingsScreen; 