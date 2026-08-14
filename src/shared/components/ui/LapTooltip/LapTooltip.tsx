import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import type { LapTooltipProps } from "@/shared/types/ui.types";

export default function LapTooltip({
  text,
  maxLines = 1,
  className,
  ...rest
}: LapTooltipProps) {
  const displayText = text.charAt(0).toUpperCase() + text.slice(1);

  return (
    <Tooltip title={text} arrow>
      <Typography
        className={className}
        sx={{
          display: "-webkit-box",
          WebkitLineClamp: maxLines,
          WebkitBoxOrient: "vertical",
          overflow: "hidden",
          textOverflow: "ellipsis",
        }}
        {...rest}
      >
        {displayText}
      </Typography>
    </Tooltip>
  );
}
