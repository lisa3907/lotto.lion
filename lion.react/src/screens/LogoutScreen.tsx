import React, { useState } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Alert
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import Header from '../components/Header';
import Loading from '../components/Loading';
import { useAuth } from '../contexts/AuthContext';

const LogoutScreen: React.FC = () => {
  const navigate = useNavigate();
  const { logout } = useAuth();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);
  const [showLeaveDialog, setShowLeaveDialog] = useState(false);

  const handleLogout = async () => {
    try {
      setIsLoading(true);
      setError('');
      await logout();
      navigate('/login');
    } catch (error) {
      console.error('로그아웃 실패:', error);
      setError('로그아웃에 실패했습니다.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleLeaveMember = async () => {
    try {
      setIsLoading(true);
      setError('');
      await logout();
      // TODO: 회원 탈퇴 API 구현
      navigate('/login');
    } catch (error) {
      console.error('회원 탈퇴 실패:', error);
      setError('회원 탈퇴에 실패했습니다.');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <Loading message="처리중..." />;
  }

  return (
    <Box>
      <Header title="로그아웃" />
      <Container maxWidth="md" sx={{ mt: 3 }}>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Paper elevation={3} sx={{ p: 3 }}>
          <Typography variant="h6" gutterBottom>
            계정 관리
          </Typography>
          
          <Box sx={{ mt: 3 }}>
            <Button
              variant="contained"
              color="primary"
              fullWidth
              onClick={() => setShowConfirmDialog(true)}
              sx={{ mb: 2 }}
            >
              로그아웃
            </Button>
            
            <Button
              variant="outlined"
              color="error"
              fullWidth
              onClick={() => setShowLeaveDialog(true)}
            >
              회원 탈퇴
            </Button>
          </Box>
        </Paper>

        {/* 로그아웃 확인 다이얼로그 */}
        <Dialog
          open={showConfirmDialog}
          onClose={() => setShowConfirmDialog(false)}
        >
          <DialogTitle>로그아웃</DialogTitle>
          <DialogContent>
            <Typography>
              정말 로그아웃 하시겠습니까?
            </Typography>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setShowConfirmDialog(false)}>
              취소
            </Button>
            <Button onClick={handleLogout} color="primary" autoFocus>
              확인
            </Button>
          </DialogActions>
        </Dialog>

        {/* 회원 탈퇴 확인 다이얼로그 */}
        <Dialog
          open={showLeaveDialog}
          onClose={() => setShowLeaveDialog(false)}
        >
          <DialogTitle>회원 탈퇴</DialogTitle>
          <DialogContent>
            <Typography color="error">
              회원 탈퇴 시 모든 데이터가 삭제되며 복구할 수 없습니다.
              정말 탈퇴하시겠습니까?
            </Typography>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setShowLeaveDialog(false)}>
              취소
            </Button>
            <Button onClick={handleLeaveMember} color="error" autoFocus>
              탈퇴
            </Button>
          </DialogActions>
        </Dialog>
      </Container>
    </Box>
  );
};

export default LogoutScreen; 