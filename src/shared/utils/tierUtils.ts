/**
 * Resolves the tier name based on the percentage score.
 * Matches backend tier calculations exactly.
 *
 * @param score The percentage score (e.g. 0 to 100)
 */
export function getTier(score: number): string {
  if (score <= 20) {
    return 'Code Cadet'; // Baseline
  }
  if (score <= 40) {
    return 'Syntax Voyager'; // Easy master
  }
  if (score <= 60) {
    return 'Logic Architect'; // Medium master
  }
  if (score <= 80) {
    return 'Runtime Titan'; // Hard master
  }
  return 'System Sovereign'; // Top of the leaderboard
}
