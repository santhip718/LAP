import { renderHook, waitFor, act } from "@testing-library/react";
import { useAssessments } from "@/features/admin/hooks/useAssessments";

const mockGetAssessments = jest.fn();
const mockDeleteAssessment = jest.fn();

jest.mock("@/features/admin/services/adminService", () => ({
  getAssessments: (...args: unknown[]) => mockGetAssessments(...args),
  deleteAssessment: (...args: unknown[]) => mockDeleteAssessment(...args),
}));

const mockItems: Array<{ id: string; title: string; total_mark: number; passing_mark: number; duration_minute: number; description: null; course: undefined }> = [
  { id: "1", title: "Quiz 1", total_mark: 100, passing_mark: 50, duration_minute: 60, description: null, course: undefined },
];

beforeEach(() => {
  jest.clearAllMocks();
});

describe("useAssessments", () => {
  it("starts with isLoading true", () => {
    mockGetAssessments.mockReturnValue(new Promise(() => {}));
    const { result } = renderHook(() => useAssessments());
    expect(result.current.isLoading).toBe(true);
    expect(result.current.items).toEqual([]);
    expect(result.current.error).toBeNull();
  });

  it("fetches assessments and returns data", async () => {
    mockGetAssessments.mockResolvedValue(mockItems);
    const { result } = renderHook(() => useAssessments());
    await waitFor(() => expect(result.current.isLoading).toBe(false));
    expect(result.current.items).toEqual(mockItems);
    expect(result.current.error).toBeNull();
  });

  it("sets error when fetch fails", async () => {
    mockGetAssessments.mockRejectedValue(new Error("API error"));
    const { result } = renderHook(() => useAssessments());
    await waitFor(() => expect(result.current.isLoading).toBe(false));
    expect(result.current.items).toEqual([]);
    expect(result.current.error).toEqual(new Error("API error"));
  });

  it("refetch re-fetches data", async () => {
    mockGetAssessments.mockResolvedValue(mockItems);
    const { result } = renderHook(() => useAssessments());
    await waitFor(() => expect(result.current.isLoading).toBe(false));

    mockGetAssessments.mockResolvedValue([]);
    act(() => result.current.refetch());
    await waitFor(() => expect(result.current.items).toEqual([]));
  });

  it("deleteAssessment calls delete API and sets isDeleting", async () => {
    mockGetAssessments.mockResolvedValue(mockItems);
    mockDeleteAssessment.mockResolvedValue(undefined);
    const { result } = renderHook(() => useAssessments());
    await waitFor(() => expect(result.current.isLoading).toBe(false));

    let deletePromise: Promise<void>;
    act(() => {
      deletePromise = result.current.deleteAssessment("1");
    });
    expect(result.current.isDeleting).toBe(true);
    await act(async () => deletePromise);
    expect(result.current.isDeleting).toBe(false);
    expect(mockDeleteAssessment).toHaveBeenCalledWith("1");
  });
});
