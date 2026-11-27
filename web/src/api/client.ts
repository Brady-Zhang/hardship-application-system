import type { components } from './schema'

export type HardshipApplication = components['schemas']['HardshipApplicationResponse']
export type HardshipApplicationRequest = components['schemas']['HardshipApplicationRequest']

type ProblemDetails = components['schemas']['ProblemDetails']
type ValidationProblemDetails = components['schemas']['ValidationProblemDetails']

/** Field name to the messages for it, in the casing the form uses. */
export type FieldErrors = Record<string, string[]>

export class ApiError extends Error {
  readonly status: number
  readonly fieldErrors: FieldErrors

  constructor(status: number, message: string, fieldErrors: FieldErrors = {}) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.fieldErrors = fieldErrors
  }

  get hasFieldErrors(): boolean {
    return Object.keys(this.fieldErrors).length > 0
  }
}

const UNREACHABLE = 'Could not reach the server. Check your connection and try again.'
const UNEXPECTED = 'Something went wrong. Please try again.'

// The API names fields as the C# properties are named; the form names them as the JSON does.
function toFieldName(key: string): string {
  return key.length === 0 ? key : key[0].toLowerCase() + key.slice(1)
}

function toFieldErrors(errors: ValidationProblemDetails['errors']): FieldErrors {
  return Object.fromEntries(
    Object.entries(errors ?? {}).map(([key, messages]) => [toFieldName(key), messages]),
  )
}

function toMessage(problem: ProblemDetails | null, status: number): string {
  return problem?.detail ?? problem?.title ?? `${UNEXPECTED} (${status})`
}

async function readProblem(response: Response): Promise<ValidationProblemDetails | null> {
  try {
    return (await response.json()) as ValidationProblemDetails
  } catch {
    // A proxy or a crash can answer with something that is not problem details at all.
    return null
  }
}

async function request<T>(path: string, init?: { method: string; body: unknown }): Promise<T> {
  let response: Response

  try {
    response = await fetch(path, {
      method: init?.method ?? 'GET',
      headers: init ? { 'Content-Type': 'application/json' } : undefined,
      body: init ? JSON.stringify(init.body) : undefined,
    })
  } catch {
    throw new ApiError(0, UNREACHABLE)
  }

  if (!response.ok) {
    const problem = await readProblem(response)
    throw new ApiError(response.status, toMessage(problem, response.status), toFieldErrors(problem?.errors))
  }

  return (await response.json()) as T
}

export const applicationsApi = {
  list: () => request<HardshipApplication[]>('/api/applications'),

  get: (id: string) => request<HardshipApplication>(`/api/applications/${id}`),

  create: (body: HardshipApplicationRequest) =>
    request<HardshipApplication>('/api/applications', { method: 'POST', body }),

  update: (id: string, body: HardshipApplicationRequest) =>
    request<HardshipApplication>(`/api/applications/${id}`, { method: 'PUT', body }),
}
