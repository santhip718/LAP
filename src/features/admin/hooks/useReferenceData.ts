import { useState, useEffect } from "react";
import { referenceDataService } from "../../../shared/services/referenceDataService";
import type { RefTerm } from "../../../shared/services/referenceDataService";
import { courseServiceStrings } from "../utils/constants";

export function useReferenceData() {
  const [categories, setCategories] = useState<RefTerm[]>([]);
  const [subcategories, setSubcategories] = useState<RefTerm[]>([]);
  const [difficultyLevels, setDifficultyLevels] = useState<RefTerm[]>([]);
  const [contentTypes, setContentTypes] = useState<RefTerm[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    setError(null);

    Promise.all([
      referenceDataService.getCategories(),
      referenceDataService.getSubcategories(),
      referenceDataService.getDifficultyLevels(),
      referenceDataService.getContentTypes(),
    ])
      .then(([cats, subs, diffs, types]) => {
        if (cancelled) return;
        setCategories(cats);
        setSubcategories(subs);
        setDifficultyLevels(diffs);
        setContentTypes(types);
        setLoading(false);
      })
      .catch((err) => {
        if (cancelled) return;
        setError(courseServiceStrings.error.loadReferenceDataFailed);
        setLoading(false);
        console.error("Error loading reference data:", err);
      });

    return () => {
      cancelled = true;
    };
  }, []);

  return { categories, subcategories, difficultyLevels, contentTypes, loading, error };
}
