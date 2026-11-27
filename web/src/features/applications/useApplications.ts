import { useMutation, useQueryClient } from '@tanstack/react-query'
import { applicationsApi, type HardshipApplicationRequest } from '../../api/client'

export const applicationKeys = {
  all: () => ['applications'] as const,
  detail: (id: string) => ['applications', id] as const,
}

export function useCreateApplication() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: HardshipApplicationRequest) => applicationsApi.create(request),
    // Invalidate rather than write the new record into the cache by hand: the server
    // decides the id, the timestamps and the surplus.
    onSuccess: () => queryClient.invalidateQueries({ queryKey: applicationKeys.all() }),
  })
}
