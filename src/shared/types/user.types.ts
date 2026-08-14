import type { USER_ROLES } from "@/shared/constants/roles";
export type UserRole = typeof USER_ROLES[keyof typeof USER_ROLES];

export interface User {
  id: string;
  fullName: string;
  email: string;
  role: UserRole;
}
