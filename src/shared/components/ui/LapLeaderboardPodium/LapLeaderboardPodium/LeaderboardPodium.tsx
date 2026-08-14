import { Box, Avatar, Paper, Typography } from '@mui/material';
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents';
import MilitaryTechIcon from '@mui/icons-material/MilitaryTech';
import type { LeaderboardUser } from '@/features/leaderboard/types/leaderboard.types';
import { getUserAvatar } from '@/features/leaderboard/utils/avatarUtils';
import { capitalizeFirst } from '@/shared/utils/stringUtils';
import { PODIUM_CARD_CONFIGS } from './LeaderboardPodium.constants';

interface LeaderboardPodiumProps {
  leaderboard: LeaderboardUser[];
}

export default function LeaderboardPodium({ leaderboard }: LeaderboardPodiumProps) {
  const first = leaderboard.find((x) => x.rank === 1);
  const second = leaderboard.find((x) => x.rank === 2);
  const third = leaderboard.find((x) => x.rank === 3);

  const renderPodiumCard = (
    user: LeaderboardUser | undefined,
    rank: 1 | 2 | 3
  ) => {
    if (!user) return null;

    const avatar = getUserAvatar(user.full_name);
    
    let avatarSize = 84;
    let borderColor = 'var(--on-surface-variant)';
    let gradient = 'linear-gradient(180deg, var(--surface-container-high), var(--on-surface-variant))';
    let cardHeight = '200px';

    if (rank === 1) {
      avatarSize = 96;
      borderColor = 'var(--podium-gold-border, #FBBF24)';
      gradient = 'linear-gradient(180deg, var(--podium-gold-light, #FDE68A), var(--podium-gold-border, #FBBF24))';
      cardHeight = '240px';
    } else if (rank === 3) {
      avatarSize = 84;
      borderColor = 'var(--podium-bronze-border, #EA580C)';
      gradient = 'linear-gradient(180deg, var(--podium-bronze-light, #FED7AA), var(--podium-bronze-border, #EA580C))';
      cardHeight = '180px';
    }

    return (
      <Box
        className={`lb-podium-column lb-podium-rank-${rank}`}
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          width: { xs: 0, sm: '220px' },
          flex: { xs: '1 1 1px', sm: '0 0 auto' },
          minWidth: { xs: 0, sm: 'unset' },
          position: 'relative',
          mt: rank === 1 ? 0 : { xs: 0, sm: 5 },
        }}
      >
        <Box sx={{ fontSize: '32px', mb: 1, lineHeight: 1, color: rank === 1 ? '#FFD700' : rank === 2 ? '#C0C0C0' : '#CD7F32' }}>
          {rank === 1 ? <EmojiEventsIcon sx={{ fontSize: 32 }} /> : <MilitaryTechIcon sx={{ fontSize: 32 }} />}
        </Box>

        <Avatar
          src={avatar}
          sx={{
            width: avatarSize,
            height: avatarSize,
            border: `4px solid ${borderColor}`,
            boxShadow: 'var(--podium-avatar-shadow, 0 4px 12px rgba(0,0,0,0.15))',
            zIndex: 2,
            bgcolor: 'var(--primary)',
            color: 'var(--on-primary)',
            fontWeight: 'bold',
            fontSize: rank === 1 ? '28px' : '24px',
            transform: 'translateY(16px)',
          }}
        >
          {capitalizeFirst(user.full_name).charAt(0)}
        </Avatar>

        <Paper
          elevation={0}
          sx={{
            width: '100%',
            minHeight: cardHeight,
            background: gradient,
            borderRadius: '24px',
            boxShadow: 'var(--card-shadow)',
            pt: '32px',
            pb: 3,
            px: 2,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            textAlign: 'center',
            color: 'var(--on-surface)',
            zIndex: 1,
          }}
        >
          <Typography
            sx={{
              fontWeight: 700,
              fontSize: rank === 1 ? '1.1rem' : '1rem',
              mb: 0.5,
              wordBreak: 'break-word',
            }}
          >
            {capitalizeFirst(user.full_name)}
          </Typography>
          <Typography sx={{ fontSize: '14px', opacity: 0.9, mb: 0.5 }}>
            Score : {user.overall_weighted_score}
          </Typography>
          <Typography sx={{ fontWeight: 600, fontSize: '12px', opacity: 0.75 }}>
            Rank #{user.rank}
          </Typography>
        </Paper>
      </Box>
    );
  };

  if (!first && !second && !third) return null;

  return (
    <Box
      className="lb-podium"
      sx={{
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'flex-end',
        gap: { xs: 2, sm: 3 },
        mb: 6,
        mt: 2,
      }}
    >
      {renderPodiumCard(second, 2)}
      {renderPodiumCard(first, 1)}
      {renderPodiumCard(third, 3)}
    </Box>
  );
}
