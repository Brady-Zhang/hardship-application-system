import { Button, Stack, Typography } from '@mui/material'
import { Link as RouterLink, useNavigate } from 'react-router-dom'
import { ApplicationForm } from './ApplicationForm'
import { emptyApplication } from './applicationSchema'
import { useCreateApplication } from './useApplications'

export function NewApplicationPage() {
  const createApplication = useCreateApplication()
  const navigate = useNavigate()

  return (
    <Stack spacing={2} sx={{ alignItems: 'flex-start' }}>
      <Typography variant="h5" component="h2">
        New hardship application
      </Typography>

      <ApplicationForm
        initialValues={emptyApplication}
        submitLabel="Submit application"
        onSubmit={async (request) => {
          const created = await createApplication.mutateAsync(request)
          navigate('/', { state: { flash: `Application saved for ${created.customerName}` } })
        }}
      />

      <Button component={RouterLink} to="/">
        Back to applications
      </Button>
    </Stack>
  )
}
