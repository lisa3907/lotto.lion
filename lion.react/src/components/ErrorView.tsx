import React from 'react';
import { Box, Typography, Button } from '@mui/material';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';

interface ErrorViewProps {
  message: string;
  onRetry?: () => void;
}

const ErrorView: React.FC<ErrorViewProps> = ({ message, onRetry }) => {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        height: '100vh',
        gap: 2
      }}
    >
      <ErrorOutlineIcon color="error" sx={{ fontSize: 64 }} />
      <Typography variant="h6" color="error" align="center">
        {message}
      </Typography>
      {onRetry && (
        <Button
          variant="contained"
          onClick={onRetry}
          sx={{ mt: 2 }}
        >
          다시 시도
        </Button>
      )}
    </Box>
  );
};

export default ErrorView; 