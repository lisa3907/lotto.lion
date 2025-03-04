import { api } from './ApiClient';

interface ApiResponse<T = any> {
  success: boolean;
  message?: string;
  result?: T;
}

class MessageService {
  // 메시지 목록 가져오기
  async getMessages(userToken: string, page: number = 1, limit: number = 10) {
    try {
      const response = await api.get<ApiResponse>(
        '/api/messages',
        userToken,
        {
          params: {
            page,
            limit
          }
        }
      );
      
      if (!response.success) {
        throw new Error(response.message || '메시지 목록 조회에 실패했습니다.');
      }
      
      return response.result;
    } catch (error) {
      // 메시지 목록 조회 실패 시 로그 유지
      console.error('메시지 목록 조회 실패:', error);
      throw error;
    }
  }

  // 메시지 상세 정보 가져오기
  async getMessage(userToken: string, messageId: string) {
    try {
      const response = await api.get<ApiResponse>(
        `/api/messages/${messageId}`,
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '메시지 상세 정보 조회에 실패했습니다.');
      }
      
      return response.result;
    } catch (error) {
      // 메시지 상세 정보 조회 실패 시 로그 유지
      console.error('메시지 상세 정보 조회 실패:', error);
      throw error;
    }
  }

  // 메시지 읽음 상태 업데이트
  async markMessageAsRead(userToken: string, messageId: string) {
    try {
      const response = await api.put<ApiResponse>(
        `/api/messages/${messageId}/read`,
        {},
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '메시지 읽음 상태 업데이트에 실패했습니다.');
      }
      
      return response.result;
    } catch (error) {
      // 메시지 읽음 상태 업데이트 실패 시 로그 유지
      console.error('메시지 읽음 상태 업데이트 실패:', error);
      throw error;
    }
  }

  // 메시지 삭제
  async deleteMessage(userToken: string, messageId: string) {
    try {
      const response = await api.delete<ApiResponse>(
        `/api/messages/${messageId}`,
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '메시지 삭제에 실패했습니다.');
      }
      
      return response.result;
    } catch (error) {
      // 메시지 삭제 실패 시 로그 유지
      console.error('메시지 삭제 실패:', error);
      throw error;
    }
  }

  // 알림 메시지 목록 가져오기
  async getAlertMessages(userToken: string, page: number = 1, limit: number = 10) {
    try {
      const response = await api.get<ApiResponse>(
        '/api/alerts',
        userToken,
        {
          params: {
            page,
            limit
          }
        }
      );
      
      if (!response.success) {
        throw new Error(response.message || '알림 메시지 목록 조회에 실패했습니다.');
      }
      
      return response.result;
    } catch (error) {
      // 알림 메시지 목록 조회 실패 시 로그 유지
      console.error('알림 메시지 목록 조회 실패:', error);
      throw error;
    }
  }

  // 알림 개수 가져오기
  async getAlertCount(userToken: string) {
    try {
      const response = await api.get<ApiResponse>(
        '/api/alerts/count',
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '알림 개수 조회에 실패했습니다.');
      }
      
      return response.result;
    } catch (error) {
      // 알림 개수 조회 실패 시 로그 유지
      console.error('알림 개수 조회 실패:', error);
      throw error;
    }
  }

  // 모든 알림 삭제
  async clearAlerts(userToken: string) {
    try {
      const response = await api.delete<ApiResponse>(
        '/api/alerts',
        userToken
      );
      
      if (!response.success) {
        throw new Error(response.message || '알림 삭제에 실패했습니다.');
      }
      
      return response.result;
    } catch (error) {
      // 알림 삭제 실패 시 로그 유지
      console.error('알림 삭제 실패:', error);
      throw error;
    }
  }
}

export const messageService = new MessageService(); 