import { getUserAvatar } from "@/features/leaderboard/utils/avatarUtils";

describe("getUserAvatar", () => {
  it("returns avatar URL for known user ID", () => {
    expect(getUserAvatar("4d73d5dd-5ffc-4e8d-a72c-f29cea936665")).toBe("/avatars/santhip.png");
  });

  it("returns avatar URL for admin ID", () => {
    expect(getUserAvatar("12d28b41-3ac2-4585-aa36-203cc5b465d8")).toBe("/avatars/admin.png");
  });

  it("returns empty string for unknown ID", () => {
    expect(getUserAvatar("unknown-id")).toBe("");
  });
});
