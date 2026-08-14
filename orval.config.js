module.exports = {
  learningPortal: {
    input: "http://localhost:5020/swagger/v1/swagger.json",
    output: {
      mode: "tags-split",
      target: "./src/shared/services/api/services",
      schemas: "./src/shared/services/api/models",
      client: "axios",
    },
  },
};
