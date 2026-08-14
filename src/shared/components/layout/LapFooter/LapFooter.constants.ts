export const FOOTER = {
  BRAND: "EduFlow",
  TAGLINE:
    "Bridging academic excellence with computational intelligence for the future of learning.",
} as const;

export const FOOTER_SECTIONS = [
  {
    heading: "Resources",
    links: [
      { label: "Research Papers", href: "#" },
      { label: "API Docs", href: "#" },
      { label: "Documentation", href: "#" },
    ],
  },
  {
    heading: "Company",
    links: [
      { label: "About Us", href: "#" },
      { label: "Careers", href: "#" },
      { label: "Press Kit", href: "#" },
    ],
  },
  {
    heading: "Legal",
    links: [
      { label: "Privacy Policy", href: "#" },
      { label: "Terms of Service", href: "#" },
      { label: "Cookie Settings", href: "#" },
    ],
  },
] as const;

export const COPYRIGHT = `\u00A9 2024 EduFlow AI. Bridging academic excellence with computational intelligence.`;
