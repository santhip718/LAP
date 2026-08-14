import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Typography from "@mui/material/Typography";
import type { AssessmentOverviewDto } from "@/shared/services/api/models/assessmentOverviewDto";
import "./SummaryCard.css";

interface SummaryCardProps {
  assessment: AssessmentOverviewDto;
  totalMarks: number;
  questionCount: number;
  labels: {
    LABEL_TITLE: string;
    LABEL_DURATION: string;
    LABEL_DESCRIPTION: string;
    LABEL_TOTAL_MARKS: string;
    LABEL_PASSING_MARKS: string;
    LABEL_QUESTIONS_COUNT: string;
    UNTITLED: string;
    DURATION_SUFFIX: string;
    DASH: string;
    NO_DESCRIPTION: string;
  };
}

export default function SummaryCard({
  assessment,
  totalMarks,
  questionCount,
  labels: L,
}: SummaryCardProps) {
  return (
    <Card variant="outlined" className="summary-card">
      <CardContent className="summary-card-content">
        <div className="assessment-overview-summary-grid">
          <div className="assessment-overview-summary-field">
            <Typography variant="caption" className="assessment-overview-summary-label">{L.LABEL_TITLE}</Typography>
            <Typography variant="body1" className="assessment-overview-summary-value--semibold">
              {assessment.title ?? L.UNTITLED}
            </Typography>
          </div>
          <div className="assessment-overview-summary-field">
            <Typography variant="caption" className="assessment-overview-summary-label">{L.LABEL_DURATION}</Typography>
            <Typography variant="body1" className="assessment-overview-summary-value--semibold">
              {assessment.duration_minute != null
                ? `${assessment.duration_minute} ${L.DURATION_SUFFIX}`
                : L.DASH}
            </Typography>
          </div>
          <div className="assessment-overview-summary-field assessment-overview-summary-field--full">
            <Typography variant="caption" className="assessment-overview-summary-label">{L.LABEL_DESCRIPTION}</Typography>
            <Typography variant="body1" sx={{ lineHeight: 1.6 }}>
              {assessment.description || L.NO_DESCRIPTION}
            </Typography>
          </div>
          <div className="assessment-overview-summary-marks">
            <div className="assessment-overview-summary-marks-item">
              <Typography variant="caption">{L.LABEL_TOTAL_MARKS}</Typography>
              <Typography variant="h4" className="text-primary">{totalMarks}</Typography>
            </div>
            <div className="assessment-overview-summary-marks-item">
              <Typography variant="caption">{L.LABEL_PASSING_MARKS}</Typography>
              <Typography variant="h4" className="text-secondary">{assessment.passing_mark ?? L.DASH}</Typography>
            </div>
            <div className="assessment-overview-summary-marks-item">
              <Typography variant="caption">{L.LABEL_QUESTIONS_COUNT}</Typography>
              <Typography variant="h4" className="text-primary-container">{questionCount}</Typography>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
