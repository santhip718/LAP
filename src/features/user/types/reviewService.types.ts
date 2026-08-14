export interface ReviewData {
  rating: number;
  reviewText: string;
}

export interface ReviewFormValues {
  reviewText: string;
}

export interface ReviewFormProps {
  onSubmit: (data: ReviewData) => Promise<void>;
  initialValues?: ReviewData;
  onCancel?: () => void;
}

export interface RatingsViewProps {
  readonly courseId: string;
  readonly refreshKey?: number;
}
