import { useNavigate } from 'react-router-dom'
import { FormDialog } from '../../components/FormDialog'
import { ApplicationForm } from './ApplicationForm'
import { emptyApplication } from './applicationSchema'
import { useCreateApplication } from './useApplications'

const FORM_ID = 'new-application-form'

export function NewApplicationDialog() {
  const createApplication = useCreateApplication()
  const navigate = useNavigate()

  return (
    <FormDialog
      title="New hardship application"
      onClose={() => navigate('/')}
      form={{ id: FORM_ID, submitLabel: 'Submit application', saving: createApplication.isPending }}
    >
      <ApplicationForm
        id={FORM_ID}
        initialValues={emptyApplication}
        onSubmit={async (request) => {
          const created = await createApplication.mutateAsync(request)
          navigate('/', { state: { flash: `Application saved for ${created.customerName}` } })
        }}
      />
    </FormDialog>
  )
}
