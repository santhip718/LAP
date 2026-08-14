import apiClient from "@/shared/services/api/apiClient";
import { getReferenceData } from "@/shared/services/api/services/reference-data/reference-data";
import { REF_DATA_SETS } from "@/shared/constants/referenceData";
import type { RefTermDto } from "@/shared/services/api/models";
import { cacheService } from "@/shared/services/cache/cacheService";

const refDataApi = getReferenceData(apiClient);

export interface RefTerm {
  id: string;
  name: string;
}

function mapRefTermDto(dto: RefTermDto): RefTerm {
  return {
    id: dto.id ?? "",
    name: dto.name ?? "",
  };
}

async function getRefSet(name: string): Promise<RefTerm[]> {
  const { data } = await refDataApi.getApiV1ReferenceDataRefSetName(name);
  const items = Array.isArray(data) ? data : [];
  return items.map(mapRefTermDto);
}

export async function getDesignations(): Promise<RefTerm[]> {
  const cached = cacheService.get<RefTerm[]>("designations");
  if (cached) return cached;
  const data = await getRefSet(REF_DATA_SETS.DESIGNATION);
  cacheService.set("designations", data);
  return data;
}

export async function getGenders(): Promise<RefTerm[]> {
  const cached = cacheService.get<RefTerm[]>("genders");
  if (cached) return cached;
  const data = await getRefSet(REF_DATA_SETS.GENDER);
  cacheService.set("genders", data);
  return data;
}

export async function getCategories(): Promise<RefTerm[]> {
  return getRefSet(REF_DATA_SETS.CATEGORY);
}

export async function getDifficultyLevels(): Promise<RefTerm[]> {
  return getRefSet(REF_DATA_SETS.DIFFICULTY_LEVEL);
}

export const referenceDataService = {
  getDesignations,
  getGenders,
  getCategories,
  getDifficultyLevels,
};
