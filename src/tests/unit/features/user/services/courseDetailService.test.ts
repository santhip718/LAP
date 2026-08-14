const mockGetApiV1CourseIdOverview = jest.fn();
const mockGetApiV1CourseIdProgress = jest.fn();

jest.mock("@/shared/services/api/config/axios", () => ({}));

jest.mock(
  "@/shared/services/api/services/course/course",
  () => ({
    getCourse: () => ({
      getApiV1CourseIdOverview: mockGetApiV1CourseIdOverview,
      getApiV1CourseIdProgress: mockGetApiV1CourseIdProgress,
    }),
  }),
);

import {
  getCourseOverview,
  getCourseProgress,
} from "@/features/user/services/courseDetailService";

const overviewDto = {
  id: "c-1",
  title: "React Basics",
  category: { id: "cat-1", name: "Frontend" },
  difficulty_level: { id: "diff-1", name: "Beginner" },
  duration_minute: 120,
  overall_rating: 4.5,
  thumbnail_img: "thumb.jpg",
  is_drafted: false,
  description: "Learn React",
  created_by_user: {
    id: "u-1",
    full_name: "John",
    email: "john@test.com",
    roles: ["Instructor"],
  },
  topic: [
    {
      id: "t-1",
      name: "Getting Started",
      sequence_order: 1,
      duration_minute: 30,
      contents: [
        {
          id: "c-1",
          title: "Intro Video",
          content_type: { id: "ct-1", name: "Video" },
          video_url: "https://example.com/video",
          meta_duration_minute: 10,
          sequence_order: 1,
        },
      ],
    },
  ],
  enrollment_count: 100,
  assessment_title: "Final Quiz",
  total_mark: 100,
  passing_mark: 70,
};

describe("courseDetailService", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe("getCourseOverview", () => {
    it("maps full course overview correctly", async () => {
      mockGetApiV1CourseIdOverview.mockResolvedValue({ data: overviewDto });

      const result = await getCourseOverview("c-1");
      expect(result.id).toBe("c-1");
      expect(result.title).toBe("React Basics");
      expect(result.topics).toHaveLength(1);
      expect(result.topics[0].contents).toHaveLength(1);
      expect(result.status).toBe(true);
    });

    it("marks is_drafted as false status", async () => {
      mockGetApiV1CourseIdOverview.mockResolvedValue({
        data: { ...overviewDto, is_drafted: true },
      });

      const result = await getCourseOverview("c-1");
      expect(result.status).toBe(false);
    });

    it("throws on missing created_by_user (fail-fast)", async () => {
      mockGetApiV1CourseIdOverview.mockResolvedValue({ data: {} });

      await expect(getCourseOverview("c-1")).rejects.toThrow();
    });

    it("propagates API errors", async () => {
      mockGetApiV1CourseIdOverview.mockRejectedValue(
        new Error("Course not found"),
      );
      await expect(getCourseOverview("bad-id")).rejects.toThrow(
        "Course not found",
      );
    });
  });

  describe("getCourseProgress", () => {
    it("returns progress percentage", async () => {
      mockGetApiV1CourseIdProgress.mockResolvedValue({
        data: { progress_percentage: 75 },
      });

      const result = await getCourseProgress("c-1");
      expect(result).toBe(75);
    });

    it("returns 0 on error (swallowed)", async () => {
      mockGetApiV1CourseIdProgress.mockRejectedValue(new Error("fail"));
      const result = await getCourseProgress("c-1");
      expect(result).toBe(0);
    });
  });
});
