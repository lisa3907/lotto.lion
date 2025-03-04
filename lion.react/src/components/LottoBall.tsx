import React from 'react';
import { Box, Typography } from '@mui/material';

interface LottoBallProps {
  number: number;
  isBonus?: boolean;
  isSelected?: boolean;
  isHighlighted?: boolean;
  onClick?: () => void;
}

const getLottoBallColor = (number: number): string => {
  if (number <= 10) return '#FBC400'; // 노란색
  if (number <= 20) return '#69C8F2'; // 파란색
  if (number <= 30) return '#FF7272'; // 빨간색
  if (number <= 40) return '#AAAAAA'; // 회색
  return '#B0D840'; // 초록색
};

const LottoBall: React.FC<LottoBallProps> = ({
  number,
  isBonus = false,
  isSelected = false,
  isHighlighted = false,
  onClick
}) => {
  return (
    <Box
      onClick={onClick}
      sx={{
        width: 40,
        height: 40,
        borderRadius: '50%',
        backgroundColor: getLottoBallColor(number),
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        position: 'relative',
        cursor: onClick ? 'pointer' : 'default',
        opacity: isSelected || isHighlighted ? 1 : 0.7,
        transition: 'all 0.2s',
        boxShadow: isHighlighted ? '0 0 10px 3px rgba(255, 215, 0, 0.7)' : 'none',
        '&:hover': {
          opacity: onClick ? 1 : isSelected || isHighlighted ? 1 : 0.7,
          transform: onClick ? 'scale(1.1)' : 'none'
        }
      }}
    >
      <Typography
        variant="body1"
        sx={{
          color: 'white',
          fontWeight: 'bold',
          fontSize: '1.2rem'
        }}
      >
        {number}
      </Typography>
      {isBonus && (
        <Box
          sx={{
            position: 'absolute',
            top: -10,
            right: -10,
            backgroundColor: '#FF4081',
            color: 'white',
            borderRadius: '50%',
            width: 20,
            height: 20,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontSize: '0.8rem',
            fontWeight: 'bold'
          }}
        >
          +
        </Box>
      )}
    </Box>
  );
};

export default LottoBall; 