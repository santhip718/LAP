import Typography from "@mui/material/Typography";
import LapInput from "../../../../shared/components/ui/LapInput/LapInput";
import LapButton from "../../../../shared/components/ui/LapButton/LapButton";
import type { ContentSectionProps } from "../../types";
import { contentSectionStrings } from "../../pages/CourseManagement/CourseManagement.constants";
import { CONTENT_TYPE_VIDEO, CONTENT_TYPE_PDF, MIME_TYPE_PDF, ACCEPTED_IMAGE_TYPES } from "../../utils/constants";

export default function ContentSection({
  register,
  errors,
  watch,
  contentTypes,
  fields,
  onAddTopic,
  onRemoveTopic,
}: ContentSectionProps) {
  const topicErrors = errors.topics;

  const getFileName = (val?: File | FileList | string) => {
    if (!val) return "";
    if (val instanceof FileList && val.length > 0) return val[0].name;
    if (val instanceof File) return val.name;
    if (typeof val === "string") return val;
    return "";
  };

  return (
    <div className="cc-card">
      <div className="cc-card-header">
        <Typography variant="h5" className="cc-card-title">{contentSectionStrings.cardTitle}</Typography>
        <Typography variant="body2" className="cc-card-subtitle">
          {contentSectionStrings.cardSubtitle}
        </Typography>
      </div>

      <div className="cc-topics">
        {fields.map((field, i) => {
          const selectedTypeId = watch(`topics.${i}.contentTypeId`);
          const selectedType = contentTypes.find((ct) => ct.id === selectedTypeId);
          const isVideo = selectedType?.name.toLowerCase() === CONTENT_TYPE_VIDEO;
          const isPdf = selectedType?.name.toLowerCase() === CONTENT_TYPE_PDF;

          const contentFileValue = watch(`topics.${i}.contentFile`);
          const existingPdfUrl = watch(`topics.${i}.existingPdfUrl`);
          const fileName = getFileName(contentFileValue) || (isPdf && existingPdfUrl ? existingPdfUrl.split("/").pop() || existingPdfUrl : "");

          return (
            <div key={field.id} className="cc-topic">
              <div className="cc-topic-header">
                <div className="cc-topic-number">{contentSectionStrings.topicLabel} {i + 1}</div>
                <button
                  type="button"
                  className="cc-topic-remove"
                  aria-label={contentSectionStrings.removeButtonAriaLabel}
                  onClick={() => onRemoveTopic(i)}
                >
                  <span className="material-symbols-outlined">delete</span>
                </button>
              </div>

              <input type="hidden" {...register(`topics.${i}.contentId`)} />

              <div className="cc-field">
                <LapInput
                  id={`topics.${i}.name`}
                  label={`${contentSectionStrings.labels.metaTopicName} *`}
                  placeholder={contentSectionStrings.placeholders.metaTopicName}
                  error={topicErrors?.[i]?.name ? String(topicErrors[i].name.message) : undefined}
                  {...register(`topics.${i}.name`, {
                    required: contentSectionStrings.validation.metaTopicNameRequired,
                  })}
                />
              </div>

              <div className="cc-field">
                <LapInput
                  id={`topics.${i}.contentTitle`}
                  label={`${contentSectionStrings.labels.contentTitle} *`}
                  placeholder={contentSectionStrings.placeholders.contentTitle}
                  error={topicErrors?.[i]?.contentTitle ? String(topicErrors[i].contentTitle.message) : undefined}
                  {...register(`topics.${i}.contentTitle`, {
                    required: contentSectionStrings.validation.contentTitleRequired,
                  })}
                />
              </div>

              <div className="cc-row">
                <div className="cc-field">
                  <LapInput
                    id={`topics.${i}.metaTopicOrder`}
                    htmlType="number"
                    label={`${contentSectionStrings.labels.metaTopicOrder} *`}
                    placeholder={contentSectionStrings.placeholders.metaTopicOrder}
                    error={topicErrors?.[i]?.metaTopicOrder ? String(topicErrors[i].metaTopicOrder.message) : undefined}
                    {...register(`topics.${i}.metaTopicOrder`, {
                      required: contentSectionStrings.validation.metaTopicOrderRequired,
                      min: { value: 1, message: contentSectionStrings.validation.metaTopicOrderMin },
                    })}
                  />
                </div>

                <div className="cc-field">
                  <LapInput
                    id={`topics.${i}.metaTopicDuration`}
                    htmlType="number"
                    label={`${contentSectionStrings.labels.metaTopicDuration} *`}
                    placeholder={contentSectionStrings.placeholders.metaTopicDuration}
                    error={topicErrors?.[i]?.metaTopicDuration ? String(topicErrors[i].metaTopicDuration.message) : undefined}
                    {...register(`topics.${i}.metaTopicDuration`, {
                      required: contentSectionStrings.validation.metaTopicDurationRequired,
                      min: { value: 1, message: contentSectionStrings.validation.metaTopicDurationMin },
                    })}
                  />
                </div>
              </div>

              <div className="cc-row">
                <div className="cc-field">
                  <LapInput
                    id={`topics.${i}.sequenceOrder`}
                    htmlType="number"
                    label={`${contentSectionStrings.labels.sequenceOrder} *`}
                    placeholder={contentSectionStrings.placeholders.sequenceOrder}
                    error={topicErrors?.[i]?.sequenceOrder ? String(topicErrors[i].sequenceOrder.message) : undefined}
                    {...register(`topics.${i}.sequenceOrder`, {
                      required: contentSectionStrings.validation.sequenceOrderRequired,
                      min: { value: 1, message: contentSectionStrings.validation.sequenceOrderMin },
                    })}
                  />
                </div>

                <div className="cc-field">
                  <label className="cc-label" htmlFor={`topics.${i}.contentTypeId`}>
                    {contentSectionStrings.labels.contentType} <span className="cc-required">*</span>
                  </label>
                  <select
                    className={`cc-select${topicErrors?.[i]?.contentTypeId ? " cc-input-error" : ""}`}
                    id={`topics.${i}.contentTypeId`}
                    {...register(`topics.${i}.contentTypeId`, {
                      required: contentSectionStrings.validation.contentTypeRequired,
                    })}
                    value={watch(`topics.${i}.contentTypeId`) || ""}
                  >
                    <option value="">{contentSectionStrings.placeholders.contentType}</option>
                    {contentTypes.map((ct) => (
                      <option key={ct.id} value={ct.id}>
                        {ct.name}
                      </option>
                    ))}
                  </select>
                  {topicErrors?.[i]?.contentTypeId && (
                    <span className="cc-error">
                      {topicErrors[i].contentTypeId.message}
                    </span>
                  )}
                </div>
              </div>

              {isPdf && (
                <div className="cc-field">
                  <label className="cc-label" htmlFor={`topics.${i}.contentFile`}>
                    {contentSectionStrings.labels.uploadFile} <span className="cc-required">*</span>
                  </label>
                  <div
                    className={`cc-topic-dropzone ${
                      fileName ? "cc-topic-dropzone-has-file" : ""
                    } ${topicErrors?.[i]?.contentFile ? "cc-input-error" : ""}`}
                  >
                    <input
                      type="file"
                      className="cc-dropzone-input"
                      id={`topics.${i}.contentFile`}
                      accept={MIME_TYPE_PDF}
                      {...register(`topics.${i}.contentFile`, {
                        validate: (value) => {
                          if (!isPdf) return true;
                          const name = getFileName(value);
                          if (name) return true;
                          const existing = watch(`topics.${i}.existingPdfUrl`);
                          return existing ? true : contentSectionStrings.validation.pdfFileRequired;
                        },
                      })}
                    />
                    <span className="material-symbols-outlined cc-topic-dropzone-icon">
                      {fileName ? contentSectionStrings.dropzone.pdfCheckIcon : contentSectionStrings.dropzone.pdfUploadIcon}
                    </span>
                    <span className="cc-topic-dropzone-text">
                      {fileName ? fileName : contentSectionStrings.dropzone.pdfPlaceholder}
                    </span>
                  </div>
                  {topicErrors?.[i]?.contentFile && (
                    <span className="cc-error">
                      {topicErrors[i].contentFile.message}
                    </span>
                  )}
                </div>
              )}

              {isVideo && (
                <div className="cc-field">
                  <LapInput
                    id={`topics.${i}.videoUrl`}
                    label={`${contentSectionStrings.labels.videoUrl} *`}
                    placeholder={contentSectionStrings.placeholders.videoUrl}
                    error={topicErrors?.[i]?.videoUrl ? String(topicErrors[i].videoUrl.message) : undefined}
                    {...register(`topics.${i}.videoUrl`, {
                      validate: (value) => {
                        if (!isVideo) return true;
                        if (!value) return contentSectionStrings.validation.videoUrlRequired;
                        try {
                          const urlStr = value.includes("://") ? value : `https://${value}`;
                          new URL(urlStr);
                          return true;
                        } catch {
                          return contentSectionStrings.validation.videoUrlInvalid;
                        }
                      },
                    })}
                  />
                </div>
              )}
            </div>
          );
        })}

        <LapButton type="ghost" onClick={onAddTopic} icon={<span className="material-symbols-outlined cc-add-topic-icon">add</span>}>
          {contentSectionStrings.addTopicButton}
        </LapButton>
      </div>
    </div>
  );
}
