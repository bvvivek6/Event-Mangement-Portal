import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Signup } from './components/signup/signup';
import { Home } from './components/home/home';
import { Profile } from './components/profile/profile';
import { AdminDashboard } from './components/admin-dashboard/admin-dashboard';
import { OrganizerDashboard } from './components/organizer-dashboard/organizer-dashboard';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  {
    path: 'admin/infosys/eventmanagementportal-4819/login',
    component: Login,
    data: { role: 'Admin' },
  },
  {
    path: 'organizer/infosys/eventmanagementportal-7426/login',
    component: Login,
    data: { role: 'Organizer' },
  },
  { path: 'signup', component: Signup },
  {
    path: 'admin/infosys/eventmanagementportal-4819/register',
    component: Signup,
    data: { role: 'Admin' },
  },
  {
    path: 'organizer/infosys/eventmanagementportal-7426/register',
    component: Signup,
    data: { role: 'Organizer' },
  },
  { path: 'home', component: Home, canActivate: [authGuard] },
  { path: 'profile', component: Profile, canActivate: [authGuard] },
  {
    path: 'admin/infosys/eventmanagementportal-4819/dashboard',
    component: AdminDashboard,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'organizer/infosys/eventmanagementportal-7426/dashboard',
    component: OrganizerDashboard,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Organizer'] },
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
];
