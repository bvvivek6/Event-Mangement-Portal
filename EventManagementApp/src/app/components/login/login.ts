import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Auth, AuthResponse } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login implements OnInit {
  loginData = { email: '', password: '', role: 'User' };
  presetRole: string | null = null;
  showRoleSelector = true;
  errorMessage = '';

  constructor(
    private authService: Auth,
    private router: Router,
    private route: ActivatedRoute,
  ) {}

  ngOnInit(): void {
    const routeRole = this.route.snapshot.data['role'] as string | undefined;
    if (routeRole === 'Admin' || routeRole === 'Organizer') {
      this.presetRole = routeRole;
      this.showRoleSelector = false;
      this.loginData.role = routeRole;
    } else {
      this.loginData.role = 'User';
    }
  }

  onSubmit() {
    this.authService.login(this.loginData).subscribe({
      next: (res: AuthResponse) => {
        this.authService.saveSession(res);
        const role = res.role ?? this.authService.getRole() ?? 'User';
        console.log('Login success', res);
        this.router.navigate(this.getRedirectRoute(role));
      },
      error: (err) => {
        this.errorMessage = err?.error?.message ?? 'Invalid email or password';
        console.error('Login error', err);
      },
    });
  }

  private getRedirectRoute(role: string): string[] {
    if (role === 'Admin') {
      return ['/admin/infosys/eventmanagementportal-4819/dashboard'];
    }

    if (role === 'Organizer') {
      return ['/organizer/infosys/eventmanagementportal-7426/dashboard'];
    }

    return ['/profile'];
  }
}
