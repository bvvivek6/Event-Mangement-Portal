import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { Auth } from '../services/auth';

export const roleGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const authService = inject(Auth);
  const router = inject(Router);
  const allowedRoles = (route.data['roles'] as string[] | undefined) ?? [];

  if (allowedRoles.length === 0 || authService.hasRole(allowedRoles)) {
    return true;
  }

  return router.createUrlTree(authService.isLoggedIn() ? ['/home'] : ['/login'], {
    queryParams: { returnUrl: state.url },
  });
};
