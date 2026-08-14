export const PODIUM_CARD_CONFIGS = {
  1: {
    avatarSize: 96,
    borderColor: "var(--podium-gold-border, #FBBF24)",
    gradient: "linear-gradient(180deg, var(--podium-gold-light, #FDE68A), var(--podium-gold-border, #FBBF24))",
    cardHeight: "240px",
  },
  2: {
    avatarSize: 84,
    borderColor: "var(--on-surface-variant)",
    gradient: "linear-gradient(180deg, var(--surface-container-high), var(--on-surface-variant))",
    cardHeight: "200px",
  },
  3: {
    avatarSize: 84,
    borderColor: "var(--podium-bronze-border, #EA580C)",
    gradient: "linear-gradient(180deg, var(--podium-bronze-light, #FED7AA), var(--podium-bronze-border, #EA580C))",
    cardHeight: "180px",
  },
} as const;
