import { describe, expect, it } from 'vitest'
import router from '../router'

describe('Application routes', () => {
  it('registers all main screens required by the assignment', () => {
    const paths = router.getRoutes().map((route) => route.path)

    expect(paths).toEqual(
      expect.arrayContaining(['/customers', '/goods', '/orders', '/reports', '/processing']),
    )
  })
})
