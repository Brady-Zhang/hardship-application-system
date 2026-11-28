import { Alert, AlertTitle, Button, Stack } from '@mui/material'
import { Component, type ErrorInfo, type ReactNode } from 'react'

type Props = { children: ReactNode }
type State = { failed: boolean }

/**
 * The last line of defence: a render that throws would otherwise leave a blank page.
 *
 * There is no "try again" here. Re-rendering the same tree with the same state throws
 * again; reloading is the honest offer. Mounted with the path as its key, so navigating
 * away from a broken page is enough to recover.
 */
export class ErrorBoundary extends Component<Props, State> {
  state: State = { failed: false }

  static getDerivedStateFromError(): State {
    return { failed: true }
  }

  componentDidCatch(error: Error, info: ErrorInfo): void {
    console.error('Unhandled rendering error', error, info.componentStack)
  }

  render(): ReactNode {
    if (!this.state.failed) {
      return this.props.children
    }

    return (
      <Stack spacing={2} sx={{ alignItems: 'flex-start' }}>
        <Alert severity="error">
          <AlertTitle>Something went wrong</AlertTitle>
          This page could not be displayed. Nothing you entered has been sent.
        </Alert>

        <Button variant="contained" onClick={() => window.location.reload()}>
          Reload the page
        </Button>
      </Stack>
    )
  }
}
