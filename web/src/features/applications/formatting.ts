const money = new Intl.NumberFormat('en-AU', { style: 'currency', currency: 'AUD' })
const day = new Intl.DateTimeFormat('en-AU', { dateStyle: 'medium' })
const instant = new Intl.DateTimeFormat('en-AU', { dateStyle: 'medium', timeStyle: 'short' })

export function formatMoney(amount: number): string {
  return money.format(amount)
}

// A date of birth is a calendar date, not a moment. Parsing it with an explicit midnight keeps
// it in the local day; `new Date('1990-04-17')` is UTC midnight and shows as the 16th here.
export function formatDay(isoDate: string): string {
  return day.format(new Date(`${isoDate}T00:00:00`))
}

export function formatInstant(isoInstant: string): string {
  return instant.format(new Date(isoInstant))
}
