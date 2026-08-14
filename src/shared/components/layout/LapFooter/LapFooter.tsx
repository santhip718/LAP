import Typography from "@mui/material/Typography";
import { FOOTER, FOOTER_SECTIONS, COPYRIGHT } from "./LapFooter.constants";
import logo from "@/assets/images/info-guide-logo.png";
import "./LapFooter.css";

export default function LapFooter() {
  return (
    <footer className="footer">
      <div className="footer-inner">
        <div className="footer-brand">
          <a href="#" className="footer-logo">
            <img src={logo} alt={FOOTER.BRAND} className="footer-logo-img" />
          </a>
          <Typography variant="body1" className="footer-tagline">
            {FOOTER.TAGLINE}
          </Typography>
        </div>
        {FOOTER_SECTIONS.map((section) => (
          <div key={section.heading}>
            <Typography
              variant="caption"
              className="footer-heading"
              component="h5"
            >
              {section.heading}
            </Typography>
            <ul className="footer-links">
              {section.links.map((link) => (
                <li key={link.label}>
                  <a href={link.href} className="footer-link">
                    <Typography variant="body2" component="span">
                      {link.label}
                    </Typography>
                  </a>
                </li>
              ))}
            </ul>
          </div>
        ))}
      </div>
      <div className="footer-bottom">
        <Typography
          variant="caption"
          className="footer-copyright"
          component="p"
        >
          {COPYRIGHT}
        </Typography>
      </div>
    </footer>
  );
}
