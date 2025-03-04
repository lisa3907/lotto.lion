import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';

// 기본 API 클라이언트 설정
const apiClient: AxiosInstance = axios.create({
  headers: {
    'Content-Type': 'application/x-www-form-urlencoded',
  },
});

// 응답 인터셉터 설정
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // 에러 처리 로직
    if (error.response) {
      // 서버 응답이 있는 경우 - 401, 403, 500 등의 에러 코드 확인을 위해 로그 유지
      console.error('API 에러 응답:', error.response.status, error.response.data);
      
      // 401 Unauthorized 에러 처리
      if (error.response.status === 401) {
        // 로컬 스토리지에서 토큰 제거
        localStorage.removeItem('authToken');
        localStorage.removeItem('guestToken');
        
        // 로그인 페이지로 리다이렉트 (필요한 경우)
        // window.location.href = '/login';
      }
    } else if (error.request) {
      // 요청은 보냈지만 응답이 없는 경우 - 네트워크 문제 진단을 위해 로그 유지
      console.error('API 요청 에러:', error.request);
    } else {
      // 요청 설정 중 에러가 발생한 경우 - 요청 구성 문제 진단을 위해 로그 유지
      console.error('API 설정 에러:', error.message);
    }
    
    return Promise.reject(error);
  }
);

// 요청 인터셉터 설정
apiClient.interceptors.request.use(
  (config) => {
    // 요청 전 처리 로직
    return config;
  },
  (error) => {
    // 요청 인터셉터 에러 - 요청 구성 문제 진단을 위해 로그 유지
    console.error('API 요청 인터셉터 에러:', error);
    return Promise.reject(error);
  }
);

// API 요청 함수
export const apiRequest = async <T>(
  method: string,
  url: string,
  data?: any,
  token?: string,
  config?: AxiosRequestConfig
): Promise<T> => {
  try {
    const headers: Record<string, string> = {};
    
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    
    const requestConfig: AxiosRequestConfig = {
      ...config,
      method,
      url,
      data,
      headers: {
        ...headers,
        ...(config?.headers || {})
      }
    };
    
    const response: AxiosResponse<T> = await apiClient(requestConfig);
    return response.data;
  } catch (error) {
    // API 요청 실패 - 디버깅을 위해 로그 유지
    console.error(`API ${method} 요청 실패:`, url, error);
    throw error;
  }
};

// API 클라이언트 객체
export const api = {
  get: <T>(url: string, token?: string, config?: AxiosRequestConfig) => 
    apiRequest<T>('GET', url, undefined, token, config),
  
  post: <T>(url: string, data?: any, token?: string, config?: AxiosRequestConfig) => 
    apiRequest<T>('POST', url, data, token, config),
  
  put: <T>(url: string, data?: any, token?: string, config?: AxiosRequestConfig) => 
    apiRequest<T>('PUT', url, data, token, config),
  
  delete: <T>(url: string, token?: string, config?: AxiosRequestConfig) => 
    apiRequest<T>('DELETE', url, undefined, token, config)
};

export default apiClient; 