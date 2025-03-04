import { User } from '../types';
import { api } from './ApiClient';

class UserService {
  async getUserInfo(userToken: string): Promise<User> {
    try {
      const response = await api.post<{ success: boolean; message: string; result: User }>(
        '/api/user/GetUserInfor',
        null,
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '사용자 정보 조회에 실패했습니다.');
      }
      
      return response.result;
    } catch (error) {
      // 사용자 정보 조회 실패 - 프로필 정보 로드 문제 진단을 위해 로그 유지
      console.error('사용자 정보 조회 실패:', error);
      throw error;
    }
  }

  async updateUserInfo(userToken: string, data: {
    loginName: string;
    maxSelectNumber: string | number;
    digit1: string | number;
    digit2: string | number;
    digit3: string | number;
  }): Promise<User> {
    try {
      const params = {
        login_name: data.loginName,
        max_select_number: data.maxSelectNumber.toString(),
        num1: data.digit1.toString(),
        num2: data.digit2.toString(),
        num3: data.digit3.toString()
      };

      const response = await api.post<{ success: boolean; message: string }>(
        '/api/user/UpdateUserInfor',
        params,
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '사용자 정보 업데이트에 실패했습니다.');
      }
      
      // 업데이트 후 최신 사용자 정보 반환
      return this.getUserInfo(userToken);
    } catch (error) {
      // 사용자 정보 업데이트 실패 - 프로필 정보 변경 문제 진단을 위해 로그 유지
      console.error('사용자 정보 업데이트 실패:', error);
      throw error;
    }
  }

  async changePassword(userToken: string, newPassword: string): Promise<void> {
    try {
      const response = await api.post<{ success: boolean; message: string }>(
        '/api/user/ChangePassword',
        { new_password: newPassword },
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '비밀번호 변경에 실패했습니다.');
      }
    } catch (error) {
      // 비밀번호 변경 실패 - 보안 정보 변경 문제 진단을 위해 로그 유지
      console.error('비밀번호 변경 실패:', error);
      throw error;
    }
  }

  async updateEmailAddress(userToken: string, email: string, verificationCode: string): Promise<void> {
    try {
      const response = await api.post<{ success: boolean; message: string }>(
        '/api/user/UpdateEmailAddress',
        {
          email,
          verification_code: verificationCode
        },
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '이메일 주소 업데이트에 실패했습니다.');
      }
    } catch (error) {
      // 이메일 주소 업데이트 실패 - 연락처 정보 변경 문제 진단을 위해 로그 유지
      console.error('이메일 주소 업데이트 실패:', error);
      throw error;
    }
  }

  async sendMailToCheckMailAddress(userToken: string, email: string): Promise<void> {
    try {
      const response = await api.post<{ success: boolean; message: string }>(
        '/api/user/email/verify',
        { email },
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '이메일 확인 메일 전송에 실패했습니다.');
      }
    } catch (error) {
      // 이메일 확인 메일 전송 실패 - 연락처 정보 변경 문제 진단을 위해 로그 유지
      console.error('이메일 확인 메일 전송 실패:', error);
      throw error;
    }
  }

  async checkMailAddress(userToken: string, email: string, code: string): Promise<boolean> {
    try {
      const response = await api.post<{ success: boolean; message: string }>(
        '/api/user/email/verify-code',
        { email, code },
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '이메일 확인 코드 검증에 실패했습니다.');
      }
      
      return response.success;
    } catch (error) {
      // 이메일 확인 코드 검증 실패 - 연락처 정보 변경 문제 진단을 위해 로그 유지
      console.error('이메일 확인 코드 검증 실패:', error);
      throw error;
    }
  }

  async requestEmailChange(userToken: string, newEmail: string): Promise<void> {
    try {
      const response = await api.post<{ success: boolean; message: string }>(
        '/api/user/email/change-request',
        { email: newEmail },
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '이메일 변경 요청에 실패했습니다.');
      }
    } catch (error) {
      // 이메일 변경 요청 실패 - 연락처 정보 변경 문제 진단을 위해 로그 유지
      console.error('이메일 변경 요청 실패:', error);
      throw error;
    }
  }

  async confirmEmailChange(userToken: string, code: string): Promise<void> {
    try {
      const response = await api.post<{ success: boolean; message: string }>(
        '/api/user/email/change-confirm',
        { code },
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '이메일 변경 확인에 실패했습니다.');
      }
    } catch (error) {
      // 이메일 변경 확인 실패 - 연락처 정보 변경 문제 진단을 위해 로그 유지
      console.error('이메일 변경 확인 실패:', error);
      throw error;
    }
  }

  async getNotificationSettings(userToken: string): Promise<any> {
    try {
      const response = await api.get<{ success: boolean; message: string; result: any }>(
        '/api/user/notifications',
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '알림 설정 조회에 실패했습니다.');
      }
      
      return response.result;
    } catch (error) {
      // 알림 설정 조회 실패 - 알림 설정 로드 문제 진단을 위해 로그 유지
      console.error('알림 설정 조회 실패:', error);
      throw error;
    }
  }

  async updateNotificationSettings(userToken: string, settings: any): Promise<void> {
    try {
      const response = await api.put<{ success: boolean; message: string }>(
        '/api/user/notifications',
        settings,
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '알림 설정 업데이트에 실패했습니다.');
      }
    } catch (error) {
      // 알림 설정 업데이트 실패 - 알림 설정 변경 문제 진단을 위해 로그 유지
      console.error('알림 설정 업데이트 실패:', error);
      throw error;
    }
  }
}

export const userService = new UserService(); 