import type { HardshipApplication } from '../api/client'

export function anApplication(overrides: Partial<HardshipApplication> = {}): HardshipApplication {
  const monthlyIncome = overrides.monthlyIncome ?? 5000
  const monthlyExpenses = overrides.monthlyExpenses ?? 3200

  return {
    id: '11111111-1111-1111-1111-111111111111',
    customerName: 'Alex Taylor',
    dateOfBirth: '1990-04-17',
    monthlyIncome,
    monthlyExpenses,
    hardshipReason: 'Reduced hours at work',
    monthlySurplus: monthlyIncome - monthlyExpenses,
    isInDeficit: monthlyIncome - monthlyExpenses < 0,
    createdAt: '2026-11-20T01:00:00+00:00',
    updatedAt: null,
    ...overrides,
  }
}
