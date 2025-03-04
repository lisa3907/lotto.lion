import React from 'react';
import { Button as MuiButton, ButtonProps as MuiButtonProps } from '@mui/material';

interface ButtonProps extends MuiButtonProps {
  loading?: boolean;
}

const Button: React.FC<ButtonProps> = ({ children, loading, ...props }) => {
  return (
    <MuiButton
      {...props}
      disabled={loading || props.disabled}
    >
      {loading ? '처리중...' : children}
    </MuiButton>
  );
};

export default Button; 