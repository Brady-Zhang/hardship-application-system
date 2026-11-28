import { Alert, Snackbar, Typography } from '@mui/material'
import { useState } from 'react'
import { ApplicationForm } from './ApplicationForm'
import { emptyApplication } from './applicationSchema'
import { useCreateApplication } from './useApplications'

export function NewApplicationPage() {
  const createApplication = useCreateApplication()
  const [saved, setSaved] = useState<string | null>(null)

  return (
    <>
      <Typography variant="h5" component="h2" gutterBottom>
        New hardship application
      </Typography>

      <ApplicationForm
        initialValues={emptyApplication}
        submitLabel="Submit application"
        onSubmit={async (request) => {
          const created = await createApplication.mutateAsync(request)
          setSaved(created.customerName)
        }}
      />

      <Snackbar
        open={saved !== null}
        autoHideDuration={4000}
        onClose={() => setSaved(null)}
        message={`Application saved for ${saved}`}
      >
        <Alert severity="success" onClose={() => setSaved(null)}>
          {`Application saved for ${saved}`}
        </Alert>
      </Snackbar>
    </>
  )
}
