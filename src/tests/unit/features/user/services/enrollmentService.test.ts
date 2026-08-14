const mockGetApiV1Enrollment = jest.fn();
const mockPostEnrollment = jest.fn();

jest.mock("@/shared/services/api/config/axios", () => ({}));

jest.mock(
  "@/shared/services/api/services/enrollment/enrollment",
  () => ({
    getEnrollment: () => ({
      getApiV1Enrollment: mockGetApiV1Enrollment,
    }),
  }),
);

jest.mock(
  "@/shared/services/api/services/course/course",
  () => ({
    getCourse: () => ({
      postApiV1CourseCourseIdEnrollment: mockPostEnrollment,
    }),
  }),
);

import {
  getEnrollments,
  enrollInCourse,
} from "@/features/user/services/enrollmentService";

describe("enrollmentService", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe("getEnrollments", () => {
    it("returns mapped enrollments from API response", async () => {
      mockGetApiV1Enrollment.mockResolvedValue({
        data: {
          data: [
            {
              id: "enr-1",
              course_id: "course-1",
              course_title: "React Basics",
              course_category: { name: "Frontend" },
              enrolled_on: "2025-01-01",
              completed_on: null,
              progress_percentage: 50,
              enrollment_status: true,
              thumbnail_img: "thumb.jpg",
            },
          ],
          total: 1,
          page: 1,
          page_size: 10,
        },
      });

      const result = await getEnrollments({ pageSize: 10 });
      expect(result).toEqual({
        courses: [
          {
            id: "enr-1",
            courseId: "course-1",
            title: "React Basics",
            category: "Frontend",
            enrolledOn: "2025-01-01",
            completedOn: null,
            progress: 50,
            status: true,
            thumbnail: "thumb.jpg",
          },
        ],
        total: 1,
        page: 1,
        pageSize: 10,
      });
    });

    it("handles empty response data", async () => {
      mockGetApiV1Enrollment.mockResolvedValue({ data: {} });
      const result = await getEnrollments();
      expect(result.courses).toEqual([]);
      expect(result.total).toBe(0);
    });

    it("propagates API errors", async () => {
      mockGetApiV1Enrollment.mockRejectedValue(new Error("Network error"));
      await expect(getEnrollments()).rejects.toThrow("Network error");
    });
  });

  describe("enrollInCourse", () => {
    it("calls the enrollment API with the course id", async () => {
      mockPostEnrollment.mockResolvedValue({});
      await enrollInCourse("course-42");
      expect(mockPostEnrollment).toHaveBeenCalledWith("course-42");
    });

    it("propagates API errors", async () => {
      mockPostEnrollment.mockRejectedValue(new Error("Enrollment failed"));
      await expect(enrollInCourse("bad-id")).rejects.toThrow(
        "Enrollment failed",
      );
    });
  });
});
