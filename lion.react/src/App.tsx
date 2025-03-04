import React from 'react';
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import { AuthProvider } from './contexts/AuthContext';
import { LottoProvider } from './contexts/LottoContext';
import AppNavigator from './navigation/AppNavigator';

// 테마 설정
const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#f50057',
    },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AuthProvider>
        <LottoProvider>
          <AppNavigator />
        </LottoProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;
