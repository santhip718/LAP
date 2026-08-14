import { Box, Typography } from '@mui/material';
import { useParams } from 'react-router-dom';
import LapErrorBoundary from '@/shared/components/feedback/LapErrorBoundary/LapErrorBoundary';
import LapSpinnerv1 from '@/shared/components/ui/LapSpinnerv1/LapSpinnerv1';
import LapDataTable from '@/shared/components/ui/LapDataTable/LapDataTable';
import LapNoContent from '@/shared/components/ui/LapNoContent/LapNoContent';
import LeaderboardPodium from '@/shared/components/ui/LapLeaderboardPodium/LapLeaderboardPodium/LeaderboardPodium';
import LeaderboardStats from '@/shared/components/ui/LapLeaderboardStats/LeaderboardStats';
import { leaderboardColumns } from '@/features/leaderboard/utils/leaderboardTableConfig.tsx';
import { useCourseLeaderboard } from '../../hooks/useCourseLeaderboard';
import { COURSE_LEADERBOARD_UI } from './CourseLeaderboardPage.constants';
import type { LeaderboardUser } from '@/features/leaderboard/types/leaderboard.types';
import type { CourseLeaderboardPageProps } from '@/features/leaderboard/types/course-leaderboard.types';
import './CourseLeaderboardPage.css';

export default function CourseLeaderboardPage({ courseId: propCourseId }: CourseLeaderboardPageProps = {}) {
  const { courseId: paramCourseId } = useParams<{ courseId: string }>();
  const courseId = propCourseId ?? paramCourseId ?? '';
  const { leaderboard, loading, error, refetch } = useCourseLeaderboard(courseId);

  if (loading) {
    return <LapSpinnerv1 />;
  }

  if (error) {
    return (
      <LapErrorBoundary>
        <Box className="course-leaderboard-page">
          <Box component="main" className="course-leaderboard-main">
            <LapNoContent
              title={COURSE_LEADERBOARD_UI.ERROR_TITLE}
              message={COURSE_LEADERBOARD_UI.ERROR_MESSAGE}
            >
              <button onClick={refetch}>{COURSE_LEADERBOARD_UI.BTN_RETRY}</button>
            </LapNoContent>
          </Box>
        </Box>
      </LapErrorBoundary>
    );
  }

  if (leaderboard.length === 0) {
    return (
      <LapErrorBoundary>
        <Box className="course-leaderboard-page">
          <Box component="main" className="course-leaderboard-main">
            <Box className="course-leaderboard-header">
              <Typography
                variant="h1"
                className="course-leaderboard-title"
                sx={{
                  fontSize: { xs: '22px', sm: '28px', md: '36px' },
                  lineHeight: { xs: '30px', sm: '36px', md: '44px' },
                  wordBreak: 'break-word',
                  overflowWrap: 'break-word',
                  whiteSpace: 'normal',
                }}
              >
                {COURSE_LEADERBOARD_UI.PAGE_TITLE}
              </Typography>
              <Typography variant="body1" className="course-leaderboard-subtitle">
                {COURSE_LEADERBOARD_UI.PAGE_SUBTITLE}
              </Typography>
            </Box>
            <LapNoContent
              icon={COURSE_LEADERBOARD_UI.EMPTY_ICON}
              title=""
              message={COURSE_LEADERBOARD_UI.EMPTY_MESSAGE}
            />
          </Box>
        </Box>
      </LapErrorBoundary>
    );
  }

  return (
    <LapErrorBoundary>
      <Box className="course-leaderboard-page">
        <Box component="main" className="course-leaderboard-main">
          <Box className="course-leaderboard-header">
            <Typography
              variant="h1"
              className="course-leaderboard-title"
              sx={{
                fontSize: { xs: '22px', sm: '28px', md: '36px' },
                lineHeight: { xs: '30px', sm: '36px', md: '44px' },
                wordBreak: 'break-word',
                overflowWrap: 'break-word',
                whiteSpace: 'normal',
              }}
            >
                {COURSE_LEADERBOARD_UI.PAGE_TITLE}
              </Typography>
              <Typography variant="body1" className="course-leaderboard-subtitle">
                {COURSE_LEADERBOARD_UI.PAGE_SUBTITLE}
              </Typography>
            </Box>

          <LeaderboardStats leaderboard={leaderboard} />

          <LeaderboardPodium leaderboard={leaderboard} />

          <LapDataTable<LeaderboardUser>
            columns={leaderboardColumns}
            data={leaderboard}
            pageSize={COURSE_LEADERBOARD_UI.PAGE_SIZE}
            enableInfiniteScroll
            enableSearch
            searchPlaceholder={COURSE_LEADERBOARD_UI.SEARCH_PLACEHOLDER}
            searchKeys={['full_name']}
          />
        </Box>
      </Box>
    </LapErrorBoundary>
  );
}
