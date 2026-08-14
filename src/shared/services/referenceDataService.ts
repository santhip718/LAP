import apiClient from "@/shared/services/api/apiClient";
import { getReferenceData } from "@/shared/services/api/services/reference-data/reference-data";
import { REF_DATA_SETS } from "@/shared/constants/referenceData";

const refDataApi = getReferenceData(apiClient);

export interface RefTerm {
  id: string;
  name: string;
}

export const referenceDataService = {
  async getCategories(): Promise<RefTerm[]> {
    const { data } = await refDataApi.getApiV1ReferenceDataRefSetName(REF_DATA_SETS.CATEGORY);
    return Array.isArray(data) ? (data as RefTerm[]) : [];
  },

  async getSubcategories(): Promise<RefTerm[]> {
    const { data } = await refDataApi.getApiV1ReferenceDataRefSetName(REF_DATA_SETS.SUBCATEGORY);
    return Array.isArray(data) ? (data as RefTerm[]) : [];
  },

  async getDifficultyLevels(): Promise<RefTerm[]> {
    const { data } = await refDataApi.getApiV1ReferenceDataRefSetName(REF_DATA_SETS.DIFFICULTY_LEVEL);
    return Array.isArray(data) ? (data as RefTerm[]) : [];
  },

  async getContentTypes(): Promise<RefTerm[]> {
    const { data } = await refDataApi.getApiV1ReferenceDataRefSetName(REF_DATA_SETS.CONTENT_TYPE);
    return Array.isArray(data) ? (data as RefTerm[]) : [];
  },

  async getDesignations(): Promise<RefTerm[]> {
    const { data } = await refDataApi.getApiV1ReferenceDataRefSetName(REF_DATA_SETS.DESIGNATION);
    return Array.isArray(data) ? (data as RefTerm[]) : [];
  },

  async getGenders(): Promise<RefTerm[]> {
    const { data } = await refDataApi.getApiV1ReferenceDataRefSetName(REF_DATA_SETS.GENDER);
    return Array.isArray(data) ? (data as RefTerm[]) : [];
  },
};
