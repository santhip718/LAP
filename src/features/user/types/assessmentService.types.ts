import type { AssessmentHistoryItemDto } from "@/shared/services/api/models";

export interface AssessmentOverview {
  id: string;
  title: string;
  description: string;
  totalMark: number;
  passingMark: number;
  durationMinute: number;
  courseId: string;
  course?: {
    id: string;
    title: string;
    category: { id: string; name: string } | null;
    difficultyLevel: { id: string; name: string } | null;
    durationMinute: number;
    overallRating: number;
    thumbnailImg: string;
    isDrafted: boolean;
  };
}

export interface AssessmentQuestion {
  id: string;
  assessmentId: string;
  metaTopicId: string;
  questionType: { id: string; name: string };
  questionText: string;
  optionList: string[];
  weight: number;
}

export interface AssessmentResultDetail {
  assessmentId: string;
  assessmentTitle: string;
  passingMark: number;
  totalMark: number;
  score: number;
  weightedScore: number;
  passed: boolean;
  correctAnswer: number;
  totalQuestion: number;
  completedOn: string;
  durationTakenMinutes: number;
  answers: {
    questionId: string;
    questionText: string;
    selectedAnswer: string;
    isCorrect: boolean;
    obtainedScore: number;
  }[];
  weakTopics: {
    metaTopicId: string;
    topicName: string;
    averageScore: number;
  }[];
}

export interface AssessmentResult {
  assessmentHistoryId?: string;
  assessmentId?: string;
  courseId?: string;
  status?: string | null;
  startedOn?: string;
  completedOn?: string;
  durationTakenMinutes?: number;
  totalQuestion?: number;
  correctAnswer?: number;
  score?: number;
  weightedScore?: number;
  courseMasteryScore?: number;
  passed?: boolean;
  tierAwarded?: string | null;
  weakTopics?: WeakTopic[];
  answers?: AnswerReview[];
}

export interface WeakTopic {
  topicName?: string;
  averageScore?: number;
}

export interface AnswerReview {
  questionId?: string;
  isCorrect?: boolean;
  questionText?: string;
  selectedAnswer?: string;
  obtainedScore?: number;
}

export interface Answers {
  [questionId: string]: string;
}

export interface Flagged {
  [questionId: string]: boolean;
}

export interface AssessmentHistoryCardProps {
  item: AssessmentHistoryItemDto;
  onClick?: (courseId?: string | null, assessmentId?: string | null) => void;
}
