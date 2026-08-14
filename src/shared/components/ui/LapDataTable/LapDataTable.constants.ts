export const SORT_DIRECTIONS = { ASC: "asc", DESC: "desc" } as const;

export const DEFAULT_PAGE_SIZE = 10;

export const PAGE_SIZE_OPTIONS = [5, 10, 25];

export const EMPTY_CELL_FALLBACK = "\u2014";

export const ALIGNMENT_CLASSES = { CENTER: "cm-cell-center", RIGHT: "cm-cell-right" } as const;

export const DATA_TABLE = {
  EMPTY_MESSAGE: "No data available",
  SEARCH_PLACEHOLDER: "Search...",
  FILTER_LABEL: "Filter",
  DISPLAYED_ROWS: (from: number, to: number, count: number) =>
    `Showing ${from}–${to} of ${count}`,
} as const;
