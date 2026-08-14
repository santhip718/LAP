import Typography from "@mui/material/Typography";
import { TRUSTED_BY_CONSTANTS } from "./TrustedBy.constants";
import "./TrustedBySection.css";

export default function TrustedBySection() {
  return (
    <section className="trusted-section">
      <div className="trusted-inner">
        <Typography variant="caption" component="p" className="trusted-label">
          {TRUSTED_BY_CONSTANTS.label}
        </Typography>
        <div className="marquee-wrapper">
          <div className="trusted-logos">
            {TRUSTED_BY_CONSTANTS.logos.map((logo) => (
              <div key={logo.name} className="trusted-logo">
                <span className="material-symbols-outlined">{logo.icon}</span>
                {logo.name}
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}
