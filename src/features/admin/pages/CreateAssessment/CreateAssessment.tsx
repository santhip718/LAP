import { useParams, useNavigate } from "react-router-dom";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import AssessmentForm from "@/features/admin/components/AssessmentForm/AssessmentForm";
import LapButton from "@/shared/components/ui/LapButton/LapButton";
import "./CreateAssessment.css";

export default function CreateAssessment() {
  const { courseId } = useParams<{ courseId: string }>();
  const navigate = useNavigate();

  if (!courseId) {
    return (
      <Box className="create-assessment-page">
        <Typography color="error">Course ID is missing.</Typography>
      </Box>
    );
  }

  const handleSuccess = () => {
    navigate(`/admin/courses/${courseId}`);
  };

  const handleCancel = () => {
    navigate(`/admin/courses/${courseId}`);
  };

  return (
    <Box className="create-assessment-page">
      <Box className="create-assessment-container">
        <header className="create-assessment-header">
          <LapButton
            type="ghost"
            className="create-assessment-back-btn"
            onClick={handleCancel}
            icon={<span className="material-symbols-outlined">arrow_back</span>}
          >
            Back to Course
          </LapButton>
          <Typography variant="h4" className="create-assessment-title">
            Create Assessment
          </Typography>
          <Typography variant="body2" className="create-assessment-subtitle">
            Upload questions and set configuration for the course assessment.
          </Typography>
        </header>

        <Box className="create-assessment-card">
          <AssessmentForm
            courseId={courseId}
            onSuccess={handleSuccess}
            onCancel={handleCancel}
          />
        </Box>
      </Box>
    </Box>
  );
}
