import { useEffect, type ReactNode } from "react";
import Typography from "@mui/material/Typography";
import type { LapSidebarProps } from "@/shared/types/ui.types";
import "./LapSidebar.css";
 
export default function LapSidebar({
  course,
  onToggleCollapse,
  isCollapsed,
  isMobileOpen,
  onMobileClose,
  children,
}: LapSidebarProps) {
  useEffect(() => {
    if (isMobileOpen) {
      document.body.style.overflow = "hidden";
    } else {
      document.body.style.overflow = "";
    }
    return () => {
      document.body.style.overflow = "";
    };
  }, [isMobileOpen]);
 
  return (
    <>
      {isMobileOpen && (
        <div className="co-sidebar-backdrop" onClick={onMobileClose} />
      )}
 
      <aside
        className={[
          "co-sidebar",
          isCollapsed ? "co-sidebar--collapsed" : "",
          isMobileOpen ? "co-sidebar--mobile-open" : "",
        ]
          .filter(Boolean)
          .join(" ")}
      >
        <div className="co-sidebar-mobile-header">
          <div className="co-sidebar-icon">
            <span className="material-symbols-outlined">school</span>
          </div>
          <Typography variant="h6" className="co-sidebar-title">
            Course Syllabus
          </Typography>
          <button
            className="co-sidebar-close"
            onClick={onMobileClose}
            title="Close sidebar"
          >
            <span className="material-symbols-outlined">close</span>
          </button>
        </div>
 
        <div className="side-header">
          <div className="co-sidebar-toggle-row">
            <button
              className="co-sidebar-toggle"
              onClick={onToggleCollapse}
              title={isCollapsed ? "Expand sidebar" : "Collapse sidebar"}
            >
              <span className="material-symbols-outlined">
                {isCollapsed ? "menu" : "menu_open"}
              </span>
            </button>
          </div>
 
          <div className="co-sidebar-header">
            <div className="co-sidebar-icon">
              <span className="material-symbols-outlined">school</span>
            </div>
            <div className="co-sidebar-header-text">
              <Typography variant="body1" className="co-sidebar-title">
                Course Syllabus
              </Typography>
              <Typography variant="caption" className="co-sidebar-subtitle">
                {typeof course?.category === "string"
                  ? course.category
                  : course?.category?.name || "No Category"}
              </Typography>
            </div>
          </div>
        </div>
 
        {children && <div className="co-sidebar-content">{children}</div>}
      </aside>
    </>
  );
}
 