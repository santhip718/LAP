const mockPostReview = jest.fn();
const mockGetReviews = jest.fn();
const mockGetUserReview = jest.fn();
const mockPutReview = jest.fn();
const mockDeleteReview = jest.fn();

jest.mock("@/shared/services/api/config/axios", () => ({}));

jest.mock(
  "@/shared/services/api/services/review/review",
  () => ({
    getReview: () => ({
      postApiV1ReviewCourseCourseId: mockPostReview,
      getApiV1ReviewCourseCourseId: mockGetReviews,
      getApiV1ReviewCourseCourseIdUserUserIdReview: mockGetUserReview,
      putApiV1ReviewId: mockPutReview,
      deleteApiV1ReviewId: mockDeleteReview,
    }),
  }),
);

import {
  submitReview,
  getCourseReviews,
  getUserReviewForCourse,
  updateReview,
  deleteReview,
} from "@/features/user/services/reviewService";

const sampleReview = {
  id: "r-1",
  rating: 5,
  review_text: "Great course!",
  user_full_name: "John",
  created_on: "2025-01-01",
};

describe("reviewService", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe("submitReview", () => {
    it("sends create request and returns review", async () => {
      mockPostReview.mockResolvedValue({ data: sampleReview });

      const result = await submitReview("c-1", {
        rating: 5,
        reviewText: "Great course!",
      });
      expect(result).toEqual(sampleReview);
      expect(mockPostReview).toHaveBeenCalledWith("c-1", {
        rating: 5,
        review_text: "Great course!",
      });
    });

    it("propagates API errors", async () => {
      mockPostReview.mockRejectedValue(new Error("Submit failed"));
      await expect(
        submitReview("c-1", { rating: 1, reviewText: "Bad" }),
      ).rejects.toThrow("Submit failed");
    });
  });

  describe("getCourseReviews", () => {
    it("passes page and pageSize as params", async () => {
      mockGetReviews.mockResolvedValue({ data: [] });
      await getCourseReviews("c-1", 2, 10);
      expect(mockGetReviews).toHaveBeenCalledWith("c-1", { page: 2, pageSize: 10 });
    });

    it("returns array response directly", async () => {
      mockGetReviews.mockResolvedValue({ data: [sampleReview] });
      const result = await getCourseReviews("c-1");
      expect(result).toHaveLength(1);
    });

    it("extracts data from paginated response", async () => {
      mockGetReviews.mockResolvedValue({
        data: { data: [sampleReview] },
      });
      const result = await getCourseReviews("c-1");
      expect(result).toHaveLength(1);
    });

    it("returns empty on unknown shape", async () => {
      mockGetReviews.mockResolvedValue({ data: { foo: "bar" } });
      const result = await getCourseReviews("c-1");
      expect(result).toEqual([]);
    });
  });

  describe("getUserReviewForCourse", () => {
    it("returns review for the user", async () => {
      mockGetUserReview.mockResolvedValue({ data: sampleReview });
      const result = await getUserReviewForCourse("c-1", "u-1");
      expect(result.id).toBe("r-1");
    });
  });

  describe("updateReview", () => {
    it("sends update request", async () => {
      mockPutReview.mockResolvedValue({ data: sampleReview });
      const result = await updateReview("r-1", { rating: 4 });
      expect(result.id).toBe("r-1");
      expect(mockPutReview).toHaveBeenCalledWith("r-1", {
        rating: 4,
        review_text: null,
      });
    });
  });

  describe("deleteReview", () => {
    it("sends delete request", async () => {
      mockDeleteReview.mockResolvedValue({});
      await deleteReview("r-1");
      expect(mockDeleteReview).toHaveBeenCalledWith("r-1");
    });
  });
});
