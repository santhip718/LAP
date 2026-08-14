import "./StatCard.css";

interface StatCardProps {
  label: string;
  value: string | number;
  trend?: {
    text: string;
    icon?: string;
    color?: "emerald" | "secondary";
  };
  progress?: number;
}

export default function StatCard({ label, value, trend, progress }: StatCardProps) {
  return (
    <div className="statcard">
      <p className="statcard-label">{label}</p>
      <h3 className="statcard-value">{value}</h3>
      {trend && (
        <div className={`statcard-trend statcard-trend-${trend.color ?? "emerald"}`}>
          <span className="material-symbols-outlined statcard-trend-icon">
            {trend.icon ?? (trend.color === "secondary" ? "insights" : "trending_up")}
          </span>
          <span>{trend.text}</span>
        </div>
      )}
      {progress != null && (
        <div className="statcard-progress-track">
          <div
            className="statcard-progress-fill"
            style={{ width: `${Math.min(100, Math.max(0, progress))}%` }}
          />
        </div>
      )}
    </div>
  );
}
