export const PERMISSIONS = {} as const;

export type Permission = typeof PERMISSIONS[keyof typeof PERMISSIONS];
