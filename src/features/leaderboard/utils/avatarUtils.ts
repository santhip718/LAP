import { LEADERBOARD_AVATARS } from '../constants/leaderboardAvatars';

export const getUserAvatar = (id: string): string => {
  return LEADERBOARD_AVATARS[id] || '';
};
