import type { ReactNode } from "react";

export interface CourseLayoutProps {
  sidebar: ReactNode;
  children: ReactNode;
  isSidebarCollapsed: boolean;
  isMobileOpen: boolean;
  onMobileToggle: () => void;
  sidebarWidth?: number;
}
