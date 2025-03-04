/**
 * 당첨금을 한국 원화 형식으로 포맷팅합니다.
 */
export const formatPrizeAmount = (amount: string | number): string => {
  const numAmount = typeof amount === 'string' ? parseInt(amount) : amount;
  return new Intl.NumberFormat('ko-KR', {
    style: 'currency',
    currency: 'KRW'
  }).format(numAmount);
};

/**
 * 당첨 번호와 선택 번호를 비교하여 당첨 결과를 반환합니다.
 */
export const checkWinningResult = (
  winningNumbers: number[],
  bonusNumber: number,
  selectedNumbers: number[]
): {
  matchCount: number;
  bonusMatch: boolean;
  rank: number;
} => {
  const matchCount = selectedNumbers.filter(num => winningNumbers.includes(num)).length;
  const bonusMatch = selectedNumbers.includes(bonusNumber);

  // 당첨 순위 결정
  let rank = 0;
  if (matchCount === 6) rank = 1;
  else if (matchCount === 5 && bonusMatch) rank = 2;
  else if (matchCount === 5) rank = 3;
  else if (matchCount === 4) rank = 4;
  else if (matchCount === 3) rank = 5;

  return { matchCount, bonusMatch, rank };
};

/**
 * 당첨 순위에 따른 설명을 반환합니다.
 */
export const getPrizeDescription = (rank: number): string => {
  switch (rank) {
    case 1:
      return '1등 당첨! 6개 번호 모두 일치';
    case 2:
      return '2등 당첨! 5개 번호 + 보너스 번호 일치';
    case 3:
      return '3등 당첨! 5개 번호 일치';
    case 4:
      return '4등 당첨! 4개 번호 일치';
    case 5:
      return '5등 당첨! 3개 번호 일치';
    default:
      return '아쉽게도 당첨되지 않았습니다.';
  }
}; 