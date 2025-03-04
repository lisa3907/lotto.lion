import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

// 화면 컴포넌트들 임포트
import PrizeScreen from '../screens/PrizeScreen';
import MyNumbersScreen from '../screens/MyNumbersScreen';
import StoreScreen from '../screens/StoreScreen';
import SettingsScreen from '../screens/SettingsScreen';
import LoginScreen from '../screens/LoginScreen';
import SignUpScreen from '../screens/SignUpScreen';
import MessageScreen from '../screens/MessageScreen';
import CountSetScreen from '../screens/CountSetScreen';
import EmailChangeScreen from '../screens/EmailChangeScreen';
import PasswordChangeScreen from '../screens/PasswordChangeScreen';
import LogoutScreen from '../screens/LogoutScreen';
import ProfileScreen from '../screens/ProfileScreen';

// 보호된 라우트 컴포넌트
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />;
};

const AppNavigator: React.FC = () => {
  return (
    <Routes>
      {/* 공개 라우트 */}
      <Route path="/login" element={<LoginScreen />} />
      <Route path="/signup" element={<SignUpScreen />} />
      <Route path="/prize" element={<PrizeScreen />} />
      
      {/* 보호된 라우트 */}
      <Route path="/profile" element={
        <ProtectedRoute>
          <ProfileScreen />
        </ProtectedRoute>
      } />
      <Route path="/numbers" element={
        <ProtectedRoute>
          <MyNumbersScreen />
        </ProtectedRoute>
      } />
      <Route path="/store" element={
        <ProtectedRoute>
          <StoreScreen />
        </ProtectedRoute>
      } />
      <Route path="/settings" element={
        <ProtectedRoute>
          <SettingsScreen />
        </ProtectedRoute>
      } />
      <Route path="/messages" element={
        <ProtectedRoute>
          <MessageScreen />
        </ProtectedRoute>
      } />
      <Route path="/count-set" element={
        <ProtectedRoute>
          <CountSetScreen />
        </ProtectedRoute>
      } />
      <Route path="/email-change" element={
        <ProtectedRoute>
          <EmailChangeScreen />
        </ProtectedRoute>
      } />
      <Route path="/password-change" element={
        <ProtectedRoute>
          <PasswordChangeScreen />
        </ProtectedRoute>
      } />
      <Route path="/logout" element={
        <ProtectedRoute>
          <LogoutScreen />
        </ProtectedRoute>
      } />
      
      {/* 기본 리다이렉트 */}
      <Route path="/" element={<Navigate to="/prize" replace />} />
    </Routes>
  );
};

export default AppNavigator; 