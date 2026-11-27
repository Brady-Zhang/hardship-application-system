import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'
import { ApiError, type HardshipApplicationRequest } from '../../api/client'
import { ApplicationForm } from './ApplicationForm'
import { emptyApplication } from './applicationSchema'

function renderForm(onSubmit: (request: HardshipApplicationRequest) => Promise<unknown>) {
  return render(
    <ApplicationForm
      initialValues={emptyApplication}
      submitLabel="Submit application"
      onSubmit={onSubmit}
    />,
  )
}

async function fillInAValidApplication(user: ReturnType<typeof userEvent.setup>) {
  await user.type(screen.getByLabelText(/customer name/i), 'Alex Taylor')
  await user.type(screen.getByLabelText(/date of birth/i), '1990-04-17')
  await user.type(screen.getByLabelText(/monthly income/i), '5000')
  await user.type(screen.getByLabelText(/monthly expenses/i), '3200')
}

describe('ApplicationForm', () => {
  it('refuses to submit an empty form and says which fields are missing', async () => {
    const user = userEvent.setup()
    const onSubmit = vi.fn()
    renderForm(onSubmit)

    await user.click(screen.getByRole('button', { name: /submit application/i }))

    expect(await screen.findByText('Customer name is required.')).toBeVisible()
    expect(screen.getByText('Date of birth is required.')).toBeVisible()
    expect(screen.getByText('Monthly income is required.')).toBeVisible()
    expect(onSubmit).not.toHaveBeenCalled()
  })

  it('sends numbers and a null reason when the reason is left blank', async () => {
    const user = userEvent.setup()
    const onSubmit = vi.fn().mockResolvedValue({})
    renderForm(onSubmit)

    await fillInAValidApplication(user)
    await user.click(screen.getByRole('button', { name: /submit application/i }))

    await waitFor(() => expect(onSubmit).toHaveBeenCalledOnce())
    expect(onSubmit).toHaveBeenCalledWith({
      customerName: 'Alex Taylor',
      dateOfBirth: '1990-04-17',
      monthlyIncome: 5000,
      monthlyExpenses: 3200,
      hardshipReason: null,
    })
  })

  it('accepts expenses above income', async () => {
    const user = userEvent.setup()
    const onSubmit = vi.fn().mockResolvedValue({})
    renderForm(onSubmit)

    await user.type(screen.getByLabelText(/customer name/i), 'Alex Taylor')
    await user.type(screen.getByLabelText(/date of birth/i), '1990-04-17')
    await user.type(screen.getByLabelText(/monthly income/i), '2500')
    await user.type(screen.getByLabelText(/monthly expenses/i), '4100')
    await user.click(screen.getByRole('button', { name: /submit application/i }))

    await waitFor(() => expect(onSubmit).toHaveBeenCalledOnce())
  })

  it('rejects an applicant under eighteen', async () => {
    const user = userEvent.setup()
    const onSubmit = vi.fn()
    renderForm(onSubmit)

    const lastYear = new Date()
    lastYear.setFullYear(lastYear.getFullYear() - 1)

    await user.type(screen.getByLabelText(/customer name/i), 'Sam Young')
    await user.type(screen.getByLabelText(/date of birth/i), lastYear.toISOString().slice(0, 10))
    await user.type(screen.getByLabelText(/monthly income/i), '100')
    await user.type(screen.getByLabelText(/monthly expenses/i), '200')
    await user.click(screen.getByRole('button', { name: /submit application/i }))

    expect(await screen.findByText(/at least 18 years old/i)).toBeVisible()
    expect(onSubmit).not.toHaveBeenCalled()
  })

  it('puts the server validation errors onto the fields they belong to', async () => {
    const user = userEvent.setup()
    const onSubmit = vi.fn().mockRejectedValue(
      new ApiError(400, 'One or more validation errors occurred.', {
        customerName: ['That name is already on file.'],
      }),
    )
    renderForm(onSubmit)

    await fillInAValidApplication(user)
    await user.click(screen.getByRole('button', { name: /submit application/i }))

    expect(await screen.findByText('That name is already on file.')).toBeVisible()
  })

  it('shows a form level message when the failure belongs to no field', async () => {
    const user = userEvent.setup()
    const onSubmit = vi.fn().mockRejectedValue(new ApiError(0, 'Could not reach the server.'))
    renderForm(onSubmit)

    await fillInAValidApplication(user)
    await user.click(screen.getByRole('button', { name: /submit application/i }))

    expect(await screen.findByRole('alert')).toHaveTextContent('Could not reach the server.')
  })
})
