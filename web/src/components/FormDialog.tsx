import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from '@mui/material'
import type { ReactNode } from 'react'

type Props = {
  title: string
  onClose: () => void
  children: ReactNode
  /**
   * The form these actions submit. The buttons sit in `DialogActions`, outside the form,
   * and reach it through the HTML `form` attribute — so they stay pinned to the bottom of
   * the dialog instead of scrolling away with the fields.
   *
   * Left out while there is no form to submit yet, and the dialog offers only Close.
   */
  form?: { id: string; submitLabel: string; saving: boolean }
}

export function FormDialog({ title, onClose, children, form }: Props) {
  const titleId = 'form-dialog-title'

  return (
    <Dialog open onClose={onClose} fullWidth maxWidth="sm" aria-labelledby={titleId}>
      <DialogTitle id={titleId}>{title}</DialogTitle>

      <DialogContent dividers>{children}</DialogContent>

      <DialogActions>
        <Button onClick={onClose} disabled={form?.saving}>
          {form ? 'Cancel' : 'Close'}
        </Button>

        {form && (
          <Button type="submit" form={form.id} variant="contained" disabled={form.saving}>
            {form.saving ? 'Saving…' : form.submitLabel}
          </Button>
        )}
      </DialogActions>
    </Dialog>
  )
}
