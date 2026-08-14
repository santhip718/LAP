import { useNavigate } from "react-router-dom";
import Typography from "@mui/material/Typography";
import { HERO_CONSTANTS } from "./Hero.constants";
import LapButton from "@/shared/components/ui/LapButton/LapButton";
import "./HeroSection.css";

export default function HeroSection() {
  const navigate = useNavigate();

  return (
    <section className="hero">
      <div className="hero-glow" />
      <div className="hero-inner">
        <div className="hero-content">
          <div className="hero-badge">
            <span className="material-symbols-outlined hero-badge-icon">
              {HERO_CONSTANTS.badgeIcon}
            </span>
            <Typography
              variant="caption"
              component="span"
              className="hero-badge-text"
            >
              {HERO_CONSTANTS.badgeText}
            </Typography>
          </div>
          <Typography variant="h1" className="hero-title">
            {HERO_CONSTANTS.titleBefore}{" "}
            <Typography
              variant="h1"
              component="span"
              className="hero-title-accent"
            >
              {HERO_CONSTANTS.titleAccent}
            </Typography>{" "}
            {HERO_CONSTANTS.titleAfter}
          </Typography>
          <Typography
            variant="body1"
            className="hero-subtitle hero-subtitle-center"
          >
            {HERO_CONSTANTS.subtitle}
          </Typography>
          <div className="hero-actions hero-actions-center">
            <LapButton
              type="home"
              htmlType="button"
              icon={
                <span className="material-symbols-outlined">
                  {HERO_CONSTANTS.ctaIcon}
                </span>
              }
              onClick={() => navigate("/login")}
            >
              {HERO_CONSTANTS.ctaButton}
            </LapButton>
          </div>
        </div>
        <div className="hero-visual">
          <div className="hero-image-wrapper">
            <div className="hero-callout hero-callout-top">
              <Typography
                variant="caption"
                component="p"
                className="hero-callout-label hero-callout-label-secondary"
              >
                {HERO_CONSTANTS.calloutTop}
              </Typography>
              <div className="hero-callout-bar hero-callout-bar-secondary" />
            </div>
            <div className="hero-callout hero-callout-bottom">
              <Typography
                variant="caption"
                component="p"
                className="hero-callout-label hero-callout-label-primary"
              >
                {HERO_CONSTANTS.calloutBottom}
              </Typography>
              <div className="hero-callout-bar hero-callout-bar-primary" />
            </div>
            <div className="hero-callout hero-callout-side">
              <Typography
                variant="caption"
                component="p"
                className="hero-callout-label hero-callout-label-surface"
              >
                {HERO_CONSTANTS.calloutSide}
              </Typography>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
