import type { Column } from '@/shared/types';
import type { LeaderboardUser } from '../types/leaderboard.types';
import { LEADERBOARD_UI } from '../pages/LeaderboardPage.constants';
import { getUserAvatar } from './avatarUtils';
import { capitalizeFirst } from '@/shared/utils/stringUtils';
import { Box, Avatar, Chip } from '@mui/material';
import { getTier } from '@/shared/utils/tierUtils';

export const leaderboardColumns: Column<LeaderboardUser>[] = [
  {
    key: 'rank',
    label: LEADERBOARD_UI.COL_RANK,
    sortable: true,
    render: (value: unknown) => {
      const val = Number(value);
      return (
        <span style={{ fontWeight: 700, fontSize: '16px', color: 'var(--on-surface)', paddingLeft: '8px' }}>
          {val}
        </span>
      );
    },
  },
  {
    key: 'full_name',
    label: LEADERBOARD_UI.COL_LEARNER,
    sortable: true,
    render: (value: unknown, row: LeaderboardUser) => {
      const name = value as string;
      const avatar = getUserAvatar(row.user_id);
      return (
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
          <Avatar
            src={avatar}
            sx={{
              width: 32,
              height: 32,
              fontSize: 13,
              fontWeight: 600,
              bgcolor: 'var(--primary)',
              color: 'var(--on-primary)',
            }}
          >
            {capitalizeFirst(name).charAt(0)}
          </Avatar>
          <span style={{ fontWeight: 500, color: 'var(--on-surface)' }}>{capitalizeFirst(name)}</span>
        </Box>
      );
    },
  },
  {
    key: 'overall_weighted_score',
    label: LEADERBOARD_UI.COL_SCORE,
    sortable: true,
    render: (value: unknown) => (
      <Box
        sx={{
          display: 'inline-flex',
          alignItems: 'center',
          fontWeight: 600,
          color: 'var(--on-surface)',
          fontSize: '14px',
         }}
       >
        {value !== null && value !== undefined ? (value as number).toFixed(1) : '—'}
      </Box>
    ),
  },
  {
    key: 'tier_awarded',
    label: LEADERBOARD_UI.COL_TIER,
    sortable: true,
    render: (value: unknown, row: LeaderboardUser) => {
      const score = row.overall_weighted_score ?? 0;
      const tier = (value as string) || getTier(score);
      return tier ? (
        <Chip
          label={tier}
          size="small"
          sx={{ fontWeight: 600, fontSize: '12px', textTransform: 'uppercase' }}
        />
      ) : (
        <span style={{ color: 'var(--text-secondary)' }}>—</span>
      );
    },
  },
];
