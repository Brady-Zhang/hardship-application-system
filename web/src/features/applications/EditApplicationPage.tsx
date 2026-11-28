import { Button, Stack, Typography } from '@mui/material'
import { Link as RouterLink, useNavigate, useParams } from 'react-router-dom'
import { isNotFound } from '../../api/errors'
import { LoadFailed, Loading } from '../../components/QueryStates'
import { ApplicationForm } from './ApplicationForm'
import { toFormValues } from './applicationSchema'
import { useApplication, useUpdateApplication } from './useApplications'

export function EditApplicationPage() {
  const { id = '' } = useParams<{ id: string }>()
  const application = useApplication(id)
  const updateApplication = useUpdateApplication(id)
  const navigate = useNavigate()

  return (
    <Stack spacing={2} sx={{ alignItems: 'flex-start' }}>
      <Typography variant="h5" component="h2">
        Edit application
      </Typography>

      {application.isPending && <Loading label="Loading application" />}

      {application.isError && (
        <LoadFailed
          error={application.error}
          onRetry={isNotFound(application.error) ? undefined : () => void application.refetch()}
        />
      )}

      {/* The form is mounted only once the record has arrived, so it never renders empty and
          then fills itself in. `enableReinitialize` still matters: saving replaces the values. */}
      {application.isSuccess && (
        <ApplicationForm
          initialValues={toFormValues(application.data)}
          submitLabel="Save changes"
          onSubmit={async (request) => {
            const updated = await updateApplication.mutateAsync(request)
            navigate('/', { state: { flash: `Changes saved for ${updated.customerName}` } })
          }}
        />
      )}

      <Button component={RouterLink} to="/">
        Back to applications
      </Button>
    </Stack>
  )
}
