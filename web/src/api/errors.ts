import { ApiError } from './client'

/** An ApiError already carries a message meant for the user; anything else does not. */
export function messageFor(error: unknown): string {
  return error instanceof ApiError ? error.message : 'Something went wrong. Please try again.'
}

export function isNotFound(error: unknown): boolean {
  return error instanceof ApiError && error.status === 404
}
