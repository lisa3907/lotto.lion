import React, { useState, useEffect } from 'react';
import { 
  Container, 
  Box, 
  Typography, 
  CircularProgress, 
  Paper, 
  Button, 
  Alert, 
  TextField, 
  Grid,
  Snackbar,
  Avatar,
  Divider,
  IconButton
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import HomeIcon from '@mui/icons-material/Home';
import Header from '../components/Header';
import { useAuth } from '../contexts/AuthContext';
import { userService } from '../services/UserService';
import { User } from '../types';

const ProfileScreen: React.FC = () => {
  const navigate = useNavigate();
  const { isAuthenticated, user: authUser } = useAuth();
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [isEditing, setIsEditing] = useState(false);
  
  // 편집 가능한 필드
  const [loginName, setLoginName] = useState('');
  const [maxSelectNumber, setMaxSelectNumber] = useState('');
  const [digit1, setDigit1] = useState('');
  const [digit2, setDigit2] = useState('');
  const [digit3, setDigit3] = useState('');

  useEffect(() => {
    if (isAuthenticated) {
      fetchUserInfo();
    }
  }, [isAuthenticated]);

  // 사용자 정보 가져오기
  const fetchUserInfo = async () => {
    try {
      setIsLoading(true);
      setError('');

      const userToken = localStorage.getItem('userToken');
      if (!userToken) {
        throw new Error('사용자 토큰이 없습니다.');
      }

      const userData = await userService.getUserInfo(userToken);
      console.log('가져온 사용자 정보:', userData);
      setUser(userData);
      
      // 편집 필드 초기화
      setLoginName(userData.loginName || '');
      setMaxSelectNumber(userData.maxSelectNumber?.toString() || '0');
      setDigit1(userData.digit1?.toString() || '0');
      setDigit2(userData.digit2?.toString() || '0');
      setDigit3(userData.digit3?.toString() || '0');
    } catch (error) {
      console.error('사용자 정보 로드 실패:', error);
      setError(error instanceof Error ? error.message : '사용자 정보를 불러오는데 실패했습니다.');
    } finally {
      setIsLoading(false);
    }
  };
  
  // 사용자 정보 업데이트
  const handleUpdateUserInfo = async () => {
    try {
      setIsSaving(true);
      setError('');
      
      const userToken = localStorage.getItem('userToken');
      if (!userToken) {
        throw new Error('사용자 토큰이 없습니다.');
      }
      
      // 입력값 검증
      if (!loginName.trim()) {
        throw new Error('이름을 입력해주세요.');
      }
      
      const maxSelectNum = parseInt(maxSelectNumber);
      if (isNaN(maxSelectNum) || maxSelectNum < 0) {
        throw new Error('최대 선택 가능 번호 수는 0 이상의 숫자여야 합니다.');
      }
      
      const num1 = parseInt(digit1);
      const num2 = parseInt(digit2);
      const num3 = parseInt(digit3);
      
      if (isNaN(num1) || num1 < 0 || num1 > 45 || 
          isNaN(num2) || num2 < 0 || num2 > 45 || 
          isNaN(num3) || num3 < 0 || num3 > 45) {
        throw new Error('행운의 번호는 0~45 사이의 숫자여야 합니다.');
      }
      
      // 사용자 정보 업데이트
      const updatedUser = await userService.updateUserInfo(userToken, {
        loginName,
        maxSelectNumber: maxSelectNum,
        digit1: num1,
        digit2: num2,
        digit3: num3
      });
      
      setUser(updatedUser);
      setSuccess('사용자 정보가 성공적으로 업데이트되었습니다.');
      setIsEditing(false);
    } catch (error) {
      console.error('사용자 정보 업데이트 실패:', error);
      setError(error instanceof Error ? error.message : '사용자 정보를 업데이트하는데 실패했습니다.');
    } finally {
      setIsSaving(false);
    }
  };
  
  // 편집 모드 토글
  const toggleEditMode = () => {
    if (isEditing) {
      // 편집 취소 시 원래 값으로 되돌림
      if (user) {
        setLoginName(user.loginName || '');
        setMaxSelectNumber(user.maxSelectNumber?.toString() || '0');
        setDigit1(user.digit1?.toString() || '0');
        setDigit2(user.digit2?.toString() || '0');
        setDigit3(user.digit3?.toString() || '0');
      }
    }
    setIsEditing(!isEditing);
  };

  if (!isAuthenticated) {
    return (
      <Box>
        <Header title="내 프로필" />
        <Container maxWidth="md" sx={{ mt: 3 }}>
          <Paper elevation={3} sx={{ p: 3 }}>
            <Typography variant="h6" gutterBottom>로그인이 필요합니다</Typography>
            <Typography variant="body1">
              사용자 정보를 확인하려면 로그인이 필요합니다.
            </Typography>
          </Paper>
        </Container>
      </Box>
    );
  }

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: '#f5f5f5' }}>
      <Header title="프로필" />
      
      {/* 홈 버튼 추가 */}
      <Box sx={{ position: 'absolute', top: 10, left: 10, zIndex: 1000 }}>
        <IconButton 
          color="inherit" 
          onClick={() => navigate('/prize')}
          aria-label="홈으로 이동"
        >
          <HomeIcon />
        </IconButton>
      </Box>
      
      <Container maxWidth="md" sx={{ py: 4 }}>
        {isLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
            <CircularProgress />
          </Box>
        ) : error ? (
          <Alert severity="error" sx={{ mb: 3 }}>
            {error}
            <Button 
              variant="outlined" 
              size="small" 
              sx={{ ml: 2 }} 
              onClick={fetchUserInfo}
            >
              다시 시도
            </Button>
          </Alert>
        ) : (
          <>
            <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Avatar sx={{ width: 64, height: 64, mr: 2, bgcolor: 'primary.main' }}>
                  {user?.loginName?.charAt(0) || user?.name?.charAt(0) || '?'}
                </Avatar>
                <Box>
                  <Typography variant="h6">
                    {isEditing ? (
                      <TextField
                        label="이름"
                        value={loginName}
                        onChange={(e) => setLoginName(e.target.value)}
                        variant="outlined"
                        size="small"
                        fullWidth
                        sx={{ mb: 1 }}
                      />
                    ) : (
                      user?.loginName || user?.name || '이름 없음'
                    )}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {user?.loginId || '아이디 없음'}
                  </Typography>
                </Box>
              </Box>
              
              <Divider sx={{ my: 2 }} />
              
              <Grid container spacing={2}>
                <Grid item xs={12} sm={6}>
                  <Typography variant="subtitle2" color="text.secondary">이메일</Typography>
                  <Typography variant="body1">{user?.emailAddress || user?.email || '이메일 없음'}</Typography>
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Typography variant="subtitle2" color="text.secondary">전화번호</Typography>
                  <Typography variant="body1">{user?.phoneNumber || '전화번호 없음'}</Typography>
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Typography variant="subtitle2" color="text.secondary">메일 수신</Typography>
                  <Typography variant="body1">{user?.isMailSend ? '수신함' : '수신 안함'}</Typography>
                </Grid>
                <Grid item xs={12} sm={6}>
                  <Typography variant="subtitle2" color="text.secondary">최대 선택 가능 번호</Typography>
                  {isEditing ? (
                    <TextField
                      type="number"
                      value={maxSelectNumber}
                      onChange={(e) => setMaxSelectNumber(e.target.value)}
                      variant="outlined"
                      size="small"
                      fullWidth
                      inputProps={{ min: 0 }}
                    />
                  ) : (
                    <Typography variant="body1">{user?.maxSelectNumber || 0}개</Typography>
                  )}
                </Grid>
                <Grid item xs={12}>
                  <Typography variant="subtitle2" color="text.secondary">행운의 번호</Typography>
                  {isEditing ? (
                    <Box sx={{ display: 'flex', gap: 2, mt: 1 }}>
                      <TextField
                        type="number"
                        label="번호 1"
                        value={digit1}
                        onChange={(e) => setDigit1(e.target.value)}
                        variant="outlined"
                        size="small"
                        inputProps={{ min: 0, max: 45 }}
                        sx={{ width: '100px' }}
                      />
                      <TextField
                        type="number"
                        label="번호 2"
                        value={digit2}
                        onChange={(e) => setDigit2(e.target.value)}
                        variant="outlined"
                        size="small"
                        inputProps={{ min: 0, max: 45 }}
                        sx={{ width: '100px' }}
                      />
                      <TextField
                        type="number"
                        label="번호 3"
                        value={digit3}
                        onChange={(e) => setDigit3(e.target.value)}
                        variant="outlined"
                        size="small"
                        inputProps={{ min: 0, max: 45 }}
                        sx={{ width: '100px' }}
                      />
                    </Box>
                  ) : (
                    <Box sx={{ display: 'flex', gap: 1, mt: 1 }}>
                      <Box sx={{ 
                        width: 36, 
                        height: 36, 
                        borderRadius: '50%', 
                        bgcolor: 'primary.main', 
                        display: 'flex', 
                        alignItems: 'center', 
                        justifyContent: 'center',
                        color: 'white',
                        fontWeight: 'bold'
                      }}>
                        {user?.digit1 || 0}
                      </Box>
                      <Box sx={{ 
                        width: 36, 
                        height: 36, 
                        borderRadius: '50%', 
                        bgcolor: 'primary.main', 
                        display: 'flex', 
                        alignItems: 'center', 
                        justifyContent: 'center',
                        color: 'white',
                        fontWeight: 'bold'
                      }}>
                        {user?.digit2 || 0}
                      </Box>
                      <Box sx={{ 
                        width: 36, 
                        height: 36, 
                        borderRadius: '50%', 
                        bgcolor: 'primary.main', 
                        display: 'flex', 
                        alignItems: 'center', 
                        justifyContent: 'center',
                        color: 'white',
                        fontWeight: 'bold'
                      }}>
                        {user?.digit3 || 0}
                      </Box>
                    </Box>
                  )}
                </Grid>
              </Grid>
              
              <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
                {isEditing ? (
                  <>
                    <Button 
                      variant="contained" 
                      onClick={handleUpdateUserInfo}
                      disabled={isSaving}
                      sx={{ mr: 2 }}
                    >
                      {isSaving ? '저장 중...' : '저장'}
                    </Button>
                    <Button 
                      variant="outlined" 
                      onClick={toggleEditMode}
                      disabled={isSaving}
                    >
                      취소
                    </Button>
                  </>
                ) : (
                  <Button 
                    variant="contained" 
                    onClick={toggleEditMode}
                  >
                    정보 수정
                  </Button>
                )}
              </Box>
            </Paper>
          </>
        )}
      </Container>
      
      {/* 성공 메시지 스낵바 */}
      <Snackbar
        open={!!success}
        autoHideDuration={6000}
        onClose={() => setSuccess('')}
        message={success}
      />
      
      {/* 하단에 홈으로 돌아가기 버튼 추가 */}
      <Box sx={{ textAlign: 'center', mt: 2, mb: 4 }}>
        <Button 
          variant="outlined" 
          startIcon={<HomeIcon />}
          onClick={() => navigate('/prize')}
        >
          홈으로 돌아가기
        </Button>
      </Box>
    </Box>
  );
};

export default ProfileScreen; 