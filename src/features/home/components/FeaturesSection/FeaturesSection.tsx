import Typography from "@mui/material/Typography";
import { FEATURES_CONSTANTS } from "./Features.constants";
import "./FeaturesSection.css";

export default function FeaturesSection() {
  return (
    <section className="features-section">
      <div className="features-inner">
        <div className="features-header">
          <Typography variant="h2" className="features-title">
            {FEATURES_CONSTANTS.title}
          </Typography>
          <Typography variant="body1" className="features-subtitle">
            {FEATURES_CONSTANTS.subtitle}
          </Typography>
        </div>
        <div className="features-grid">
          {FEATURES_CONSTANTS.list.map((f) => (
            <div key={f.name} className={`feature-card ${f.cardClass}`}>
              <div className={`feature-icon ${f.iconClass}`}>
                <span
                  className={`material-symbols-outlined ${f.iconClass.split(" ").slice(-1)[0]}`}
                >
                  {f.icon}
                </span>
              </div>
              <Typography variant="h3" className={`feature-name ${f.nameClass}`}>
                {f.name}
              </Typography>
              <Typography variant="body1" className={`feature-desc ${f.descClass}`}>
                {f.desc}
              </Typography>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
