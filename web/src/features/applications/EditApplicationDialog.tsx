import { useNavigate, useParams } from 'react-router-dom'
import { isNotFound } from '../../api/errors'
import { FormDialog } from '../../components/FormDialog'
import { LoadFailed, Loading } from '../../components/QueryStates'
import { ApplicationForm } from './ApplicationForm'
import { toFormValues } from './applicationSchema'
import { useApplication, useUpdateApplication } from './useApplications'

const FORM_ID = 'edit-application-form'

export function EditApplicationDialog() {
  const { id = '' } = useParams<{ id: string }>()
  const application = useApplication(id)
  const updateApplication = useUpdateApplication(id)
  const navigate = useNavigate()

  return (
    <FormDialog
      title="Edit application"
      onClose={() => navigate('/')}
      // No submit button until there is something to submit.
      form={
        application.isSuccess
          ? { id: FORM_ID, submitLabel: 'Save changes', saving: updateApplication.isPending }
          : undefined
      }
    >
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
          id={FORM_ID}
          initialValues={toFormValues(application.data)}
          onSubmit={async (request) => {
            const updated = await updateApplication.mutateAsync(request)
            navigate('/', { state: { flash: `Changes saved for ${updated.customerName}` } })
          }}
        />
      )}
    </FormDialog>
  )
}
