import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import App from '../../App'
import { ApiError, applicationsApi } from '../../api/client'
import { anApplication } from '../../test/anApplication'
import { renderWithProviders } from '../../test/renderWithProviders'

vi.mock('../../api/client', async (importOriginal) => ({
  ...(await importOriginal<typeof import('../../api/client')>()),
  applicationsApi: { list: vi.fn(), get: vi.fn(), create: vi.fn(), update: vi.fn() },
}))

const list = vi.mocked(applicationsApi.list)
const get = vi.mocked(applicationsApi.get)
const update = vi.mocked(applicationsApi.update)

const id = '11111111-1111-1111-1111-111111111111'
const editRoute = `/applications/${id}/edit`

describe('EditApplicationPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('fills the form with the record once it has loaded', async () => {
    get.mockResolvedValue(anApplication())

    renderWithProviders(<App />, { route: editRoute })

    expect(screen.getByLabelText('Loading application')).toBeVisible()

    expect(await screen.findByDisplayValue('Alex Taylor')).toBeVisible()
    expect(screen.getByLabelText(/date of birth/i)).toHaveValue('1990-04-17')
    expect(screen.getByLabelText(/monthly income/i)).toHaveValue(5000)
    expect(screen.getByLabelText(/reason for hardship/i)).toHaveValue('Reduced hours at work')
    expect(get).toHaveBeenCalledWith(id)
  })

  it('saves the change and returns to the list with a message', async () => {
    const user = userEvent.setup()
    get.mockResolvedValue(anApplication())
    update.mockResolvedValue(anApplication({ customerName: 'Alex Taylor-Smith' }))
    list.mockResolvedValue([anApplication({ customerName: 'Alex Taylor-Smith' })])

    renderWithProviders(<App />, { route: editRoute })

    const name = await screen.findByDisplayValue('Alex Taylor')
    await user.clear(name)
    await user.type(name, 'Alex Taylor-Smith')
    await user.click(screen.getByRole('button', { name: /save changes/i }))

    await waitFor(() => expect(update).toHaveBeenCalledOnce())
    expect(update).toHaveBeenCalledWith(id, {
      customerName: 'Alex Taylor-Smith',
      dateOfBirth: '1990-04-17',
      monthlyIncome: 5000,
      monthlyExpenses: 3200,
      hardshipReason: 'Reduced hours at work',
    })

    expect(await screen.findByText('Changes saved for Alex Taylor-Smith')).toBeVisible()
    expect(screen.getByRole('heading', { name: 'Applications' })).toBeVisible()
  })

  it('shows what the server said about an unknown record', async () => {
    get.mockRejectedValue(new ApiError(404, 'Application not found'))

    renderWithProviders(<App />, { route: editRoute })

    expect(await screen.findByRole('alert')).toHaveTextContent('Application not found')
    expect(screen.queryByLabelText(/customer name/i)).not.toBeInTheDocument()
  })
})
