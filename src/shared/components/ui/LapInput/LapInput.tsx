import type { ReactNode } from "react";
import Typography from "@mui/material/Typography";
import type { LapInputType, LapInputProps } from "@/shared/types/ui.types";
import "./LapInput.css";

export default function LapInput({
  type: variant = "default",
  htmlType = "text",
  label,
  error,
  id,
  rightElement,
  ...rest
}: LapInputProps) {
  const inputId = id ?? rest.name;

  const classNames = [
    "input",
    variant !== "default" ? `input--${variant}` : "",
    error ? "input--error" : "",
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <div className={`input-field${variant !== "default" ? ` input-field--${variant}` : ""}`}>
      {label && (
        <label className="input-label" htmlFor={inputId}>
          <Typography variant="caption" component="span">{label}</Typography>
        </label>
      )}
      <div className="input-wrapper">
        <input
          className={classNames}
          type={htmlType}
          id={inputId}
          {...rest}
        />
        {rightElement && (
          <div className="input-right">{rightElement}</div>
        )}
      </div>
      {error && <Typography variant="caption" component="span" className="input-error">{error}</Typography>}
    </div>
  );
}
