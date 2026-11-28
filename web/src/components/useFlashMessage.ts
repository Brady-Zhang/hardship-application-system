import { useCallback } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'

export type FlashState = { flash?: string }

/**
 * A one-off message handed over by the page that navigated here — "saved", typically.
 *
 * It travels in the history entry rather than in a store, so there is no shared state to
 * reset and a message cannot outlive the navigation that caused it.
 */
export function useFlashMessage(): [string | null, () => void] {
  const location = useLocation()
  const navigate = useNavigate()

  const dismiss = useCallback(() => {
    navigate(location.pathname, { replace: true, state: null })
  }, [navigate, location.pathname])

  return [(location.state as FlashState | null)?.flash ?? null, dismiss]
}
