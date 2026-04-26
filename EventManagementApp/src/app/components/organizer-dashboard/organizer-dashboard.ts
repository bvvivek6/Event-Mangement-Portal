import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-organizer-dashboard',
  imports: [CommonModule, RouterModule],
  templateUrl: './organizer-dashboard.html',
  styleUrl: './organizer-dashboard.css',
})
export class OrganizerDashboard implements OnInit {
  profile: any = null;
  errorMessage = '';
  loading = true;

  constructor(
    private authService: Auth,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.authService.getProfile().subscribe({
      next: (data) => {
        this.profile = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'You do not have access to this area.';
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
