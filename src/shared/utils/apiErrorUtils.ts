export function extractErrorMessage(err: unknown, fallback = "Something went wrong"): string {
  const msg =
    (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
    (err as { message?: string })?.message ??
    fallback;
  return msg;
}
