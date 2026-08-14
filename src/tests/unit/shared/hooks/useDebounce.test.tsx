import { renderHook, act } from "@testing-library/react";
import { useDebounce } from "@/shared/hooks/useDebounce";

beforeEach(() => {
  jest.useFakeTimers();
});

afterEach(() => {
  jest.useRealTimers();
});

describe("useDebounce", () => {
  it("returns the initial value immediately", () => {
    const { result } = renderHook(() => useDebounce("hello", 500));
    expect(result.current).toBe("hello");
  });

  it("updates after the delay", () => {
    const { result, rerender } = renderHook(
      ({ value, delay }) => useDebounce(value, delay),
      { initialProps: { value: "hello", delay: 500 } },
    );

    rerender({ value: "world", delay: 500 });

    expect(result.current).toBe("hello");

    act(() => {
      jest.advanceTimersByTime(500);
    });

    expect(result.current).toBe("world");
  });

  it("cancels previous timer on rapid changes", () => {
    const { result, rerender } = renderHook(
      ({ value, delay }) => useDebounce(value, delay),
      { initialProps: { value: "a", delay: 300 } },
    );

    rerender({ value: "b", delay: 300 });
    rerender({ value: "c", delay: 300 });

    act(() => {
      jest.advanceTimersByTime(200);
    });

    expect(result.current).toBe("a");

    act(() => {
      jest.advanceTimersByTime(100);
    });

    expect(result.current).toBe("c");
  });

  it("handles delay of 0", () => {
    const { result, rerender } = renderHook(
      ({ value, delay }) => useDebounce(value, delay),
      { initialProps: { value: "first", delay: 0 } },
    );

    rerender({ value: "second", delay: 0 });

    act(() => {
      jest.runAllTimers();
    });

    expect(result.current).toBe("second");
  });
});
