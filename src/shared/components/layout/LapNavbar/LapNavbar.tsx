import { useEffect, useState } from "react";
import { NavLink, Link, useNavigate, useLocation } from "react-router-dom";
import { authService } from "../../../../features/auth/services/authService";
import { feedbackService } from "@/shared/services/feedback";
import { useAuth } from "@/core/providers/AuthProvider/useAuth";
import LapButton from "@/shared/components/ui/LapButton/LapButton";
import { ROUTES } from "@/shared/constants/routes";
import logo from "@/assets/images/info-guide-logo.png";
import { NAV_ITEMS, NAVBAR_LABELS, SCROLL_THRESHOLD } from "./LapNavbar.constants";
import { LapThemeToggle } from "@/shared/components/ui/LapThemeToggle";
import Typography from "@mui/material/Typography";
import "./LapNavbar.css";

export default function LapNavbar() {
  const [scrolled, setScrolled] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated, isAdmin, isStudent, checkAuth } = useAuth();

  useEffect(() => {
    document.body.style.overflow = menuOpen ? "hidden" : "unset";
    document.body.style.overscrollBehavior = menuOpen ? "none" : "auto";
    return () => {
      document.body.style.overflow = "unset";
      document.body.style.overscrollBehavior = "auto";
    };
  }, [menuOpen]);

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > SCROLL_THRESHOLD);
    };
    window.addEventListener("scroll", handleScroll);
    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, [location.pathname]);

  const handleLogout = async () => {
    const confirmed = await feedbackService.showConfirm({
      title: "Logout",
      message: "Are you sure you want to log out?",
      confirmLabel: "Logout",
      cancelLabel: "Cancel",
    });
    if (confirmed) {
      await authService.logout();
      checkAuth();
      feedbackService.showToast("Logged out successfully", "info");
      navigate("/", { replace: true });
    }
  };

  const navLinks = isAdmin
    ? NAV_ITEMS.admin
    : isStudent
      ? NAV_ITEMS.student
      : [];

  return (
    <header className={`navbar${scrolled ? " navbar-scrolled" : ""}`}>
      <div className="navbar-inner">
        <div className="navbar-left">
          <Link
            to="/"
            className="navbar-logo"
            onClick={() => setMenuOpen(false)}
          >
            <img
              src={logo}
              alt={NAVBAR_LABELS.LOGO_ALT}
              className="navbar-logo-img"
            />
          </Link>
          <div className="theme-toggle-header-mobile">
            <LapThemeToggle />
          </div>
          <button
            className={`navbar-hamburger${menuOpen ? " navbar-hamburger-open" : ""}`}
            onClick={() => setMenuOpen(!menuOpen)}
            aria-label={NAVBAR_LABELS.TOGGLE_MENU}
          >
            <span />
            <span />
            <span />
          </button>
          <nav
            className={`navbar-links${menuOpen ? " navbar-links-open" : ""}`}
          >
            {navLinks.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                end={false}
                className={({ isActive }) =>
                  `navbar-link${isActive ? " navbar-link-active" : ""}`
                }
                onClick={() => setMenuOpen(false)}
              >
                <Typography variant="caption" component="span">{link.label}</Typography>
              </NavLink>
            ))}
            {isAuthenticated ? (
              <LapButton
                type="primary"
                htmlType="button"
                className="navbar-logout-mobile"
                onClick={handleLogout}
              >
                {NAVBAR_LABELS.LOGOUT}
              </LapButton>
            ) : (
              <Link
                to={ROUTES.LOGIN}
                className="navbar-signin navbar-signin-mobile"
                onClick={() => setMenuOpen(false)}
              >
                <Typography variant="caption" component="span">{NAVBAR_LABELS.SIGN_IN}</Typography>
              </Link>
            )}
          </nav>
        </div>
        <div className="navbar-right">
          <div className="theme-toggle-desktop">
            <LapThemeToggle />
          </div>
          {isAuthenticated ? (
            <LapButton type="primary" htmlType="button" className="navbar-logout-desktop" onClick={handleLogout}>
              {NAVBAR_LABELS.LOGOUT}
            </LapButton>
          ) : (
            <Link to={ROUTES.LOGIN} className="navbar-signin">
              <Typography variant="caption" component="span">{NAVBAR_LABELS.SIGN_IN}</Typography>
            </Link>
          )}
        </div>
      </div>
    </header>
  );
}
