import { screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import App from './App'
import { applicationsApi } from './api/client'
import { renderWithProviders } from './test/renderWithProviders'

vi.mock('./api/client', async (importOriginal) => ({
  ...(await importOriginal<typeof import('./api/client')>()),
  applicationsApi: { list: vi.fn(), get: vi.fn(), create: vi.fn(), update: vi.fn() },
}))

const list = vi.mocked(applicationsApi.list)

describe('App', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    list.mockResolvedValue([])
  })

  it('opens on the list of applications', async () => {
    renderWithProviders(<App />)

    expect(screen.getByRole('heading', { name: 'Hardship applications' })).toBeVisible()
    expect(await screen.findByRole('heading', { name: 'Applications' })).toBeVisible()
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument()
  })

  it('opens the form in a dialog over the list, and closes it again', async () => {
    const user = userEvent.setup()
    renderWithProviders(<App />)

    await user.click(await screen.findByRole('link', { name: /new application/i }))

    expect(await screen.findByRole('dialog')).toBeVisible()
    expect(screen.getByLabelText(/customer name/i)).toBeVisible()
    // A modal hides the rest of the app from assistive tech, so the list is no longer
    // reachable by role while the dialog is open — but it is still mounted behind it.
    expect(screen.getByText('Applications')).toBeInTheDocument()

    await user.click(screen.getByRole('button', { name: /cancel/i }))

    await waitFor(() => expect(screen.queryByRole('dialog')).not.toBeInTheDocument())
    expect(screen.getByRole('heading', { name: 'Applications' })).toBeVisible()
  })

  it('has somewhere to go from an unknown address', async () => {
    renderWithProviders(<App />, { route: '/nope' })

    expect(screen.getByText(/does not exist/i)).toBeVisible()
    expect(screen.getByRole('link', { name: /back to applications/i })).toBeVisible()
  })
})
