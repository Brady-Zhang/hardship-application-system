import { afterEach, describe, expect, it, vi } from 'vitest'
import { ApiError, applicationsApi } from './client'

function respondWith(body: unknown, status = 200): void {
  vi.stubGlobal(
    'fetch',
    vi.fn().mockResolvedValue({
      ok: status >= 200 && status < 300,
      status,
      json: async () => body,
    } as Response),
  )
}

afterEach(() => vi.unstubAllGlobals())

describe('applicationsApi', () => {
  it('returns the parsed body on success', async () => {
    respondWith([{ id: 'a', customerName: 'Alex Taylor' }])

    const applications = await applicationsApi.list()

    expect(applications).toHaveLength(1)
    expect(applications[0].customerName).toBe('Alex Taylor')
  })

  it('sends the body as JSON on create', async () => {
    respondWith({ id: 'a' }, 201)

    await applicationsApi.create({
      customerName: 'Alex Taylor',
      dateOfBirth: '1990-04-17',
      monthlyIncome: 5000,
      monthlyExpenses: 3200,
      hardshipReason: null,
    })

    const [path, init] = vi.mocked(fetch).mock.calls[0]
    expect(path).toBe('/api/applications')
    expect(init?.method).toBe('POST')
    expect(JSON.parse(init?.body as string).customerName).toBe('Alex Taylor')
  })

  it('turns a validation problem into field errors the form can use', async () => {
    respondWith(
      {
        title: 'One or more validation errors occurred.',
        status: 400,
        errors: {
          CustomerName: ['Customer name is required.'],
          MonthlyIncome: ['Monthly income cannot be negative.'],
        },
      },
      400,
    )

    const error = await applicationsApi.list().catch((caught: unknown) => caught)

    expect(error).toBeInstanceOf(ApiError)
    const apiError = error as ApiError
    expect(apiError.status).toBe(400)
    expect(apiError.hasFieldErrors).toBe(true)
    // The API says CustomerName; the form field is customerName.
    expect(apiError.fieldErrors).toEqual({
      customerName: ['Customer name is required.'],
      monthlyIncome: ['Monthly income cannot be negative.'],
    })
  })

  it('uses the problem detail as the message when there are no field errors', async () => {
    respondWith(
      { title: 'Application not found', detail: 'No hardship application exists with that id.', status: 404 },
      404,
    )

    const error = (await applicationsApi.get('missing').catch((caught: unknown) => caught)) as ApiError

    expect(error.status).toBe(404)
    expect(error.message).toBe('No hardship application exists with that id.')
    expect(error.hasFieldErrors).toBe(false)
  })

  it('falls back to a readable message when the body is not problem details', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: false,
        status: 502,
        json: async () => {
          throw new SyntaxError('Unexpected token < in JSON')
        },
      } as unknown as Response),
    )

    const error = (await applicationsApi.list().catch((caught: unknown) => caught)) as ApiError

    expect(error.status).toBe(502)
    expect(error.message).toContain('Something went wrong')
  })

  it('reports an unreachable server rather than throwing a fetch error', async () => {
    vi.stubGlobal('fetch', vi.fn().mockRejectedValue(new TypeError('Failed to fetch')))

    const error = (await applicationsApi.list().catch((caught: unknown) => caught)) as ApiError

    expect(error).toBeInstanceOf(ApiError)
    expect(error.status).toBe(0)
    expect(error.message).toContain('Could not reach the server')
  })
})
