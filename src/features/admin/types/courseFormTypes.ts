export interface CourseTopicFormInput {
  name: string;
  originalName?: string;
  contentTitle: string;
  metaTopicOrder: number;
  metaTopicDuration: number;
  sequenceOrder: number;
  contentTypeId: string;
  videoUrl?: string;
  contentFile?: File | FileList;
  existingPdfUrl?: string;
  contentId?: string;
}

export interface CreateCourseForm {
  title: string;
  description: string;
  categoryId: string;
  subCategoryId: string;
  difficultyLevelId: string;
  durationHours: number;
  thumbnailFile?: File | FileList;
  topics: CourseTopicFormInput[];
}
