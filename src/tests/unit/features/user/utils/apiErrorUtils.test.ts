import { extractErrorMessage } from "@/shared/utils/apiErrorUtils";

describe("extractErrorMessage", () => {
  it("extracts message from axios error response.data.message", () => {
    const err = { response: { data: { message: "Email already taken" } } };
    expect(extractErrorMessage(err)).toBe("Email already taken");
  });

  it("extracts message from Error instance", () => {
    const err = new Error("Network failure");
    expect(extractErrorMessage(err)).toBe("Network failure");
  });

  it("returns fallback when no message is found", () => {
    expect(extractErrorMessage({})).toBe("Something went wrong");
  });

  it("returns custom fallback when provided", () => {
    expect(extractErrorMessage(null, "Custom fallback")).toBe("Custom fallback");
  });

  it("extracts message over fallback when both exist", () => {
    const err = new Error("Real error");
    expect(extractErrorMessage(err, "Fallback")).toBe("Real error");
  });

  it("handles undefined error gracefully", () => {
    expect(extractErrorMessage(undefined)).toBe("Something went wrong");
  });

  it("falls back for plain string errors (no message property)", () => {
    expect(extractErrorMessage("raw string")).toBe("Something went wrong");
  });
});
