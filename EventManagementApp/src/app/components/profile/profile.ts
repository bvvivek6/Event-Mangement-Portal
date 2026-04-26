import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-profile',
  imports: [CommonModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile implements OnInit {
  userProfile: any = null;
  errorMessage = '';
  successMessage = '';
  loading: boolean = true;
  currentRole = '';

  constructor(
    private authService: Auth,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.currentRole = this.authService.getRole() ?? '';
    this.loadProfile();
  }

  loadProfile(): void {
    this.authService.getProfile().subscribe({
      next: (data) => {
        console.log('Profile data', data);
        this.userProfile = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load profile', err);
        this.errorMessage = err?.error?.message ?? 'Failed to load profile';
        this.loading = false;
        this.router.navigate(['/login']);
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
