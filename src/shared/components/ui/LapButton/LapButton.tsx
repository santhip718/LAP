import type { ReactNode } from "react";
import Typography from "@mui/material/Typography";
import type { LapButtonType, LapButtonProps } from "@/shared/types/ui.types";
import "./LapButton.css";

export default function LapButton({
  type: styleType = "primary",
  htmlType = "button",
  loading = false,
  icon,
  children,
  disabled,
  className,
  fullWidth,
  ...rest
}: LapButtonProps) {
  const classNames = [
    "btn",
    `btn--${styleType}`,
    loading ? "btn--loading" : "",
    fullWidth ? "btn--full-width" : "",
    className ?? "",
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <button
      className={classNames}
      type={htmlType}
      disabled={disabled || loading}
      {...rest}
    >
      {loading ? (
        <span className="btn__spinner" />
      ) : (
        icon && <span className="btn__icon">{icon}</span>
      )}
      {children && (
        <Typography variant="body1" component="span" className="btn__text">
          {children}
        </Typography>
      )}
    </button>
  );
}
