export interface LoginPayload {
  email: string;
  password: string;
}

export interface RegisterPayload {
  fullName: string;
  email: string;
  password: string;
  mobileNumber: string;
  designationId: string;
  genderId: string;
}

export interface JwtPayload {
  sub?: string;
  email?: string;
  [key: string]: unknown;
}
