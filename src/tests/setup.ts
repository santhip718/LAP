import "@testing-library/jest-dom";
import { TextEncoder, TextDecoder } from "util";

if (typeof globalThis.TextEncoder === "undefined") {
  globalThis.TextEncoder = TextEncoder;
  globalThis.TextDecoder = TextDecoder as typeof global.TextDecoder;
}

Element.prototype.scrollIntoView = jest.fn();

globalThis.IntersectionObserver = class IntersectionObserver {
  constructor() { /* noop */ }
  observe() { /* noop */ }
  unobserve() { /* noop */ }
  disconnect() { /* noop */ }
  takeRecords() { return []; }
} as unknown as typeof IntersectionObserver;

Object.defineProperty(globalThis, "import", {
  value: {
    meta: {
      env: {
        DEV: false,
        PROD: true,
        MODE: "test",
        BASE_URL: "/",
      },
    },
  },
  writable: true,
  configurable: true,
});
