import { Box, Paper, Typography } from '@mui/material';
import type { LeaderboardUser } from '@/features/leaderboard/types/leaderboard.types';
import { capitalizeFirst } from '@/shared/utils/stringUtils';
import { LEADERBOARD_STATS_UI } from './LeaderboardStats.constants';

interface LeaderboardStatsProps {
  leaderboard: LeaderboardUser[];
}

export default function LeaderboardStats({ leaderboard }: LeaderboardStatsProps) {
  if (leaderboard.length === 0) return null;

  const topPerformer = leaderboard.find((x) => x.rank === 1);
  const highestScore = Math.max(...leaderboard.map((x) => x.overall_weighted_score));
  const participantCount = leaderboard.length;

  return (
    <Box
      sx={{
        display: 'flex',
        gap: 2,
        mb: 3,
        flexWrap: 'wrap',
      }}
    >
      {topPerformer && (
        <Paper
          elevation={0}
          sx={{
            flex: '1 1 180px',
            p: 2,
            borderRadius: '16px',
            background: 'linear-gradient(135deg, #FDE68A, #FBBF24)',
            boxShadow: 'var(--card-shadow)',
            textAlign: 'center',
          }}
        >
          <Typography sx={{ fontSize: '12px', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.5px', opacity: 0.7 }}>
            {LEADERBOARD_STATS_UI.LABEL_TOP_PERFORMER}
          </Typography>
          <Typography sx={{ fontSize: '20px', fontWeight: 700, mt: 0.5 }}>
            {capitalizeFirst(topPerformer.full_name)}
          </Typography>
        </Paper>
      )}

      <Paper
        elevation={0}
        sx={{
          flex: '1 1 160px',
          p: 2,
          borderRadius: '16px',
          background: 'linear-gradient(135deg, #C7D2FE, #818CF8)',
          boxShadow: 'var(--card-shadow)',
          textAlign: 'center',
        }}
      >
        <Typography sx={{ fontSize: '12px', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.5px', opacity: 0.7 }}>
          {LEADERBOARD_STATS_UI.LABEL_HIGHEST_SCORE}
        </Typography>
        <Typography sx={{ fontSize: '20px', fontWeight: 700, mt: 0.5 }}>
          {highestScore}
        </Typography>
      </Paper>

      <Paper
        elevation={0}
        sx={{
          flex: '1 1 140px',
          p: 2,
          borderRadius: '16px',
          background: 'linear-gradient(135deg, #A7F3D0, #34D399)',
          boxShadow: 'var(--card-shadow)',
          textAlign: 'center',
        }}
      >
        <Typography sx={{ fontSize: '12px', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.5px', opacity: 0.7 }}>
          {LEADERBOARD_STATS_UI.LABEL_PARTICIPANTS}
        </Typography>
        <Typography sx={{ fontSize: '20px', fontWeight: 700, mt: 0.5 }}>
          {participantCount}
        </Typography>
      </Paper>
    </Box>
  );
}
