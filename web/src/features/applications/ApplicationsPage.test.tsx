import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { ApiError, applicationsApi } from '../../api/client'
import { anApplication } from '../../test/anApplication'
import { renderWithProviders } from '../../test/renderWithProviders'
import { ApplicationsPage } from './ApplicationsPage'

vi.mock('../../api/client', async (importOriginal) => ({
  ...(await importOriginal<typeof import('../../api/client')>()),
  applicationsApi: { list: vi.fn(), get: vi.fn(), create: vi.fn(), update: vi.fn() },
}))

const list = vi.mocked(applicationsApi.list)

describe('ApplicationsPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('shows a loading indicator and then the records', async () => {
    list.mockResolvedValue([
      anApplication({ customerName: 'Jordan Lee', monthlyIncome: 4321.55, monthlyExpenses: 1234.45 }),
    ])

    renderWithProviders(<ApplicationsPage />)

    expect(screen.getByLabelText('Loading applications')).toBeVisible()

    expect(await screen.findByText('Jordan Lee')).toBeVisible()
    expect(screen.getByText('17 Apr 1990')).toBeVisible()
    expect(screen.getByText('$4,321.55')).toBeVisible()
    expect(screen.getByText('$3,087.10')).toBeVisible()
  })

  it('calls out a deficit in words as well as in colour', async () => {
    list.mockResolvedValue([anApplication({ monthlyIncome: 2500, monthlyExpenses: 4100 })])

    renderWithProviders(<ApplicationsPage />)

    expect(await screen.findByText('-$1,600.00')).toBeVisible()
    expect(screen.getByText(/in deficit/)).toBeInTheDocument()
  })

  it('says so when there is nothing to show', async () => {
    list.mockResolvedValue([])

    renderWithProviders(<ApplicationsPage />)

    expect(await screen.findByText(/no applications yet/i)).toBeVisible()
    expect(screen.queryByRole('table')).not.toBeInTheDocument()
  })

  it('shows the failure and recovers on retry', async () => {
    const user = userEvent.setup()
    list.mockRejectedValueOnce(new ApiError(0, 'Could not reach the server.'))

    renderWithProviders(<ApplicationsPage />)

    expect(await screen.findByRole('alert')).toHaveTextContent('Could not reach the server.')

    list.mockResolvedValue([anApplication({ customerName: 'Jordan Lee' })])
    await user.click(screen.getByRole('button', { name: /retry/i }))

    expect(await screen.findByText('Jordan Lee')).toBeVisible()
    await waitFor(() => expect(screen.queryByRole('alert')).not.toBeInTheDocument())
  })
})
