import { CssBaseline, ThemeProvider, createTheme } from '@mui/material'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'

// The stock MUI theme. Nothing here needs a brand, and an unmodified theme is one less
// thing between the reviewer and the code.
const theme = createTheme()

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      // A 4xx is an answer, not a hiccup; retrying it just makes the user wait longer.
      retry: false,
    },
  },
})

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <QueryClientProvider client={queryClient}>
        <App />
      </QueryClientProvider>
    </ThemeProvider>
  </StrictMode>,
)
