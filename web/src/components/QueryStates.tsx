import { Alert, Box, Button, CircularProgress } from '@mui/material'
import { messageFor } from '../api/errors'

export function Loading({ label }: { label: string }) {
  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}>
      <CircularProgress aria-label={label} />
    </Box>
  )
}

// Without an `onRetry` the message stands on its own: a record that does not exist will
// not exist on the second try either.
export function LoadFailed({ error, onRetry }: { error: unknown; onRetry?: () => void }) {
  return (
    <Alert
      severity="error"
      action={
        onRetry && (
          <Button color="inherit" size="small" onClick={onRetry}>
            Retry
          </Button>
        )
      }
    >
      {messageFor(error)}
    </Alert>
  )
}
