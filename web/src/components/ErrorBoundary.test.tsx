import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import type { ReactNode } from 'react'
import { MemoryRouter, Link, Route, Routes, useLocation } from 'react-router-dom'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { ErrorBoundary } from './ErrorBoundary'

function Boom(): ReactNode {
  throw new Error('render exploded')
}

describe('ErrorBoundary', () => {
  // React writes the caught error to the console; the test knows about it already.
  beforeEach(() => vi.spyOn(console, 'error').mockImplementation(() => {}))
  afterEach(() => vi.restoreAllMocks())

  it('leaves a working tree alone', () => {
    render(
      <ErrorBoundary>
        <p>All fine</p>
      </ErrorBoundary>,
    )

    expect(screen.getByText('All fine')).toBeVisible()
  })

  it('shows a message instead of a blank page when a render throws', () => {
    render(
      <ErrorBoundary>
        <Boom />
      </ErrorBoundary>,
    )

    expect(screen.getByRole('alert')).toHaveTextContent('Something went wrong')
    expect(screen.getByRole('button', { name: /reload the page/i })).toBeVisible()
  })

  it('recovers when the path changes, because the boundary is keyed by it', async () => {
    const user = userEvent.setup()

    function Shell() {
      const location = useLocation()

      return (
        <>
          <Link to="/">Home</Link>
          <ErrorBoundary key={location.pathname}>
            <Routes>
              <Route path="/" element={<p>The list</p>} />
              <Route path="/boom" element={<Boom />} />
            </Routes>
          </ErrorBoundary>
        </>
      )
    }

    render(
      <MemoryRouter initialEntries={['/boom']}>
        <Shell />
      </MemoryRouter>,
    )

    expect(screen.getByRole('alert')).toHaveTextContent('Something went wrong')

    await user.click(screen.getByRole('link', { name: 'Home' }))

    expect(screen.getByText('The list')).toBeVisible()
    expect(screen.queryByRole('alert')).not.toBeInTheDocument()
  })
})
