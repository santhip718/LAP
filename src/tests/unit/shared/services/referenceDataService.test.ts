import { referenceDataService } from "@/shared/services/referenceData/referenceDataService";
import { getReferenceData } from "@/shared/services/api/services/reference-data/reference-data";
import { cacheService } from "@/shared/services/cache/cacheService";

jest.mock("@/shared/services/api/services/reference-data/reference-data", () => {
  const api = {
    getApiV1ReferenceDataRefSetName: jest.fn(),
  };
  return { getReferenceData: jest.fn(() => api) };
});

const mockApi = (getReferenceData as jest.Mock)() as {
  getApiV1ReferenceDataRefSetName: jest.Mock;
};

beforeEach(() => {
  jest.clearAllMocks();
  cacheService.clear();
});

describe("shared referenceDataService.getDesignations", () => {
  it("returns designation list from API", async () => {
    const designations = [{ id: "des-1", name: "Professor" }];
    mockApi.getApiV1ReferenceDataRefSetName.mockResolvedValue({ data: designations });
    const result = await referenceDataService.getDesignations();
    expect(mockApi.getApiV1ReferenceDataRefSetName).toHaveBeenCalledWith("designation");
    expect(result).toEqual(designations);
  });

  it("returns empty array when response data is not an array", async () => {
    mockApi.getApiV1ReferenceDataRefSetName.mockResolvedValue({ data: null });
    const result = await referenceDataService.getDesignations();
    expect(result).toEqual([]);
  });

  it("re-throws API errors", async () => {
    mockApi.getApiV1ReferenceDataRefSetName.mockRejectedValue(new Error("API error"));
    await expect(referenceDataService.getDesignations()).rejects.toThrow("API error");
  });
});

describe("shared referenceDataService.getGenders", () => {
  it("returns gender list from API", async () => {
    const genders = [{ id: "gen-1", name: "Male" }];
    mockApi.getApiV1ReferenceDataRefSetName.mockResolvedValue({ data: genders });
    const result = await referenceDataService.getGenders();
    expect(mockApi.getApiV1ReferenceDataRefSetName).toHaveBeenCalledWith("gender");
    expect(result).toEqual(genders);
  });

  it("returns empty array when response data is not an array", async () => {
    mockApi.getApiV1ReferenceDataRefSetName.mockResolvedValue({ data: {} });
    const result = await referenceDataService.getGenders();
    expect(result).toEqual([]);
  });

  it("re-throws API errors", async () => {
    mockApi.getApiV1ReferenceDataRefSetName.mockRejectedValue(new Error("Network error"));
    await expect(referenceDataService.getGenders()).rejects.toThrow("Network error");
  });
});
