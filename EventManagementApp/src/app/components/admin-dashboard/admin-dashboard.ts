import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { Auth, AdminDashboardResponse } from '../../services/auth';

@Component({
  selector: 'app-admin-dashboard',
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard implements OnInit {
  dashboard: AdminDashboardResponse | null = null;
  errorMessage = '';
  loading = true;

  constructor(
    private authService: Auth,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.authService.getAdminDashboard().subscribe({
      next: (data) => {
        this.dashboard = data;
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = 'You do not have access to this area.';
        if (error?.status === 401 || error?.status === 403) {
          this.router.navigate(['/login']);
        }
      },
    });
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login']),
    });
  }
}
