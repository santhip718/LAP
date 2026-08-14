import React from "react";
import Home from "@/features/home/pages/Home/Home";

(globalThis as any).IS_REACT_ACT_ENVIRONMENT = true;

jest.mock("react-router-dom", () => ({
  useNavigate: () => jest.fn(),
  Link: ({ children, to }: { children: React.ReactNode; to: string }) =>
    React.createElement("a", { href: to }, children),
}));

jest.mock("@/features/home/components/HeroSection/HeroSection", () => ({
  __esModule: true,
  default: () => React.createElement("div", { "data-testid": "hero-section" }),
}));

jest.mock("@/features/home/components/TrustedBySection/TrustedBySection", () => ({
  __esModule: true,
  default: () => React.createElement("div", { "data-testid": "trusted-by-section" }),
}));

jest.mock("@/features/home/components/FeaturesSection/FeaturesSection", () => ({
  __esModule: true,
  default: () => React.createElement("div", { "data-testid": "features-section" }),
}));

jest.mock("@/shared/components/layout/LapFooter/LapFooter", () => ({
  __esModule: true,
  default: () => React.createElement("footer", { "data-testid": "lap-footer" }),
}));

describe("Home page", () => {
  it("exports a valid React component", () => {
    expect(Home).toBeDefined();
    expect(typeof Home).toBe("function");
  });

  it("renders without throwing", async () => {
    const container = document.createElement("div");
    const { createRoot } = jest.requireActual("react-dom/client");
    const root = createRoot(container);
    await React.act(async () => {
      root.render(React.createElement(Home));
    });
    await React.act(async () => {
      root.unmount();
    });
  });

  it("renders all child sections", async () => {
    const container = document.createElement("div");
    const { createRoot } = jest.requireActual("react-dom/client");
    const root = createRoot(container);

    await React.act(async () => {
      root.render(React.createElement(Home));
    });

    expect(container.querySelector('[data-testid="hero-section"]')).not.toBeNull();
    expect(container.querySelector('[data-testid="trusted-by-section"]')).not.toBeNull();
    expect(container.querySelector('[data-testid="features-section"]')).not.toBeNull();
    expect(container.querySelector('[data-testid="lap-footer"]')).not.toBeNull();

    await React.act(async () => {
      root.unmount();
    });
  });
});
