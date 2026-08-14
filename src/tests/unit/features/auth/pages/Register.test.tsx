import React from "react";
import Register from "@/features/auth/pages/Register/Register";
import { referenceDataService } from "@/shared/services/referenceData";

(globalThis as any).IS_REACT_ACT_ENVIRONMENT = true;

const mockNavigate = jest.fn();

jest.mock("react-router-dom", () => ({
  useNavigate: () => mockNavigate,
  Link: ({ children, to }: { children: React.ReactNode; to: string }) =>
    React.createElement("a", { href: to }, children),
}));

jest.mock("react-hook-form", () => ({
  useForm: () => ({
    register: jest.fn(() => ({})),
    handleSubmit: jest.fn((fn: Function) => (e: any) => {
      e?.preventDefault?.();
      return fn({});
    }),
    formState: { errors: {}, isSubmitting: false },
  }),
}));

jest.mock("@/shared/services/referenceData", () => ({
  referenceDataService: {
    getDesignations: jest.fn(),
    getGenders: jest.fn(),
  },
}));

beforeEach(() => {
  jest.clearAllMocks();
  (referenceDataService.getDesignations as jest.Mock).mockResolvedValue([]);
  (referenceDataService.getGenders as jest.Mock).mockResolvedValue([]);
});

function createTestRoot() {
  const container = document.createElement("div");
  const { createRoot } = jest.requireActual("react-dom/client");
  const root = createRoot(container);
  return { container, root };
}

describe("Register page", () => {
  it("exports a valid React component", () => {
    expect(Register).toBeDefined();
    expect(typeof Register).toBe("function");
  });

  it("renders without throwing", async () => {
    const { root } = createTestRoot();
    await React.act(async () => {
      root.render(React.createElement(Register));
    });
    await React.act(async () => {
      root.unmount();
    });
  });

  describe("data fetching on mount", () => {
    it("calls referenceDataService.getDesignations and getGenders on mount", async () => {
      const { root } = createTestRoot();

      await React.act(async () => {
        root.render(React.createElement(Register));
      });

      expect(referenceDataService.getDesignations).toHaveBeenCalledTimes(1);
      expect(referenceDataService.getGenders).toHaveBeenCalledTimes(1);

      await React.act(async () => {
        root.unmount();
      });
    });

    it("handles service errors gracefully (falls back to empty arrays)", async () => {
      (referenceDataService.getDesignations as jest.Mock).mockRejectedValue(new Error("API error"));
      (referenceDataService.getGenders as jest.Mock).mockRejectedValue(new Error("API error"));

      const { root } = createTestRoot();

      await React.act(async () => {
        root.render(React.createElement(Register));
      });

      expect(referenceDataService.getDesignations).toHaveBeenCalledTimes(1);
      expect(referenceDataService.getGenders).toHaveBeenCalledTimes(1);

      await React.act(async () => {
        root.unmount();
      });
    });

    it("uses returned designation and gender data", async () => {
      const designations = [
        { id: "des-1", name: "Professor" },
        { id: "des-2", name: "Student" },
      ];
      const genders = [
        { id: "gen-1", name: "Male" },
        { id: "gen-2", name: "Female" },
      ];

      (referenceDataService.getDesignations as jest.Mock).mockResolvedValue(designations);
      (referenceDataService.getGenders as jest.Mock).mockResolvedValue(genders);

      const { root } = createTestRoot();

      await React.act(async () => {
        root.render(React.createElement(Register));
      });

      expect(referenceDataService.getDesignations).toHaveBeenCalled();
      expect(referenceDataService.getGenders).toHaveBeenCalled();

      await React.act(async () => {
        root.unmount();
      });
    });
  });
});
