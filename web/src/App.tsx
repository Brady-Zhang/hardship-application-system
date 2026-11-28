import { Alert, Button, Container, Link, Stack, Typography } from '@mui/material'
import { Link as RouterLink, Route, Routes } from 'react-router-dom'
import { ApplicationsPage } from './features/applications/ApplicationsPage'
import { EditApplicationPage } from './features/applications/EditApplicationPage'
import { NewApplicationPage } from './features/applications/NewApplicationPage'

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

export default function App() {
  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        <Link component={RouterLink} to="/" underline="hover" color="inherit">
          Hardship applications
        </Link>
      </Typography>

      <Routes>
        <Route path="/" element={<ApplicationsPage />} />
        <Route path="/applications/new" element={<NewApplicationPage />} />
        <Route path="/applications/:id/edit" element={<EditApplicationPage />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Container>
  )
}
