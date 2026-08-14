import { feedbackService } from "@/shared/services/feedback/feedbackService";
import type { ToastItem } from "@/shared/services/feedback/feedbackService";

describe("feedbackService", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe("showToast", () => {
    it("emits toast:show event with a toast item", () => {
      const listener = jest.fn();
      feedbackService.on("toast:show", listener);

      feedbackService.showToast("Test message", "success");

      expect(listener).toHaveBeenCalledTimes(1);
      const toast: ToastItem = listener.mock.calls[0][0];
      expect(toast.message).toBe("Test message");
      expect(toast.type).toBe("success");
      expect(toast.duration).toBe(3000);
      expect(toast.id).toMatch(/^toast-/);
    });

    it("uses default type and duration when not provided", () => {
      const listener = jest.fn();
      feedbackService.on("toast:show", listener);

      feedbackService.showToast("Default toast");

      const toast: ToastItem = listener.mock.calls[0][0];
      expect(toast.type).toBe("info");
      expect(toast.duration).toBe(3000);
    });

    it("increments toast id on each call", () => {
      const listener = jest.fn();
      feedbackService.on("toast:show", listener);

      feedbackService.showToast("First");
      feedbackService.showToast("Second");

      expect(listener).toHaveBeenCalledTimes(2);
      const id1: string = listener.mock.calls[0][0].id;
      const id2: string = listener.mock.calls[1][0].id;
      expect(id1).not.toBe(id2);
    });
  });

  describe("dismissToast", () => {
    it("emits toast:dismiss event with the given id", () => {
      const listener = jest.fn();
      feedbackService.on("toast:dismiss", listener);

      feedbackService.dismissToast("toast-1");

      expect(listener).toHaveBeenCalledWith("toast-1");
    });
  });

  describe("showConfirm", () => {
    it("emits confirm:show event with config and resolve function", () => {
      const listener = jest.fn();
      feedbackService.on("confirm:show", listener);

      const config = { title: "Confirm?", message: "Are you sure?" };
      feedbackService.showConfirm(config);

      expect(listener).toHaveBeenCalledTimes(1);
      const payload = listener.mock.calls[0][0];
      expect(payload.config).toEqual(config);
      expect(typeof payload.resolve).toBe("function");
    });

    it("returns a promise that resolves when resolve(true) is called", async () => {
      let capturedResolve: ((value: boolean) => void) | null = null;
      feedbackService.on("confirm:show", ((payload: { resolve: (value: boolean) => void }) => {
        capturedResolve = payload.resolve;
      }) as (...args: unknown[]) => void);

      const promise = feedbackService.showConfirm({ title: "Test", message: "Test" });

      capturedResolve!(true);
      await expect(promise).resolves.toBe(true);
    });

    it("returns a promise that resolves when resolve(false) is called", async () => {
      let capturedResolve: ((value: boolean) => void) | null = null;
      feedbackService.on("confirm:show", ((payload: { resolve: (value: boolean) => void }) => {
        capturedResolve = payload.resolve;
      }) as (...args: unknown[]) => void);

      const promise = feedbackService.showConfirm({ title: "Test", message: "Test" });

      capturedResolve!(false);
      await expect(promise).resolves.toBe(false);
    });
  });

  describe("on/off", () => {
    it("can remove a registered listener", () => {
      const listener = jest.fn();
      feedbackService.on("toast:show", listener);
      feedbackService.off("toast:show", listener);

      feedbackService.showToast("Should not be heard");

      expect(listener).not.toHaveBeenCalled();
    });

    it("does not throw when unregistering a non-existent listener", () => {
      expect(() => {
        feedbackService.off("non-existent", jest.fn());
      }).not.toThrow();
    });
  });
});
