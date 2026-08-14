import { ASSESSMENT_MANAGEMENT } from "@/features/admin/pages/AssessmentManagement/AssessmentManagement.constants";

describe("AssessmentManagement constants", () => {
  it("has correct PAGE_TITLE and PAGE_SUBTITLE", () => {
    expect(ASSESSMENT_MANAGEMENT.PAGE_TITLE).toBe("Assessment Management");
    expect(ASSESSMENT_MANAGEMENT.PAGE_SUBTITLE).toBe("Manage, review and organize assessments for your courses.");
  });

  it("has correct button labels", () => {
    expect(ASSESSMENT_MANAGEMENT.BTN_CREATE).toBe(" Create Assessment");
    expect(ASSESSMENT_MANAGEMENT.BTN_RETRY).toBe("Try Again");
  });

  it("has correct error and loading constants", () => {
    expect(ASSESSMENT_MANAGEMENT.LOADING).toBe("Loading...");
    expect(ASSESSMENT_MANAGEMENT.ERROR_LOAD).toBe("Failed to load assessments");
  });

  it("has correct empty state constants", () => {
    expect(ASSESSMENT_MANAGEMENT.EMPTY_TITLE).toBe("No assessments available yet.");
    expect(ASSESSMENT_MANAGEMENT.EMPTY_MESSAGE).toBe("No assessments are available for this course.");
  });

  it("has correct modal constants", () => {
    expect(ASSESSMENT_MANAGEMENT.MODAL_CREATE_TITLE).toBe("Create Assessment");
    expect(ASSESSMENT_MANAGEMENT.MODAL_DELETE_TITLE).toBe("Delete Assessment");
    expect(ASSESSMENT_MANAGEMENT.MODAL_DELETE_MESSAGE).toBe("Are you sure you want to delete this assessment? This action cannot be undone.");
    expect(ASSESSMENT_MANAGEMENT.BTN_DELETE_CANCEL).toBe("Cancel");
    expect(ASSESSMENT_MANAGEMENT.BTN_DELETE_CONFIRM).toBe("Delete");
    expect(ASSESSMENT_MANAGEMENT.BTN_DELETE_DELETING).toBe("Deleting...");
  });

  it("has correct toast messages", () => {
    expect(ASSESSMENT_MANAGEMENT.TOAST_DELETE_SUCCESS).toBe("Assessment deleted successfully.");
    expect(ASSESSMENT_MANAGEMENT.TOAST_DELETE_ERROR).toBe("Failed to delete assessment. Please try again.");
  });
});
