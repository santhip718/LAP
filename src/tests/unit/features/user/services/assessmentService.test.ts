const mockGetApiV1CourseCourseIdAssessmentOverview = jest.fn();
const mockGetApiV1AssessmentIdQuestion = jest.fn();
const mockPostApiV1AssessmentIdSubmit = jest.fn();
const mockGetApiV1AssessmentUserUserIdAssessmentHistory = jest.fn();

jest.mock("@/shared/services/api/config/axios", () => ({}));

jest.mock(
  "@/shared/services/api/services/assessment/assessment",
  () => ({
    getAssessment: () => ({
      getApiV1AssessmentIdQuestion: mockGetApiV1AssessmentIdQuestion,
      postApiV1AssessmentIdSubmit: mockPostApiV1AssessmentIdSubmit,
      getApiV1AssessmentUserUserIdAssessmentHistory:
        mockGetApiV1AssessmentUserUserIdAssessmentHistory,
    }),
  }),
);

jest.mock(
  "@/shared/services/api/services/course/course",
  () => ({
    getCourse: () => ({
      getApiV1CourseCourseIdAssessmentOverview:
        mockGetApiV1CourseCourseIdAssessmentOverview,
    }),
  }),
);

jest.mock("@/features/auth/utils/authUtils", () => ({
  getCurrentUser: jest.fn(),
}));

import {
  getAssessmentOverview,
  getAssessmentQuestions,
  submitAssessment,
  getAssessmentHistory,
} from "@/features/user/services/assessmentService";

describe("assessmentService", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe("getAssessmentOverview", () => {
    it("returns mapped assessment from API", async () => {
      mockGetApiV1CourseCourseIdAssessmentOverview.mockResolvedValue({
        data: [
          {
            id: "asm-1",
            title: "Final Test",
            description: "Test description",
            total_mark: 100,
            passing_mark: 70,
            duration_minute: 60,
            course: {
              id: "c-1",
              title: "Course 1",
              category: { id: "cat-1", name: "Math" },
              difficulty_level: { id: "diff-1", name: "Beginner" },
              duration_minute: 120,
              overall_rating: 4.5,
              thumbnail_img: "",
              is_drafted: false,
            },
          },
        ],
      });

      const result = await getAssessmentOverview("c-1");
      expect(result).not.toBeNull();
      expect(result!.id).toBe("asm-1");
      expect(result!.title).toBe("Final Test");
    });

    it("returns null when API returns empty array", async () => {
      mockGetApiV1CourseCourseIdAssessmentOverview.mockResolvedValue({
        data: [],
      });

      const result = await getAssessmentOverview("c-1");
      expect(result).toBeNull();
    });


  });

  describe("getAssessmentQuestions", () => {
    it("returns mapped questions", async () => {
      mockGetApiV1AssessmentIdQuestion.mockResolvedValue({
        data: [
          {
            id: "q-1",
            assessment_id: "asm-1",
            meta_topic_id: "t-1",
            question_type: { id: "qt-1", name: "MCQ" },
            question_text: "What is 2+2?",
            option_list: ["1", "2", "3", "4"],
            weight: 5,
          },
        ],
      });

      const result = await getAssessmentQuestions("asm-1");
      expect(result).toHaveLength(1);
      expect(result[0].questionText).toBe("What is 2+2?");
    });


  });

  describe("submitAssessment", () => {
    it("submits answers and returns result", async () => {
      mockPostApiV1AssessmentIdSubmit.mockResolvedValue({
        data: { status: "passed", score: 80 },
      });

      const { getCurrentUser } = jest.requireMock(
        "@/features/auth/utils/authUtils",
      );
      getCurrentUser.mockReturnValue({ id: "u-1", name: "", email: "" });

      const result = await submitAssessment(
        "asm-1",
        [{ question_id: "q-1", selected_answer: "4" }],
        "2025-01-01T00:00:00Z",
      );
      expect(result).toEqual({ status: "passed", score: 80 });
    });

    it("propagates errors", async () => {
      mockPostApiV1AssessmentIdSubmit.mockRejectedValue(
        new Error("Submit failed"),
      );
      await expect(
        submitAssessment("asm-1", [], ""),
      ).rejects.toThrow("Submit failed");
    });
  });

  describe("getAssessmentHistory", () => {
    it("returns items from response", async () => {
      mockGetApiV1AssessmentUserUserIdAssessmentHistory.mockResolvedValue({
        data: {
          item: [
            {
              assessment_history_id: "ah-1",
              assessment_id: "asm-1",
              assessment_title: "Final Test",
              course_id: "c-1",
              course_title: "Course 1",
              score: 80,
              passed: true,
              attempted_on: "2025-01-01",
            },
          ],
        },
      });

      const result = await getAssessmentHistory("u-1", {
        pageNumber: 1,
        pageSize: 10,
      });
      expect(result).toHaveLength(1);
      expect(result[0].assessment_title).toBe("Final Test");
    });

    it("returns empty on error", async () => {
      mockGetApiV1AssessmentUserUserIdAssessmentHistory.mockRejectedValue(
        new Error("fail"),
      );
      const result = await getAssessmentHistory("u-1", {});
      expect(result).toEqual([]);
    });
  });
});
