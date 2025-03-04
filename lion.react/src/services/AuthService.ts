import { User } from '../types';
import { api } from './ApiClient';

// 게스트 토큰 관련 API
export const getGuestToken = async (): Promise<string> => {
  try {
    const response = await api.post<{ success: boolean; message: string; result: string }>('/api/user/GetGuestToken');
    return response.result || '';
  } catch (error) {
    // 게스트 토큰 요청 실패 - 인증 초기화 문제 진단을 위해 로그 유지
    console.error('게스트 토큰 요청 실패:', error);
    throw error;
  }
};

// 로그인 API
export const login = async (loginId: string, password: string): Promise<User> => {
  try {
    const response = await api.post<{ success: boolean; message: string; result: User }>(
      '/api/user/Login',
      { loginId, password }
    );
    
    if (!response.success) {
      throw new Error(response.message || '로그인에 실패했습니다.');
    }
    
    return response.result;
  } catch (error) {
    // 로그인 실패 - 인증 문제 진단을 위해 로그 유지
    console.error('로그인 실패:', error);
    throw error;
  }
};

// 회원가입 API
export const signUp = async (data: {
  email: string;
  name: string;
  password: string;
  verificationCode: string;
}): Promise<User> => {
  try {
    const response = await api.post<{ success: boolean; message: string; result: User }>(
      '/api/user/SignUp',
      data
    );
    
    if (!response.success) {
      throw new Error(response.message || '회원가입에 실패했습니다.');
    }
    
    return response.result;
  } catch (error) {
    // 회원가입 실패 - 사용자 등록 문제 진단을 위해 로그 유지
    console.error('회원가입 실패:', error);
    throw error;
  }
};

// 이메일 인증 코드 전송 API
export const sendVerificationEmail = async (email: string): Promise<void> => {
  try {
    const response = await api.post<{ success: boolean; message: string }>(
      '/api/user/SendVerificationEmail',
      { email }
    );
    
    if (!response.success) {
      throw new Error(response.message || '인증 이메일 전송에 실패했습니다.');
    }
  } catch (error) {
    // 인증 이메일 전송 실패 - 이메일 전송 문제 진단을 위해 로그 유지
    console.error('인증 이메일 전송 실패:', error);
    throw error;
  }
};

// 이메일 인증 코드 확인 API
export const verifyEmail = async (email: string, code: string): Promise<boolean> => {
  try {
    const response = await api.post<{ success: boolean; message: string }>(
      '/api/user/VerifyEmail',
      { email, code }
    );
    return response.success;
  } catch (error) {
    // 이메일 인증 실패 - 인증 코드 검증 문제 진단을 위해 로그 유지
    console.error('이메일 인증 실패:', error);
    throw error;
  }
};

// 로그아웃 API
export const logout = async (token: string): Promise<void> => {
  try {
    await api.post<{ success: boolean; message: string }>(
      '/api/user/Logout',
      null,
      token
    );
  } catch (error) {
    // 로그아웃 실패 - 세션 종료 문제 진단을 위해 로그 유지
    console.error('로그아웃 실패:', error);
    throw error;
  }
};

// 사용자 정보 업데이트 API
export const updateProfile = async (
  token: string,
  data: {
    name?: string;
    email?: string;
    password?: string;
  }
): Promise<User> => {
  try {
    const response = await api.post<{ success: boolean; message: string; result: User }>(
      '/api/user/UpdateProfile',
      data,
      token
    );
    
    if (!response.success) {
      throw new Error(response.message || '프로필 업데이트에 실패했습니다.');
    }
    
    return response.result;
  } catch (error) {
    // 프로필 업데이트 실패 - 사용자 정보 변경 문제 진단을 위해 로그 유지
    console.error('프로필 업데이트 실패:', error);
    throw error;
  }
};

// 인증 서비스 객체
export const authService = {
  getGuestToken,
  login,
  signUp,
  sendVerificationEmail,
  verifyEmail,
  logout,
  updateProfile
}; 