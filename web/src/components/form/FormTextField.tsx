import { TextField, type TextFieldProps } from '@mui/material'
import { useField } from 'formik'

type Props = {
  name: string
  label: string
  hint?: string
} & Omit<TextFieldProps, 'name' | 'label' | 'error' | 'helperText' | 'value' | 'onChange' | 'onBlur'>

/**
 * A MUI text field wired to Formik. The error is shown once the field has been visited,
 * so the form does not shout at someone who is still filling it in.
 */
export function FormTextField({ name, label, hint, ...props }: Props) {
  const [field, meta] = useField<string>(name)
  const invalid = meta.touched && Boolean(meta.error)

  return (
    <TextField
      {...field}
      {...props}
      id={name}
      label={label}
      error={invalid}
      // Reserve the line whether or not there is an error, so the layout does not jump.
      helperText={invalid ? meta.error : (hint ?? ' ')}
      fullWidth
    />
  )
}
