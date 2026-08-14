import { createRequire } from "module";
const require = createRequire(import.meta.url);

let tsJestTransformer;
try {
  tsJestTransformer = require("ts-jest").default.createTransformer({
    tsconfig: "tsconfig.test.json",
    diagnostics: false,
  });
} catch {
  tsJestTransformer = require("ts-jest").createTransformer({
    tsconfig: "tsconfig.test.json",
    diagnostics: false,
  });
}

export default {
  process(sourceText, sourcePath, options) {
    const modified = sourceText.replace(/import\.meta\.env\.(\w+)/g, (_, prop) => {
      if (prop === "PROD") return "true";
      if (prop === "DEV") return "false";
      if (prop === "MODE") return '"test"';
      if (prop === "BASE_URL") return '"/"';
      return "undefined";
    });
    return tsJestTransformer.process(modified, sourcePath, options);
  },
  getCacheKey(sourceText, sourcePath, options) {
    return tsJestTransformer.getCacheKey(sourceText, sourcePath, options);
  },
};
