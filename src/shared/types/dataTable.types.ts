export interface Column<T> {
  key: string;
  label: string;
  sortable?: boolean;
  sortValue?: (row: T) => string | number | null;
  render?: (value: unknown, row: T, index: number) => React.ReactNode;
  className?: string;
  thClassName?: string;
  mobileHidden?: boolean;
}
