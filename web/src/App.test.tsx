import { screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import App from './App'
import { renderWithProviders } from './test/renderWithProviders'

describe('App', () => {
  it('renders the new application form', () => {
    renderWithProviders(<App />)

    expect(screen.getByRole('heading', { name: /hardship applications/i })).toBeVisible()
    expect(screen.getByLabelText(/customer name/i)).toBeVisible()
    expect(screen.getByRole('button', { name: /submit application/i })).toBeVisible()
  })
})
