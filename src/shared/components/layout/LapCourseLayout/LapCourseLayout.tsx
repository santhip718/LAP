import "./LapCourseLayout.css";
import type { CourseLayoutProps } from "@/shared/types/layout.types";

export default function LapCourseLayout({
  sidebar,
  children,
  isSidebarCollapsed,
  isMobileOpen,
  onMobileToggle,
  sidebarWidth = 260,
}: CourseLayoutProps) {
  return (
    <div
      className={["cl-layout", isSidebarCollapsed ? "cl-layout--collapsed" : ""]
        .filter(Boolean)
        .join(" ")}
      style={{ "--sidebar-width": `${sidebarWidth}px` } as React.CSSProperties}
    >
      <div className="cl-sidebar-col">{sidebar}</div>

      <main className="cl-main">
        <button
          className={[
            "cl-mobile-toggle",
            isMobileOpen ? "cl-mobile-toggle--hidden" : "",
          ]
            .filter(Boolean)
            .join(" ")}
          onClick={onMobileToggle}
          aria-label="Open sidebar"
        >
          <span className="material-symbols-outlined">
            {isMobileOpen ? "close" : "menu"}
          </span>
        </button>
        {children}
      </main>
    </div>
  );
}
