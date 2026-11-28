import { Container, Typography } from '@mui/material'
import { NewApplicationPage } from './features/applications/NewApplicationPage'

export default function App() {
  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Hardship applications
      </Typography>

      <NewApplicationPage />
    </Container>
  )
}
