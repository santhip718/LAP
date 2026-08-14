import { Box, Typography } from '@mui/material';
import LapErrorBoundary from '@/shared/components/feedback/LapErrorBoundary/LapErrorBoundary';
import LapSpinnerv1 from '@/shared/components/ui/LapSpinnerv1/LapSpinnerv1';
import { useLeaderboard } from '../hooks/useLeaderboard';
import LeaderboardStats from '@/shared/components/ui/LapLeaderboardStats/LeaderboardStats';
import LeaderboardPodium from '@/shared/components/ui/LapLeaderboardPodium/LeaderboardPodium';
import LapDataTable from '@/shared/components/ui/LapDataTable/LapDataTable';
import LapNoContent from '@/shared/components/ui/LapNoContent/LapNoContent';
import { leaderboardColumns } from '../utils/leaderboardTableConfig.tsx';
import { LEADERBOARD_UI } from './LeaderboardPage.constants';
import type { LeaderboardUser } from '../types/leaderboard.types';
import './LeaderboardPage.css';

export default function LeaderboardPage() {
  const { leaderboard, loading, error, refetch } = useLeaderboard();

  if (loading) {
    return <LapSpinnerv1 />;
  }

  // ── ERROR STATE ──────────────────────────────────────────────────────────────
  if (error) {
    return (
      <LapErrorBoundary>
        <Box className="leaderboard-page">
          <Box component="main" className="leaderboard-main">
            <LapNoContent
              title={LEADERBOARD_UI.ERROR_TITLE}
              message={LEADERBOARD_UI.ERROR_MESSAGE}
            >
              <button onClick={refetch}>{LEADERBOARD_UI.BTN_RETRY}</button>
            </LapNoContent>
          </Box>
        </Box>
      </LapErrorBoundary>
    );
  }

  // ── EMPTY STATE ──────────────────────────────────────────────────────────────
  if (leaderboard.length === 0) {
    return (
      <LapErrorBoundary>
        <Box className="leaderboard-page">
          <Box component="main" className="leaderboard-main">
            <Box className="leaderboard-header">
              <Typography
                variant="h1"
                className="leaderboard-title"
                sx={{
                  fontSize: { xs: '22px', sm: '28px', md: '36px' },
                  lineHeight: { xs: '30px', sm: '36px', md: '44px' },
                  wordBreak: 'break-word',
                  overflowWrap: 'break-word',
                  whiteSpace: 'normal',
                }}
              >
                {LEADERBOARD_UI.PAGE_TITLE}
              </Typography>
              <Typography variant="body1" className="leaderboard-subtitle">
                {LEADERBOARD_UI.PAGE_SUBTITLE}
              </Typography>
            </Box>
            <LapNoContent
              icon={LEADERBOARD_UI.EMPTY_ICON}
              title=""
              message={LEADERBOARD_UI.EMPTY_MESSAGE}
            />
          </Box>
        </Box>
      </LapErrorBoundary>
    );
  }

  // ── NORMAL STATE ─────────────────────────────────────────────────────────────
  return (
    <LapErrorBoundary>
      <Box className="leaderboard-page">
        <Box component="main" className="leaderboard-main">
          {/* Header */}
          <Box className="leaderboard-header">
          <Typography
            variant="h1"
            className="leaderboard-title"
            sx={{
              fontSize: { xs: '22px', sm: '28px', md: '36px' },
              lineHeight: { xs: '30px', sm: '36px', md: '44px' },
              wordBreak: 'break-word',
              overflowWrap: 'break-word',
              whiteSpace: 'normal',
            }}
          >
            {LEADERBOARD_UI.PAGE_TITLE}
          </Typography>
          <Typography variant="body1" className="leaderboard-subtitle">
            {LEADERBOARD_UI.PAGE_SUBTITLE}
          </Typography>
        </Box>

        {/* Stats section */}
        <LeaderboardStats leaderboard={leaderboard} />

          {/* Podium section */}
          <LeaderboardPodium leaderboard={leaderboard} />

          {/* Leaderboard Table (using DataTable directly with external columns config) */}
          <LapDataTable<LeaderboardUser>
            columns={leaderboardColumns}
            data={leaderboard}
            pageSize={10}
            enableInfiniteScroll
            enableSearch
            searchPlaceholder={LEADERBOARD_UI.SEARCH_PLACEHOLDER}
            searchKeys={['full_name']}
          />
        </Box>
      </Box>
    </LapErrorBoundary>
  );
}
