import * as Yup from 'yup'
import type { HardshipApplicationRequest } from '../../api/client'

/**
 * Every field is held as a string.
 *
 * Numbers would mean the form carrying `NaN` for an empty money field, and a date input
 * hands back a `yyyy-MM-dd` string anyway — which is exactly what the API wants.
 */
export type ApplicationFormValues = {
  customerName: string
  dateOfBirth: string
  monthlyIncome: string
  monthlyExpenses: string
  hardshipReason: string
}

export const MINIMUM_AGE = 18

export const emptyApplication: ApplicationFormValues = {
  customerName: '',
  dateOfBirth: '',
  monthlyIncome: '',
  monthlyExpenses: '',
  hardshipReason: '',
}

function isMoney(value: string | undefined): boolean {
  if (value === undefined || value.trim() === '') {
    return false
  }

  const amount = Number(value)
  return Number.isFinite(amount) && amount >= 0
}

function ageOn(today: Date, dateOfBirth: Date): number {
  let age = today.getFullYear() - dateOfBirth.getFullYear()
  const monthDifference = today.getMonth() - dateOfBirth.getMonth()

  if (monthDifference < 0 || (monthDifference === 0 && today.getDate() < dateOfBirth.getDate())) {
    age--
  }

  return age
}

/**
 * Mirrors the server's rules so the form can answer immediately. The server still decides —
 * see `ApplicationForm`, which puts its answer back on the fields.
 *
 * There is deliberately no rule requiring expenses below income: a deficit is the situation
 * being reported, not a mistake.
 */
export const applicationSchema: Yup.ObjectSchema<ApplicationFormValues> = Yup.object({
  customerName: Yup.string()
    .trim()
    .required('Customer name is required.')
    .max(200, 'Customer name must be 200 characters or fewer.'),

  dateOfBirth: Yup.string()
    .required('Date of birth is required.')
    .test('in-the-past', 'Date of birth must be in the past.', (value) => {
      const date = new Date(`${value}T00:00:00`)
      return !Number.isNaN(date.getTime()) && date < startOfToday()
    })
    .test('old-enough', `Applicant must be at least ${MINIMUM_AGE} years old.`, (value) => {
      const date = new Date(`${value}T00:00:00`)
      return !Number.isNaN(date.getTime()) && ageOn(startOfToday(), date) >= MINIMUM_AGE
    }),

  monthlyIncome: Yup.string()
    .required('Monthly income is required.')
    .test('money', 'Enter an amount of 0 or more.', isMoney),

  monthlyExpenses: Yup.string()
    .required('Monthly expenses are required.')
    .test('money', 'Enter an amount of 0 or more.', isMoney),

  hardshipReason: Yup.string()
    .max(1000, 'Hardship reason must be 1000 characters or fewer.')
    .default(''),
})

function startOfToday(): Date {
  const now = new Date()
  return new Date(now.getFullYear(), now.getMonth(), now.getDate())
}

export function toRequest(values: ApplicationFormValues): HardshipApplicationRequest {
  return {
    customerName: values.customerName.trim(),
    dateOfBirth: values.dateOfBirth,
    monthlyIncome: Number(values.monthlyIncome),
    monthlyExpenses: Number(values.monthlyExpenses),
    hardshipReason: values.hardshipReason.trim() === '' ? null : values.hardshipReason.trim(),
  }
}
