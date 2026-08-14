import Typography from "@mui/material/Typography";
import './DynamicContentSection.css';

export default function DynamicContentSection() {
  return (
    <section className="dynamic-section">
      <div className="dynamic-inner">
        <div className="dynamic-grid">
          <div className="dynamic-main">
            <img
              alt="Student collaborating with AI"
              className="dynamic-main-image"
              src="https://lh3.googleusercontent.com/aida-public/AB6AXuBHxzcjujqfwaiPEXxcAp5KbtZXYF62w3687A24JiwSWDBuUDVtLJW6Ip_YBRDeXI94kHn2KtPf9KfwkiF5-H6skz2ymmw03oJElVySzz8jFr-GwQjfAus0ToBuzCTGZxW0FdWOr_YzIwjHa3F185vcCIm2FiZQI-lYyfVQZS4L2iKh9ZnH7ZS81Ar7qK2gZgT-YMDvCroMkgRHv-3hu3KnORddnPDsu_w-ui5l9e7vVSuS5AY9Vxups7XBfUNBunjZIHn-yFhZGG0"
            />
            <div className="dynamic-main-overlay" />
            <div className="dynamic-main-content">
              <Typography variant="caption" className="dynamic-main-label">Case Study</Typography>
              <Typography variant="h4" className="dynamic-main-title">Bridging the Gap at Stanford Research</Typography>
              <Typography variant="body1" className="dynamic-main-text">
                How EduFlow helped automate data synthesis for high-energy physics curriculum development.
              </Typography>
            </div>
          </div>
          <div className="dynamic-side">
            <div className="dynamic-card">
              <div className="dynamic-card-body">
                <Typography variant="h4" className="dynamic-card-title">Automated Grading 2.0</Typography>
                <Typography variant="body2" className="dynamic-card-desc">LLM-powered qualitative feedback that understands nuances in scientific reasoning.</Typography>
              </div>
              <div className="dynamic-card-icon dynamic-card-icon-secondary">
                <span className="material-symbols-outlined dynamic-card-icon-color-secondary dynamic-card-icon-size">verified</span>
              </div>
            </div>
            <div className="dynamic-card">
              <div className="dynamic-card-body">
                <Typography variant="h4" className="dynamic-card-title">Researcher API</Typography>
                <Typography variant="body2" className="dynamic-card-desc">Direct access to our fine-tuned learning models for custom research deployments.</Typography>
              </div>
              <div className="dynamic-card-icon dynamic-card-icon-teal">
                <span className="material-symbols-outlined dynamic-card-icon-color-teal dynamic-card-icon-size">api</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
