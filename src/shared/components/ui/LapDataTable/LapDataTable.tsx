import { useState, useMemo, useCallback } from "react";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import TableSortLabel from "@mui/material/TableSortLabel";
import TablePagination from "@mui/material/TablePagination";
import Paper from "@mui/material/Paper";
import Box from "@mui/material/Box";
import TextField from "@mui/material/TextField";
import InputAdornment from "@mui/material/InputAdornment";
import SearchIcon from "@mui/icons-material/Search";
import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";
import { useInfiniteScroll, useDebounce } from "@/shared/hooks";
import { DATA_TABLE } from "./LapDataTable.constants";
import "./LapDataTable.css";

import type { Column } from "@/shared/types";
export type { Column };

interface DataTableProps<T> {
  columns: Column<T>[];
  data: T[];
  pageSize?: number;
  pageSizeOptions?: number[];
  onRowClick?: (row: T) => void;
  enableInfiniteScroll?: boolean;
  maxHeight?: string | number;
  enableSearch?: boolean;
  searchPlaceholder?: string;
  searchKeys?: (keyof T)[];
  enableFilter?: boolean;
  filterOptions?: { label: string; value: string }[];
  onFilterChange?: (filterValue: string) => T[] | null;
  onLoadMore?: () => void;
  hasMore?: boolean;
}

type SortDirection = "asc" | "desc";

// ─────────────────────────────────────────────────────────────────────────────
// Design tokens — all visual constants live here so changes are one-line edits
// ─────────────────────────────────────────────────────────────────────────────
const DT = {
  // Card
  cardBg:           "var(--surface-container-lowest)",
  cardRadius:       "16px",
  cardShadow:       "var(--card-shadow)",
  cardBorder:       "1px solid var(--outline-variant)",

  // Toolbar (search + filter bar)
  toolbarBorder:    "1px solid var(--outline-variant)",
  toolbarPad:       "10px 20px",

  // Header
  headerBg:         "var(--surface-container-lowest)",
  headerPad:        "8px 20px",
  headerBorder:     "1px solid var(--outline-variant)",
  headerFontSize:   "10px",
  headerFontWeight: 700,
  headerLetterSpacing: "0.8px",
  headerColor:      "var(--on-surface-variant)",

  // Body row
  rowHoverBg:       "var(--surface)",
  rowTransition:    "background 0.18s ease",

  // Body cell
  cellPad:          "10px 20px",
  cellFontSize:     "13px",
  cellFontWeight:   500,
  cellLineHeight:   "18px",
  cellColor:        "var(--on-surface)",
  cellBorder:       "1px solid var(--outline-variant)",

  // Empty state
  emptyPad:         "32px 20px",

  // Pagination
  paginationBg:     "var(--surface-container-lowest)",
  paginationBorder: "1px solid var(--outline-variant)",
  paginationPad:    "0 12px",
} as const;

