import type { LapContent } from "@/shared/types/ui.types";
import { CONTENT_TYPES } from "@/features/user/constants/constants";
import Typography from "@mui/material/Typography";
import type { CourseCanvasProps } from "../../types/courseDetailService.types";
import {
  YOUTUBE_PATTERNS,
  YOUTUBE_EMBED_TEMPLATE,
  PLACEHOLDER_LABELS,
  ICONS,
  IFRAME_ALLOW,
} from "./CourseCanvas.constants";
import "./CourseCanvas.css";

function getYoutubeEmbedUrl(url: string): string {
  for (const pattern of YOUTUBE_PATTERNS) {
    const match = url.match(pattern);
    if (match) {
      return `${YOUTUBE_EMBED_TEMPLATE}${match[1]}`;
    }
  }

  return url;
}

function getPdfSrc(content: LapContent): string | undefined {
  if (content.pdfBase64) {
    return content.pdfBase64;
  }
  if (content.pdfFilePath) {
    return content.pdfFilePath;
  }
  return undefined;
}

export default function CourseCanvas({ content }: CourseCanvasProps) {
  if (!content) {
    return (
      <div className="cc-placeholder">
        <span className="material-symbols-outlined cc-placeholder-icon">
          {ICONS.PLAY}
        </span>
        <Typography variant="body1" className="cc-placeholder-text">
          {PLACEHOLDER_LABELS.SELECT}
        </Typography>
      </div>
    );
  }

  const contentType = content.contentType?.name;
  const pdfSrc = getPdfSrc(content);

  return (
    <div className="cc-canvas">
      {contentType === CONTENT_TYPES.VIDEO && content.videoUrl ? (
        <iframe
          className="cc-frame"
          src={getYoutubeEmbedUrl(content.videoUrl)}
          title={content.title}
          allow={IFRAME_ALLOW}
          allowFullScreen
        />
      ) : contentType === CONTENT_TYPES.PDF && pdfSrc ? (
        <iframe className="cc-frame" src={pdfSrc} title={content.title} />
      ) : (
        <div className="cc-placeholder">
          <span className="material-symbols-outlined cc-placeholder-icon">
            {ICONS.DESCRIPTION}
          </span>
          <Typography variant="body1" className="cc-placeholder-text">
            {PLACEHOLDER_LABELS.NO_PREVIEW}
          </Typography>
        </div>
      )}
    </div>
  );
}
