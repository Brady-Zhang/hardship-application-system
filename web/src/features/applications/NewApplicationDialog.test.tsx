import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import App from '../../App'
import { applicationsApi } from '../../api/client'
import { anApplication } from '../../test/anApplication'
import { renderWithProviders } from '../../test/renderWithProviders'

vi.mock('../../api/client', async (importOriginal) => ({
  ...(await importOriginal<typeof import('../../api/client')>()),
  applicationsApi: { list: vi.fn(), get: vi.fn(), create: vi.fn(), update: vi.fn() },
}))

const list = vi.mocked(applicationsApi.list)
const create = vi.mocked(applicationsApi.create)

describe('NewApplicationDialog', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    list.mockResolvedValue([])
  })

  it('submits the application and closes, leaving a message on the list', async () => {
    const user = userEvent.setup()
    create.mockResolvedValue(anApplication({ customerName: 'Jordan Lee' }))

    renderWithProviders(<App />, { route: '/applications/new' })

    const dialog = await screen.findByRole('dialog')
    expect(dialog).toHaveAccessibleName('New hardship application')

    await user.type(screen.getByLabelText(/customer name/i), 'Jordan Lee')
    await user.type(screen.getByLabelText(/date of birth/i), '1988-09-02')
    await user.type(screen.getByLabelText(/monthly income/i), '4321.55')
    await user.type(screen.getByLabelText(/monthly expenses/i), '1234.45')
    await user.click(screen.getByRole('button', { name: /submit application/i }))

    await waitFor(() => expect(create).toHaveBeenCalledOnce())
    expect(create).toHaveBeenCalledWith({
      customerName: 'Jordan Lee',
      dateOfBirth: '1988-09-02',
      monthlyIncome: 4321.55,
      monthlyExpenses: 1234.45,
      hardshipReason: null,
    })

    expect(await screen.findByText('Application saved for Jordan Lee')).toBeVisible()
    await waitFor(() => expect(screen.queryByRole('dialog')).not.toBeInTheDocument())
  })

  it('stays open and says what is missing when the form is empty', async () => {
    const user = userEvent.setup()

    renderWithProviders(<App />, { route: '/applications/new' })

    await user.click(await screen.findByRole('button', { name: /submit application/i }))

    expect(await screen.findByText('Customer name is required.')).toBeVisible()
    expect(create).not.toHaveBeenCalled()
    expect(screen.getByRole('dialog')).toBeVisible()
  })
})
