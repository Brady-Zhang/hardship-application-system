import {
  Alert,
  Button,
  Paper,
  Snackbar,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material'
import { Link as RouterLink } from 'react-router-dom'
import { Loading, LoadFailed } from '../../components/QueryStates'
import { useFlashMessage } from '../../components/useFlashMessage'
import { formatDay, formatInstant, formatMoney } from './formatting'
import { useApplications } from './useApplications'

// Red on its own is not a label. The amount is already negative, and this says so out loud
// for anyone who cannot see the colour.
const visuallyHidden = {
  position: 'absolute',
  width: 1,
  height: 1,
  overflow: 'hidden',
  clip: 'rect(0 0 0 0)',
  whiteSpace: 'nowrap',
} as const

export function ApplicationsPage() {
  const applications = useApplications()
  const [flash, dismissFlash] = useFlashMessage()

  return (
    <Stack spacing={2}>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h5" component="h2">
          Applications
        </Typography>

        <Button component={RouterLink} to="/applications/new" variant="contained">
          New application
        </Button>
      </Stack>

      {applications.isPending && <Loading label="Loading applications" />}

      {applications.isError && (
        <LoadFailed error={applications.error} onRetry={() => void applications.refetch()} />
      )}

      {applications.data?.length === 0 && (
        <Alert severity="info">No applications yet. Submit the first one.</Alert>
      )}

      {applications.data !== undefined && applications.data.length > 0 && (
        <TableContainer component={Paper} variant="outlined">
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Customer</TableCell>
                <TableCell>Date of birth</TableCell>
                <TableCell align="right">Monthly income</TableCell>
                <TableCell align="right">Monthly expenses</TableCell>
                <TableCell align="right">Surplus</TableCell>
                <TableCell>Last updated</TableCell>
                <TableCell align="right">Edit</TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {applications.data.map((application) => (
                <TableRow key={application.id} hover>
                  <TableCell>{application.customerName}</TableCell>
                  <TableCell>{formatDay(application.dateOfBirth)}</TableCell>
                  <TableCell align="right">{formatMoney(application.monthlyIncome)}</TableCell>
                  <TableCell align="right">{formatMoney(application.monthlyExpenses)}</TableCell>
                  <TableCell
                    align="right"
                    sx={{
                      color: application.isInDeficit ? 'error.main' : undefined,
                      fontWeight: application.isInDeficit ? 600 : undefined,
                    }}
                  >
                    {formatMoney(application.monthlySurplus)}
                    {application.isInDeficit && (
                      <Typography component="span" sx={visuallyHidden}>
                        {' in deficit'}
                      </Typography>
                    )}
                  </TableCell>
                  <TableCell>
                    {formatInstant(application.updatedAt ?? application.createdAt)}
                  </TableCell>
                  <TableCell align="right">
                    <Button
                      component={RouterLink}
                      to={`/applications/${application.id}/edit`}
                      size="small"
                      aria-label={`Edit ${application.customerName}`}
                    >
                      Edit
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Snackbar open={flash !== null} autoHideDuration={4000} onClose={dismissFlash}>
        <Alert severity="success" onClose={dismissFlash}>
          {flash}
        </Alert>
      </Snackbar>
    </Stack>
  )
}
