import type { Config } from "jest";

const config: Config = {
  testEnvironment: "jsdom",
  transform: {
    "^.+\\.tsx?$": "<rootDir>/jest-transform.mjs",
  },
  moduleNameMapper: {
    "^@/core/config/env$": "<rootDir>/src/tests/__mocks__/envMock.ts",
    "^@/(.+)\\.(jpg|jpeg|png|gif|webp|svg)$": "<rootDir>/src/tests/__mocks__/fileMock.ts",
    "^@/(.+)$": "<rootDir>/src/$1",
    "\\.(css|less|scss)$": "identity-obj-proxy",
    "\\.(jpg|jpeg|png|gif|webp|svg)$": "<rootDir>/src/tests/__mocks__/fileMock.ts",
  },
  setupFilesAfterEnv: ["<rootDir>/src/tests/setup.ts"],
  testMatch: [
    "<rootDir>/src/tests/unit/**/*.test.{ts,tsx}",
  ],
  moduleFileExtensions: ["ts", "tsx", "js", "jsx", "json"],
  modulePathIgnorePatterns: ["<rootDir>/dist/"],
  transformIgnorePatterns: [
    "node_modules/(?!(@mui|react-router-dom|react-hook-form)/)",
  ],
};

export default config;
