import apiClient from "@/shared/services/api/config/axios";
import { getAssessment } from "@/shared/services/api/services/assessment/assessment";
import { getCourse } from "@/shared/services/api/services/course/course";
import type {
  AssessmentOverviewDto,
  QuestionDto,
  AssessmentHistoryItemDto,
  GetApiV1AssessmentUserUserIdAssessmentHistoryParams,
  AssessmentSubmitRequestDto,
  SubmitAssessmentResponseDto,
} from "@/shared/services/api/models";
import { getCurrentUser } from "@/features/auth/utils/authUtils";
import type {
  AssessmentOverview,
  AssessmentQuestion,
  AssessmentResultDetail,
  AssessmentResult,
} from "../types/assessmentService.types";

import {
  QUESTION_TYPE_MCQ,
  QUESTION_TYPE_TF,
  QUESTION_TYPE_FIB,
} from "@/features/user/constants/constants";

export type { AssessmentOverview, AssessmentQuestion, AssessmentResultDetail };

export function mapAssessmentResult(
  dto: SubmitAssessmentResponseDto,
): AssessmentResult {
  return {
    assessmentHistoryId: dto.assessment_history_id,
    assessmentId: dto.assessment_id,
    courseId: dto.course_id,
    status: dto.status,
    startedOn: dto.started_on,
    completedOn: dto.completed_on,
    durationTakenMinutes: dto.duration_taken_minutes,
    totalQuestion: dto.total_question,
    correctAnswer: dto.correct_answer,
    score: dto.score,
    weightedScore: dto.weighted_score,
    courseMasteryScore: dto.course_mastery_score,
    passed: dto.passed,
    tierAwarded: dto.tier_awarded,
    weakTopics: dto.weak_topic?.map((wt) => ({
      topicName: wt.topic_name ?? undefined,
      averageScore: wt.average_score,
    })),
    answers: dto.answers?.map((ans) => ({
      questionId: ans.question_id,
      isCorrect: ans.is_correct,
      questionText: ans.question_text ?? undefined,
      selectedAnswer: ans.selected_answer ?? undefined,
      obtainedScore: ans.obtained_score,
    })),
  };
}

const assessmentApi = getAssessment(apiClient);
const courseApi = getCourse(apiClient);

export async function getAssessmentOverview(
  courseId: string,
): Promise<AssessmentOverview | null> {
  try {
    const response =
      await courseApi.getApiV1CourseCourseIdAssessmentOverview(courseId);
    const dtos: AssessmentOverviewDto[] = response.data;
    if (dtos.length === 0) return null;
    const dto = dtos[0];
    const courseDto = dto.course;
    return {
      id: dto.id!,
      title: dto.title!,
      description: dto.description!,
      totalMark: dto.total_mark ?? 0,
      passingMark: dto.passing_mark ?? 0,
      durationMinute: dto.duration_minute ?? 0,
      courseId,
      course: courseDto
        ? {
            id: courseDto.id!,
            title: courseDto.title!,
            category: courseDto.category
              ? {
                  id: courseDto.category.id ?? "",
                  name: courseDto.category.name ?? "",
                }
              : null,
            difficultyLevel: courseDto.difficulty_level
              ? {
                  id: courseDto.difficulty_level.id ?? "",
                  name: courseDto.difficulty_level.name ?? "",
                }
              : null,
            durationMinute: courseDto.duration_minute ?? 0,
            overallRating: courseDto.overall_rating ?? 0,
            thumbnailImg:
              ((courseDto as Record<string, unknown>)
                .thumbnail_img as string) ?? "",
            isDrafted: courseDto.is_drafted ?? false,
          }
        : undefined,
    };
  } catch {
    return null;
  }
}

export async function getAssessmentQuestions(
  assessmentId: string,
): Promise<AssessmentQuestion[]> {
  const response =
    await assessmentApi.getApiV1AssessmentIdQuestion(assessmentId);
  const dtos: QuestionDto[] = response.data;
  return dtos.map((dto) => ({
    id: dto.id!,
    assessmentId: dto.assessment_id!,
    metaTopicId: dto.meta_topic_id!,
    questionType: {
      id: dto.question_type?.id ?? "",
      name: dto.question_type?.name ?? "",
    },
    questionText: dto.question_text!,
    optionList: dto.option_list ?? [],
    weight: dto.weight ?? 0,
  }));
}

export async function submitAssessment(
  assessmentId: string,
  answers: { question_id: string; selected_answer: string }[],
  startedOn: string,
): Promise<SubmitAssessmentResponseDto | null> {
  const user = getCurrentUser();
  const body: AssessmentSubmitRequestDto = {
    user_id: user!.id,
    started_on: startedOn,
    answer: answers,
  };
  const response = await assessmentApi.postApiV1AssessmentIdSubmit(
    assessmentId,
    body,
  );
  return response.data;
}

export async function getAssessmentResult(
  assessmentId: string,
): Promise<AssessmentResultDetail | null> {
  try {
    const response =
      await assessmentApi.getApiV1AssessmentIdResult(assessmentId);
    const data = response.data;
    if (!data) return null;
    const lastAttempt = data.attempts!.at(-1)!;
    return {
      assessmentId: data.assessment_id!,
      assessmentTitle: data.assessment_title!,
      passingMark: data.passing_mark ?? 0,
      totalMark: 0,
      score: lastAttempt?.score ?? 0,
      weightedScore: lastAttempt?.weighted_score ?? 0,
      passed: lastAttempt?.passed ?? false,
      correctAnswer: 0,
      totalQuestion: 0,
      completedOn: lastAttempt.attempted_on!,
      durationTakenMinutes: 0,
      answers: [],
      weakTopics: [],
    };
  } catch {
    return null;
  }
}

export async function getAssessmentHistory(
  userId: string,
  params?: GetApiV1AssessmentUserUserIdAssessmentHistoryParams,
): Promise<AssessmentHistoryItemDto[]> {
  try {
    const response =
      await assessmentApi.getApiV1AssessmentUserUserIdAssessmentHistory(
        userId,
        params,
      );
    return response.data.item ?? [];
  } catch {
    return [];
  }
}

export async function getAssessmentAttemptInfo(
  assessmentId: string,
): Promise<{ attemptsUsed: number; maxAttempts: number } | null> {
  try {
    const response =
      await assessmentApi.getApiV1AssessmentIdResult(assessmentId);
    const data = response.data;
    if (!data) return null;
    return {
      attemptsUsed: data.attempts?.length ?? 0,
      maxAttempts: 3,
    };
  } catch {
    return null;
  }
}

export async function getAssessmentAttempts(
  assessmentId: string,
): Promise<AssessmentHistoryItemDto[]> {
  try {
    const response =
      await assessmentApi.getApiV1AssessmentIdResult(assessmentId);
    const data = response.data;
    if (!data?.attempts) return [];
    return data.attempts.map((attempt, index) => ({
      assessment_history_id: attempt.attempt_number?.toString() ?? `${index}`,
      assessment_id: data.assessment_id,
      assessment_title: data.assessment_title,
      attempted_on: attempt.attempted_on,
      score: attempt.score,
      weighted_score: attempt.weighted_score,
      passed: attempt.passed,
    }));
  } catch {
    return [];
  }
}
