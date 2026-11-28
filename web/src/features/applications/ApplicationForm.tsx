import { Alert, Box, Button, Stack } from '@mui/material'
import { Form, Formik, type FormikHelpers } from 'formik'
import { ApiError, type HardshipApplicationRequest } from '../../api/client'
import { FormTextField } from '../../components/form/FormTextField'
import {
  type ApplicationFormValues,
  applicationSchema,
  toRequest,
} from './applicationSchema'

type Props = {
  initialValues: ApplicationFormValues
  submitLabel: string
  onSubmit: (request: HardshipApplicationRequest) => Promise<unknown>
}

const FORM_FIELDS = [
  'customerName',
  'dateOfBirth',
  'monthlyIncome',
  'monthlyExpenses',
  'hardshipReason',
] as const

/**
 * Splits the server's field errors into the ones this form can show against a field and the
 * ones it cannot, so nothing the server said is silently dropped.
 */
function splitServerErrors(error: ApiError) {
  const onFields: Partial<Record<string, string>> = {}
  const elsewhere: string[] = []

  for (const [field, messages] of Object.entries(error.fieldErrors)) {
    if ((FORM_FIELDS as readonly string[]).includes(field)) {
      onFields[field] = messages.join(' ')
    } else {
      elsewhere.push(...messages)
    }
  }

  return { onFields, elsewhere }
}

export function ApplicationForm({ initialValues, submitLabel, onSubmit }: Props) {
  async function submit(
    values: ApplicationFormValues,
    helpers: FormikHelpers<ApplicationFormValues>,
  ): Promise<void> {
    helpers.setStatus(undefined)

    try {
      await onSubmit(toRequest(values))
    } catch (error) {
      if (!(error instanceof ApiError)) {
        helpers.setStatus('Something went wrong. Please try again.')
        return
      }

      const { onFields, elsewhere } = splitServerErrors(error)

      if (Object.keys(onFields).length > 0) {
        helpers.setErrors(onFields)
      }

      if (elsewhere.length > 0 || Object.keys(onFields).length === 0) {
        helpers.setStatus([error.message, ...elsewhere].join(' '))
      }
    } finally {
      helpers.setSubmitting(false)
    }
  }

  return (
    <Formik
      initialValues={initialValues}
      validationSchema={applicationSchema}
      // The edit form receives its values after the record has loaded.
      enableReinitialize
      onSubmit={submit}
    >
      {({ isSubmitting, status }) => (
        <Form noValidate>
          <Stack spacing={2} sx={{ maxWidth: 520 }}>
            {typeof status === 'string' && <Alert severity="error">{status}</Alert>}

            <FormTextField name="customerName" label="Customer name" required />

            <FormTextField
              name="dateOfBirth"
              label="Date of birth"
              type="date"
              required
              slotProps={{ inputLabel: { shrink: true } }}
            />

            <FormTextField
              name="monthlyIncome"
              label="Monthly income"
              type="number"
              required
              hint="In AUD, before tax."
              slotProps={{ input: { startAdornment: <Box sx={{ mr: 1 }}>$</Box> } }}
            />

            <FormTextField
              name="monthlyExpenses"
              label="Monthly expenses"
              type="number"
              required
              hint="In AUD. It is fine for this to exceed your income."
              slotProps={{ input: { startAdornment: <Box sx={{ mr: 1 }}>$</Box> } }}
            />

            <FormTextField
              name="hardshipReason"
              label="Reason for hardship"
              multiline
              minRows={3}
              hint="Optional."
            />

            <Box>
              <Button type="submit" variant="contained" disabled={isSubmitting}>
                {isSubmitting ? 'Saving…' : submitLabel}
              </Button>
            </Box>
          </Stack>
        </Form>
      )}
    </Formik>
  )
}
