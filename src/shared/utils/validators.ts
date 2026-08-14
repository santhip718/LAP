import { REGEX } from "@/shared/constants/regex";

export function isValidEmail(email: string): boolean {
  return REGEX.EMAIL.test(email);
}
