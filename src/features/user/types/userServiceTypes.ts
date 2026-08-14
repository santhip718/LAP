export interface RefTerm {
  id: string;
  name: string;
}

export interface UserListItem {
  id: string;
  fullName: string;
  email: string;
  roles: string[];
}

export interface UserListResult {
  users: UserListItem[];
  total: number;
  page: number;
  pageSize: number;
}

export interface UserProfile {
  id: string;
  fullName: string;
  email: string;
  mobileNumber: string;
  designation: string;
  designationId: string;
  gender: string;
  genderId: string;
  currentTier: string;
  roles: string[];
  dateCreated: string;
  profileImage: string | null;
}

export type UserDetail = UserProfile;

export interface EditUserRequest {
  fullName: string;
  email: string;
  mobileNumber: string;
  designationId: string;
  genderId: string;
  roles: string[];
}
