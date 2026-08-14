const mockGetApiV1Course = jest.fn();
const mockGetApiV1CourseRecommendation = jest.fn();

jest.mock("@/shared/services/api/config/axios", () => ({}));

jest.mock(
  "@/shared/services/api/services/course/course",
  () => ({
    getCourse: () => ({
      getApiV1Course: mockGetApiV1Course,
      getApiV1CourseRecommendation: mockGetApiV1CourseRecommendation,
    }),
  }),
);

import { getCourses, getRecommendedCourses } from "@/features/user/services/courseService";

const sampleDto = {
  id: "c-1",
  title: "React 101",
  category: { name: "Frontend" },
  duration_minute: 120,
  difficulty_level: { name: "Beginner" },
  overall_rating: 4.5,
  thumbnail_img: "img.jpg",
};

describe("courseService", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe("getCourses", () => {
    it("returns paginated mapped courses", async () => {
      mockGetApiV1Course.mockResolvedValue({
        data: {
          data: [sampleDto],
          total: 1,
          page: 1,
          page_size: 10,
        },
      });

      const result = await getCourses({ pageSize: 10 });
      expect(result.courses).toHaveLength(1);
      expect(result.courses[0].title).toBe("React 101");
      expect(result.total).toBe(1);
      expect(result.courses[0].duration).toBe("2h");
    });

    it("handles empty response", async () => {
      mockGetApiV1Course.mockResolvedValue({ data: {} });
      const result = await getCourses();
      expect(result.courses).toEqual([]);
      expect(result.total).toBe(0);
    });

    it("propagates API errors", async () => {
      mockGetApiV1Course.mockRejectedValue(new Error("API down"));
      await expect(getCourses()).rejects.toThrow("API down");
    });
  });

  describe("getRecommendedCourses", () => {
    it("returns mapped recommended courses", async () => {
      mockGetApiV1CourseRecommendation.mockResolvedValue({
        data: [sampleDto],
      });

      const result = await getRecommendedCourses();
      expect(result).toHaveLength(1);
      expect(result[0].title).toBe("React 101");
    });

    it("handles empty response", async () => {
      mockGetApiV1CourseRecommendation.mockResolvedValue({ data: [] });
      const result = await getRecommendedCourses();
      expect(result).toEqual([]);
    });

    it("propagates API errors", async () => {
      mockGetApiV1CourseRecommendation.mockRejectedValue(
        new Error("No recommendations"),
      );
      await expect(getRecommendedCourses()).rejects.toThrow(
        "No recommendations",
      );
    });
  });
});