export default function LapDataTable<T>({
  columns,
  data,
  pageSize = 10,
  pageSizeOptions,
  onRowClick,
  enableInfiniteScroll = false,
  maxHeight,
  enableSearch = false,
  searchPlaceholder = DATA_TABLE.SEARCH_PLACEHOLDER,
  searchKeys = [],
  enableFilter = false,
  filterOptions = [],
  onFilterChange,
  onLoadMore,
  hasMore,
}: DataTableProps<T>) {
  // ── State ──────────────────────────────────────────────────────────────────
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(pageSize);
  const [sortKey, setSortKey] = useState<string | null>(null);
  const [sortDir, setSortDir] = useState<SortDirection>("asc");
  const [searchQuery, setSearchQuery] = useState("");
  const debouncedSearchQuery = useDebounce(searchQuery, 500);
  const [filterValue, setFilterValue] = useState("all");

  // ── Handlers (logic unchanged) ─────────────────────────────────────────────
  const handleSort = (key: string) => {
    if (sortKey === key) {
      setSortDir((d) => (d === "asc" ? "desc" : "asc"));
    } else {
      setSortKey(key);
      setSortDir("asc");
    }
  };

  const handleSearchChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => setSearchQuery(e.target.value),
    [],
  );

  const handleFilterChange = useCallback(
    (e: { target: { value: string } }) => setFilterValue(e.target.value),
    [],
  );

  // ── Data pipeline (logic unchanged) ───────────────────────────────────────
  const filteredData = useMemo(() => {
    let result = Array.isArray(data) ? data : [];
    if (enableFilter && filterValue !== "all" && onFilterChange) {
      const filtered = onFilterChange(filterValue);
      result = filtered || data;
    }
    if (enableSearch && debouncedSearchQuery.trim() && searchKeys.length > 0) {
      const q = debouncedSearchQuery.trim().toLowerCase();
      result = result.filter((row) => {
        const rowData = row as Record<string, unknown>;
        return searchKeys.some((key) => {
          let value = rowData[key as string];
          if (value === null || value === undefined) return false;
          if (typeof value === "object" && "title" in (value as Record<string, unknown>)) {
            value = (value as Record<string, unknown>).title;
          }
          return String(value).toLowerCase().includes(q);
        });
      });
    }
    return result;
  }, [data, debouncedSearchQuery, enableSearch, searchKeys, enableFilter, filterValue, onFilterChange]);

  const sortedData = useMemo(() => {
    if (!sortKey) return filteredData;
    const col = columns.find((c) => c.key === sortKey);
    return [...filteredData].sort((a, b) => {
      let aVal: unknown;
      let bVal: unknown;
      if (col?.sortValue) {
        aVal = col.sortValue(a);
        bVal = col.sortValue(b);
      } else {
        const rowA = a as Record<string, unknown>;
        const rowB = b as Record<string, unknown>;
        aVal = rowA[sortKey];
        bVal = rowB[sortKey];
      }
      if (aVal == null) return 1;
      if (bVal == null) return -1;
      const cmp =
        typeof aVal === "number" && typeof bVal === "number"
          ? aVal - bVal
          : String(aVal).localeCompare(String(bVal));
      return sortDir === "asc" ? cmp : -cmp;
    });
  }, [filteredData, sortKey, sortDir, columns]);

  const { displayedItems = [], sentinelRef, setScrollContainerRef } = useInfiniteScroll({
    items: sortedData,
    pageSize,
    onLoadMore,
    hasMore,
  });

  const paginatedData = (enableInfiniteScroll
    ? displayedItems
    : sortedData.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)) ?? [];

  const handlePageChange = (_: unknown, newPage: number) => setPage(newPage);
  const handleRowsPerPageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(e.target.value, 10));
    setPage(0);
  };

  // ── Render ─────────────────────────────────────────────────────────────────
  return (
    <Paper
      elevation={0}
      sx={{
        borderRadius: DT.cardRadius,
        border: DT.cardBorder,
        boxShadow: DT.cardShadow,
        bgcolor: DT.cardBg,
        overflow: "hidden",
      }}
    >
      {/* ── Toolbar: Search + Filter ─────────────────────────────────────── */}
      {(enableSearch || enableFilter) && (
        <Box
          sx={{
            display: "flex",
            gap: 2,
            padding: { xs: "8px 10px", sm: DT.toolbarPad },
            borderBottom: DT.toolbarBorder,
            bgcolor: DT.cardBg,
            flexWrap: "wrap",
            alignItems: "center",
          }}
        >
          {enableSearch && (
            <TextField
              placeholder={searchPlaceholder}
              variant="outlined"
              size="small"
              value={searchQuery}
              onChange={handleSearchChange}
              slotProps={{
                input: {
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchIcon sx={{ color: "var(--outline)", fontSize: 18 }} />
                    </InputAdornment>
                  ),
                },
              }}
              sx={{
                flex: enableFilter ? 1 : "auto",
                minWidth: { xs: "100%", sm: "220px" },
                width: { xs: "100%", sm: "auto" },
                "& .MuiOutlinedInput-root": {
                  borderRadius: "12px",
                  bgcolor: "var(--surface)",
                  "& fieldset": { borderColor: "var(--outline-variant)" },
                  "&:hover fieldset": { borderColor: "var(--outline)" },
                  "&.Mui-focused fieldset": { borderColor: "var(--secondary)", borderWidth: "1.5px" },
                },
                "& .MuiInputBase-input": {
                  fontSize: "12px",
                  padding: "4px 10px 4px 0",
                  color: "var(--on-surface)",
                  "&::placeholder": { color: "var(--outline)", opacity: 1 },
                },
              }}
            />
          )}
          {enableFilter && (
            <FormControl
              size="small"
              sx={{
                minWidth: 140,
                "& .MuiOutlinedInput-root": {
                  borderRadius: "10px",
                  bgcolor: "var(--surface)",
                  "& fieldset": { borderColor: "var(--outline-variant)" },
                  "&:hover fieldset": { borderColor: "var(--outline)" },
                  "&.Mui-focused fieldset": { borderColor: "var(--secondary)", borderWidth: "1.5px" },
                },
                "& .MuiInputBase-input": { fontSize: "13px", color: "var(--on-surface)" },
              }}
            >
              <InputLabel sx={{ fontSize: "13px", color: "var(--on-surface-variant)" }}>{DATA_TABLE.FILTER_LABEL}</InputLabel>
              <Select value={filterValue} label={DATA_TABLE.FILTER_LABEL} onChange={handleFilterChange}>
                {filterOptions.map((option) => (
                  <MenuItem key={option.value} value={option.value} sx={{ fontSize: "13px" }}>
                    {option.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}
        </Box>
      )}

      {/* ── Table ───────────────────────────────────────────────────────── */}
      <TableContainer
        ref={setScrollContainerRef}
        sx={{
          overflowX: "auto",
          ...(enableInfiniteScroll && {
            maxHeight: maxHeight ?? "400px",
            overflowY: "auto",
          }),
        }}
      >
        <Table
          sx={{
            minWidth: { xs: 0, sm: 640, md: 900 },
            borderCollapse: "separate",
            borderSpacing: 0,
            width: "100%",
          }}
        >
          {/* Header */}
          <TableHead>
            <TableRow>
              {columns.map((col) => (
                <TableCell
                  key={col.key}
                  sortDirection={sortKey === col.key ? sortDir : false}
                  sx={{
                    ...(col.mobileHidden && { display: { xs: "none", sm: "table-cell" } }),
                    bgcolor: DT.headerBg,
                    padding: { xs: "6px 6px", sm: "8px 12px", md: DT.headerPad },
                    borderBottom: DT.headerBorder,
                    fontSize: DT.headerFontSize,
                    fontWeight: DT.headerFontWeight,
                    letterSpacing: DT.headerLetterSpacing,
                    textTransform: "uppercase",
                    color: DT.headerColor,
                    whiteSpace: "nowrap",
                    userSelect: "none",
                    ...(col.thClassName === "cm-cell-center" && {
                      textAlign: "center" as const,
                    }),
                  }}
                >
                  {col.sortable ? (
                    <TableSortLabel
                      active={sortKey === col.key}
                      direction={sortKey === col.key ? sortDir : "asc"}
                      onClick={() => handleSort(col.key)}
                      sx={{
                        color: `${DT.headerColor} !important`,
                        "&.Mui-active": { color: "var(--secondary) !important" },
                        "& .MuiTableSortLabel-icon": {
                          fontSize: 13,
                          opacity: sortKey === col.key ? 1 : 0.35,
                          color: sortKey === col.key ? "var(--secondary) !important" : "inherit",
                        },
                        "&:hover": { color: "var(--on-surface) !important" },
                      }}
                    >
                      {col.label}
                    </TableSortLabel>
                  ) : (
                    col.label
                  )}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>

          {/* Body */}
          <TableBody>
            {paginatedData.length === 0 ? (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  align="center"
                  sx={{
                    padding: DT.emptyPad,
                    color: "var(--outline)",
                    borderBottom: "none",
                  }}
                >
                  <Box
                    sx={{
                      display: "flex",
                      flexDirection: "column",
                      alignItems: "center",
                      gap: 1,
                    }}
                  >
                    <span
                      className="material-symbols-outlined"
                      style={{ fontSize: 40, opacity: 0.3, color: "var(--on-surface)" }}
                    >
                      inbox
                    </span>
                    <p style={{ margin: 0, fontSize: 13, fontWeight: 500, color: "var(--on-surface-variant)" }}>
                      {DATA_TABLE.EMPTY_MESSAGE}
                    </p>
                  </Box>
                </TableCell>
              </TableRow>
            ) : (
              paginatedData.map((row, rowIndex) => (
                <TableRow
                  key={((row as { id?: unknown }).id as string) ?? rowIndex}
                  hover={!!onRowClick}
                  onClick={() => onRowClick?.(row)}
                  sx={{
                    cursor: onRowClick ? "pointer" : "default",
                    transition: DT.rowTransition,
                    "&:last-child td": { borderBottom: "none" },
                    "&:hover": {
                      bgcolor: DT.rowHoverBg,
                      "& td": { color: "var(--on-surface)" },
                    },
                  }}
                >
                  {columns.map((col) => {
                    const rowData = row as Record<string, unknown>;
                    const cellValue = rowData[col.key];
                    return (
                      <TableCell
                        key={col.key}
                        sx={{
                          ...(col.mobileHidden && { display: { xs: "none", sm: "table-cell" } }),
                          padding: { xs: "6px 6px", sm: "8px 10px", md: DT.cellPad },
                          fontSize: { xs: "12px", sm: DT.cellFontSize },
                          fontWeight: DT.cellFontWeight,
                          lineHeight: DT.cellLineHeight,
                          color: DT.cellColor,
                          borderBottom: DT.cellBorder,
                          transition: "color 0.18s ease",
                          ...(col.className === "cm-cell-center" && {
                            textAlign: "center" as const,
                          }),
                          ...(col.className === "cm-cell-right" && {
                            textAlign: "right" as const,
                          }),
                        }}
                      >
                        {col.render
                          ? col.render(cellValue, row, rowIndex)
                          : ((cellValue as React.ReactNode) ?? "—")}
                      </TableCell>
                    );
                  })}
                </TableRow>
              ))
            )}
            {enableInfiniteScroll && (
              <TableRow ref={sentinelRef} style={{ height: 1, visibility: "hidden" }}>
                <TableCell colSpan={columns.length} style={{ padding: 0, border: 0 }} />
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* ── Footer: Pagination ──────────────── */}
      {!enableInfiniteScroll && (
        <TablePagination
          component="div"
          count={sortedData.length}
          page={page}
          onPageChange={handlePageChange}
          rowsPerPage={rowsPerPage}
          onRowsPerPageChange={handleRowsPerPageChange}
          rowsPerPageOptions={pageSizeOptions ?? [5, 10, 25]}
          labelRowsPerPage=""
          labelDisplayedRows={({ from, to, count }) =>
            DATA_TABLE.DISPLAYED_ROWS(from, to, count)
          }
          sx={{
            bgcolor: DT.paginationBg,
            borderTop: DT.paginationBorder,
            paddingInline: DT.paginationPad,
            color: "var(--on-surface-variant)",
            fontSize: "12px",
            ".MuiTablePagination-toolbar": { minHeight: 40 },
            ".MuiTablePagination-select": { fontSize: "12px", color: "var(--on-surface)" },
            ".MuiTablePagination-selectIcon": { color: "var(--on-surface-variant)" },
            ".MuiTablePagination-displayedRows": { fontSize: "12px", color: "var(--on-surface-variant)" },
            ".MuiTablePagination-actions": {
              color: "var(--on-surface)",
              "& .Mui-disabled": { opacity: 0.35 },
              "& .MuiIconButton-root": {
                borderRadius: "8px",
                "&:hover": { bgcolor: "var(--surface)" },
              },
            },
          }}
        />
      )}
    </Paper>
  );
}
