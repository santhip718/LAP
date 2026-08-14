import { useState, useEffect, useCallback } from "react";
import { useDebounce } from "@/shared/hooks/useDebounce";
import { getActiveCategories } from "../../services/courseService";
import {
  referenceDataService,
  type RefTerm,
} from "@/shared/services/referenceData";
import type { FilterBarProps } from "../../types/courseService.types";
import {
  DEBOUNCE_DELAY_MS,
  SEARCH_PLACEHOLDER,
  SELECT_LABELS,
  CLEAR_LABEL,
  ICONS,
} from "./FilterBar.constants";
import "./FilterBar.css";

export default function FilterBar({ onFilterChange }: FilterBarProps) {
  const [searchInput, setSearchInput] = useState("");
  const debouncedSearch = useDebounce(searchInput, DEBOUNCE_DELAY_MS);
  const [categoryId, setCategoryId] = useState("");
  const [difficultyLevelId, setDifficultyLevelId] = useState("");
  const [categories, setCategories] = useState<RefTerm[]>([]);
  const [difficultyLevels, setDifficultyLevels] = useState<RefTerm[]>([]);

  useEffect(() => {
    getActiveCategories().then(setCategories).catch(() => {});
    referenceDataService
      .getDifficultyLevels()
      .then(setDifficultyLevels)
      .catch(() => {});
  }, []);

  useEffect(() => {
    onFilterChange({
      search: debouncedSearch || undefined,
      categoryId: categoryId || undefined,
      difficultyLevelId: difficultyLevelId || undefined,
    });
  }, [debouncedSearch, categoryId, difficultyLevelId, onFilterChange]);

  const handleCategoryChange = useCallback(
    (e: React.ChangeEvent<HTMLSelectElement>) => {
      setCategoryId(e.target.value);
    },
    [],
  );

  const handleDifficultyChange = useCallback(
    (e: React.ChangeEvent<HTMLSelectElement>) => {
      setDifficultyLevelId(e.target.value);
    },
    [],
  );

  const handleClear = useCallback(() => {
    setSearchInput("");
    setCategoryId("");
    setDifficultyLevelId("");
  }, []);

  const hasActiveFilters = debouncedSearch || categoryId || difficultyLevelId;

  return (
    <div className="filter-bar">
      <div className="filter-bar-search">
        <span className="material-symbols-outlined filter-bar-search-icon">
          {ICONS.SEARCH}
        </span>
        <input
          type="text"
          className="filter-bar-input"
          placeholder={SEARCH_PLACEHOLDER}
          value={searchInput}
          onChange={(e) => setSearchInput(e.target.value)}
        />
      </div>

      <div className="filter-bar-selects">
        <select
          className="filter-bar-select"
          value={categoryId}
          onChange={handleCategoryChange}
        >
          <option value="">{SELECT_LABELS.ALL_CATEGORIES}</option>
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>
              {cat.name}
            </option>
          ))}
        </select>

        <select
          className="filter-bar-select"
          value={difficultyLevelId}
          onChange={handleDifficultyChange}
        >
          <option value="">{SELECT_LABELS.ALL_LEVELS}</option>
          {difficultyLevels.map((level) => (
            <option key={level.id} value={level.id}>
              {level.name}
            </option>
          ))}
        </select>

        {hasActiveFilters && (
          <button
            className="filter-bar-clear"
            onClick={handleClear}
            type="button"
          >
            {CLEAR_LABEL}
          </button>
        )}
      </div>
    </div>
  );
}
