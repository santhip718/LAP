import { Outlet } from "react-router-dom";
import type { ReactNode } from "react";
import LapNavbar from "@/shared/components/layout/LapNavbar/LapNavbar";
import "./LapLayout.css";

interface LapLayoutProps {
  children?: ReactNode;
}

export default function LapLayout({ children }: LapLayoutProps) {
  return (
    <div className="layout">
      <LapNavbar />
      <main className="layout-main">
        {children ?? <Outlet />}
      </main>
    </div>
  )
}
