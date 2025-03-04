import React, { createContext, useContext, useState, useEffect, useCallback, useMemo, ReactNode } from 'react';
import { AuthState, User } from '../types';
import { authService } from '../services';
import { userService } from '../services';

interface AuthContextType extends AuthState {
  login: (loginId: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  signUp: (data: {
    email: string;
    name: string;
    password: string;
    verificationCode: string;
  }) => Promise<void>;
  updateProfile: (data: {
    name?: string;
    email?: string;
    password?: string;
  }) => Promise<void>;
  sendVerificationEmail: (email: string) => Promise<void>;
  verifyEmail: (email: string, code: string) => Promise<boolean>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [state, setState] = useState<AuthState>({
    user: null,
    guestToken: null,
    isAuthenticated: false,
    isLoading: true,
    error: ''
  });

  // 초기 게스트 토큰 및 사용자 정보 로드
  useEffect(() => {
    const initializeAuth = async () => {
      try {
        setState(prev => ({ ...prev, isLoading: true }));
        
        // 로컬 스토리지에서 토큰 가져오기
        const storedToken = localStorage.getItem('authToken');
        const storedGuestToken = localStorage.getItem('guestToken');
        
        // 게스트 토큰이 없으면 새로 요청
        let guestToken = storedGuestToken;
        if (!guestToken) {
          guestToken = await authService.getGuestToken();
          localStorage.setItem('guestToken', guestToken);
        }
        
        // 인증 토큰이 있으면 사용자 정보 가져오기
        if (storedToken) {
          try {
            const user = await userService.getUserInfo(storedToken);
            setState({
              user,
              guestToken,
              isAuthenticated: true,
              isLoading: false,
              error: ''
            });
          } catch (error) {
            // 토큰이 유효하지 않은 경우
            localStorage.removeItem('authToken');
            setState({
              user: null,
              guestToken,
              isAuthenticated: false,
              isLoading: false,
              error: '세션이 만료되었습니다. 다시 로그인해주세요.'
            });
          }
        } else {
          // 인증 토큰이 없는 경우
          setState({
            user: null,
            guestToken,
            isAuthenticated: false,
            isLoading: false,
            error: ''
          });
        }
      } catch (error) {
        // 인증 초기화 실패 - 앱 시작 시 인증 상태 설정 문제 진단을 위해 로그 유지
        console.error('인증 초기화 실패:', error);
        setState(prev => ({
          ...prev,
          isLoading: false,
          error: '인증 초기화에 실패했습니다.'
        }));
      }
    };

    initializeAuth();
  }, []);

  // 로그인 함수
  const login = useCallback(async (loginId: string, password: string) => {
    try {
      setState(prev => ({ ...prev, isLoading: true, error: '' }));
      
      const user = await authService.login(loginId, password);
      
      // 토큰 저장
      localStorage.setItem('authToken', user.token);
      
      setState(prev => ({
        user,
        guestToken: prev.guestToken, // ✅ prev를 사용하면 state에 대한 의존성 제거 가능
        isAuthenticated: true,
        isLoading: false,
        error: ''
      }));
    } catch (error) {
      // 로그인 실패 - 사용자 인증 문제 진단을 위해 로그 유지
      console.error('로그인 실패:', error);
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: error instanceof Error ? error.message : '로그인에 실패했습니다.'
      }));
      throw error;
    }
  }, []);

  // 로그아웃 함수
  const logout = useCallback(async () => 
  {
    try {
      setState(prev => ({ ...prev, isLoading: true }));
      
      if (state.user?.token) {
        await authService.logout(state.user.token);
      }
    } 
    catch (error) {
      // 로그아웃 실패 - 세션 종료 문제 진단을 위해 로그 유지
      console.error('로그아웃 실패:', error);
    } 
    finally {
      localStorage.removeItem('authToken');
      
      setState(prev => ({
        ...prev,
        user: null,
        isAuthenticated: false,
        isLoading: false,
        error: ''
      }));
    }
  }, [state.user]);

  // 회원가입 함수
  const signUp = useCallback(async (data: {
    email: string;
    name: string;
    password: string;
    verificationCode: string;
  }) => {
    try {
      setState(prev => ({ ...prev, isLoading: true, error: '' }));
      
      const user = await authService.signUp(data);
      
      // 토큰 저장
      localStorage.setItem('authToken', user.token);
      
      setState({
        user,
        guestToken: state.guestToken,
        isAuthenticated: true,
        isLoading: false,
        error: ''
      });
    } catch (error) {
      // 회원가입 실패 - 사용자 등록 문제 진단을 위해 로그 유지
      console.error('회원가입 실패:', error);
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: error instanceof Error ? error.message : '회원가입에 실패했습니다.'
      }));
      throw error;
    }
  }, [state.guestToken]);

  // 프로필 업데이트 함수
  const updateProfile = useCallback(async (data: {
    name?: string;
    email?: string;
    password?: string;
  }) => {
    try {
      if (!state.user?.token) {
        throw new Error('인증되지 않은 사용자입니다.');
      }
      
      setState(prev => ({ ...prev, isLoading: true, error: '' }));
      
      const updatedUser = await authService.updateProfile(state.user.token, data);
      
      setState(prev => ({
        ...prev,
        user: updatedUser,
        isLoading: false,
        error: ''
      }));
    } catch (error) {
      // 프로필 업데이트 실패 - 사용자 정보 변경 문제 진단을 위해 로그 유지
      console.error('프로필 업데이트 실패:', error);
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: error instanceof Error ? error.message : '프로필 업데이트에 실패했습니다.'
      }));
      throw error;
    }
  }, [state.user]);

  // 이메일 인증 코드 전송 함수
  const sendVerificationEmail = useCallback(async (email: string) => {
    try {
      setState(prev => ({ ...prev, isLoading: true, error: '' }));
      
      await authService.sendVerificationEmail(email);
      
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: ''
      }));
    } 
    catch (error) 
    {
      // 인증 이메일 전송 실패 - 이메일 전송 문제 진단을 위해 로그 유지
      console.error('인증 이메일 전송 실패:', error);

      setState(prev => ({
        ...prev,
        isLoading: false,
        error: error instanceof Error ? error.message : '인증 이메일 전송에 실패했습니다.'
      }));
      
      throw error;
    }
  }, []);

  // 이메일 인증 코드 확인 함수
  const verifyEmail = useCallback(async (email: string, code: string) => {
    try {
      setState(prev => ({ ...prev, isLoading: true, error: '' }));
      
      const result = await authService.verifyEmail(email, code);
      
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: ''
      }));
      
      return result;
    } catch (error) {
      // 이메일 인증 실패 - 인증 코드 검증 문제 진단을 위해 로그 유지
      console.error('이메일 인증 실패:', error);
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: error instanceof Error ? error.message : '이메일 인증에 실패했습니다.'
      }));
      throw error;
    }
  }, []);

  // 컨텍스트 값 메모이제이션
  const contextValue = useMemo(() => ({
    ...state,
    login,
    logout,
    signUp,
    updateProfile,
    sendVerificationEmail,
    verifyEmail
  }), [
    state,
    login,
    logout,
    signUp,
    updateProfile,
    sendVerificationEmail,
    verifyEmail
  ]);

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext; 