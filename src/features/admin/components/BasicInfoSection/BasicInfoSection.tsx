import Typography from "@mui/material/Typography";
import LapInput from "../../../../shared/components/ui/LapInput/LapInput";
import type { BasicInfoSectionProps } from "../../types";
import { basicInfoSectionStrings } from "../../pages/CourseManagement/CourseManagement.constants";
import { ACCEPTED_IMAGE_TYPES } from "../../utils/constants";

export default function BasicInfoSection({
  register,
  errors,
  watch,
  categories,
  subcategories,
  difficultyLevels,
  thumbnailPreview,
}: BasicInfoSectionProps) {
  const thumbnailFileValue = watch("thumbnailFile");

  const getFileName = (val?: File | FileList) => {
    if (!val) return "";
    if (val instanceof FileList && val.length > 0) return val[0].name;
    if (val instanceof File) return val.name;
    return "";
  };

  const fileName = getFileName(thumbnailFileValue);

  return (
    <div className="cc-card">
      <div className="cc-card-header">
        <Typography variant="h5" className="cc-card-title">{basicInfoSectionStrings.cardTitle}</Typography>
        <Typography variant="body2" className="cc-card-subtitle">{basicInfoSectionStrings.cardSubtitle}</Typography>
      </div>

      <div className="cc-form">
        <div className="cc-field">
          <LapInput
            id="title"
            label={`${basicInfoSectionStrings.labels.courseTitle} ${basicInfoSectionStrings.requiredIndicator}`}
            placeholder={basicInfoSectionStrings.placeholders.courseTitle}
            error={errors.title ? String(errors.title.message) : undefined}
            {...register("title", { required: basicInfoSectionStrings.validation.titleRequired })}
          />
        </div>

        <div className="cc-field">
          <label className="cc-label" htmlFor="description">
            {basicInfoSectionStrings.labels.description} <span className="cc-required">{basicInfoSectionStrings.requiredIndicator}</span>
          </label>
          <textarea
            className={`cc-textarea${errors.description ? " cc-input-error" : ""}`}
            id="description"
            placeholder={basicInfoSectionStrings.placeholders.description}
            rows={4}
            {...register("description", { required: basicInfoSectionStrings.validation.descriptionRequired })}
          />
          {errors.description && (
            <span className="cc-error">{String(errors.description.message)}</span>
          )}
        </div>

        <div className="cc-row">
          <div className="cc-field">
            <label className="cc-label" htmlFor="categoryId">
              {basicInfoSectionStrings.labels.category} <span className="cc-required">{basicInfoSectionStrings.requiredIndicator}</span>
            </label>
            <select
              className={`cc-select${errors.categoryId ? " cc-input-error" : ""}`}
              id="categoryId"
              {...register("categoryId", { required: basicInfoSectionStrings.validation.categoryRequired })}
            >
              <option value="">{basicInfoSectionStrings.placeholders.category}</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
            {errors.categoryId && (
              <span className="cc-error">{String(errors.categoryId.message)}</span>
            )}
          </div>

          <div className="cc-field">
            <label className="cc-label" htmlFor="subCategoryId">
              {basicInfoSectionStrings.labels.subcategory}
            </label>
            <select className="cc-select" id="subCategoryId" {...register("subCategoryId")}>
              <option value="">{basicInfoSectionStrings.placeholders.subcategory}</option>
              {subcategories.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="cc-row">
          <div className="cc-field">
            <label className="cc-label" htmlFor="difficultyLevelId">
              {basicInfoSectionStrings.labels.difficultyLevel} <span className="cc-required">{basicInfoSectionStrings.requiredIndicator}</span>
            </label>
            <select
              className={`cc-select${errors.difficultyLevelId ? " cc-input-error" : ""}`}
              id="difficultyLevelId"
              {...register("difficultyLevelId", {
                required: basicInfoSectionStrings.validation.difficultyRequired,
              })}
            >
              <option value="">{basicInfoSectionStrings.placeholders.difficultyLevel}</option>
              {difficultyLevels.map((d) => (
                <option key={d.id} value={d.id}>
                  {d.name}
                </option>
              ))}
            </select>
            {errors.difficultyLevelId && (
              <span className="cc-error">
                {String(errors.difficultyLevelId.message)}
              </span>
            )}
          </div>

          <div className="cc-field">
            <LapInput
              id="durationHours"
              htmlType="number"
              label={`${basicInfoSectionStrings.labels.durationHours} ${basicInfoSectionStrings.requiredIndicator}`}
              placeholder={basicInfoSectionStrings.placeholders.durationHours}
              error={errors.durationHours ? String(errors.durationHours.message) : undefined}
              {...register("durationHours", {
                required: basicInfoSectionStrings.validation.durationRequired,
                min: { value: 1, message: basicInfoSectionStrings.validation.durationMin },
              })}
            />
          </div>
        </div>

        <div className="cc-field">
          <label className="cc-label">{basicInfoSectionStrings.labels.thumbnailImage}</label>
          <div className={`cc-dropzone ${thumbnailPreview ? "cc-dropzone-has-image" : ""} ${errors.thumbnailFile ? "cc-input-error" : ""}`}>
            {thumbnailPreview ? (
              <div className="cc-preview-container">
                <img src={thumbnailPreview} alt="Thumbnail Preview" className="cc-preview-image" />
                <div className="cc-preview-overlay">
                  <span className="material-symbols-outlined">cached</span>
                  <span>{basicInfoSectionStrings.dropzone.replaceText}</span>
                </div>
                {fileName && <div className="cc-preview-filename">{fileName}</div>}
              </div>
            ) : (
              <>
                <span className="material-symbols-outlined cc-dropzone-icon">
                  cloud_upload
                </span>
                <p className="cc-dropzone-text">
                  {basicInfoSectionStrings.dropzone.dragDropText}
                  <span className="cc-dropzone-link">{basicInfoSectionStrings.dropzone.browseLink}</span>
                </p>
                <p className="cc-dropzone-hint">{basicInfoSectionStrings.dropzone.hint}</p>
              </>
            )}
            <input
              type="file"
              className="cc-dropzone-input"
              accept={ACCEPTED_IMAGE_TYPES}
              {...register("thumbnailFile")}
            />
          </div>
          {errors.thumbnailFile && (
            <span className="cc-error">{String(errors.thumbnailFile.message)}</span>
          )}
        </div>
      </div>
    </div>
  );
}
