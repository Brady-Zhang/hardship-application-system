import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  applicationsApi,
  type HardshipApplication,
  type HardshipApplicationRequest,
} from '../../api/client'

export const applicationKeys = {
  all: ['applications'] as const,
  list: () => [...applicationKeys.all, 'list'] as const,
  detail: (id: string) => [...applicationKeys.all, 'detail', id] as const,
}

export function useApplications() {
  return useQuery({
    queryKey: applicationKeys.list(),
    queryFn: () => applicationsApi.list(),
  })
}

export function useApplication(id: string) {
  return useQuery({
    queryKey: applicationKeys.detail(id),
    queryFn: () => applicationsApi.get(id),
  })
}

export function useCreateApplication() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: HardshipApplicationRequest) => applicationsApi.create(request),
    // Invalidate rather than write the new record into the cache by hand: the server
    // decides the id, the timestamps and the surplus.
    onSuccess: () => queryClient.invalidateQueries({ queryKey: applicationKeys.all }),
  })
}

export function useUpdateApplication(id: string) {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (request: HardshipApplicationRequest) => applicationsApi.update(id, request),
    onSuccess: (updated: HardshipApplication) => {
      queryClient.setQueryData(applicationKeys.detail(id), updated)
      return queryClient.invalidateQueries({ queryKey: applicationKeys.list() })
    },
  })
}
