import '@testing-library/jest-dom/vitest'
import { cleanup } from '@testing-library/react'
import { afterEach } from 'vitest'

// Testing Library only auto-cleans when Vitest runs with globals. This project imports its
// test functions explicitly, so without this the DOM accumulates between tests and queries
// start matching elements left over from the previous one.
afterEach(cleanup)
