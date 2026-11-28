import { Alert, Button, Container, Link, Stack, Typography } from '@mui/material'
import { Link as RouterLink, Outlet, Route, Routes, useLocation } from 'react-router-dom'
import { ErrorBoundary } from './components/ErrorBoundary'
import { ApplicationsPage } from './features/applications/ApplicationsPage'
import { EditApplicationDialog } from './features/applications/EditApplicationDialog'
import { NewApplicationDialog } from './features/applications/NewApplicationDialog'

function NotFound() {
  return (
    <Stack spacing={2} sx={{ alignItems: 'flex-start' }}>
      <Alert severity="warning">That page does not exist.</Alert>
      <Button component={RouterLink} to="/">
        Back to applications
      </Button>
    </Stack>
  )
}

/** The list is the screen; the form routes open over it, so closing one reveals it again. */
function ApplicationsScreen() {
  return (
    <>
      <ApplicationsPage />
      <Outlet />
    </>
  )
}

export default function App() {
  const location = useLocation()

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        <Link component={RouterLink} to="/" underline="hover" color="inherit">
          Hardship applications
        </Link>
      </Typography>

      {/* Keyed by the path: the heading above stays clickable, and going anywhere else
          mounts a fresh boundary rather than keeping the failure on screen. */}
      <ErrorBoundary key={location.pathname}>
        <Routes>
          <Route element={<ApplicationsScreen />}>
            <Route path="/" element={null} />
            <Route path="/applications/new" element={<NewApplicationDialog />} />
            <Route path="/applications/:id/edit" element={<EditApplicationDialog />} />
          </Route>

          <Route path="*" element={<NotFound />} />
        </Routes>
      </ErrorBoundary>
    </Container>
  )
}
