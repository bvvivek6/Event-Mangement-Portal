import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Auth, AuthResponse } from '../../services/auth';

@Component({
  selector: 'app-signup',
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './signup.html',
  styleUrl: './signup.css',
})
export class Signup implements OnInit {
  signupData = {
    firstname: '',
    lastname: '',
    email: '',
    bio: '',
    password: '',
    phone: '',
    role: 'User',
    registrationKey: '',
    organizationName: '',
    profileImageUrl: '',
    bannerImageUrl: '',
    contactPhone: '',
    addressLine: '',
    city: '',
    state: '',
    country: '',
    pincode: '',
    website: '',
    facebookUrl: '',
    twitterUrl: '',
    instagramUrl: '',
    linkedInUrl: '',
  };
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
      this.signupData.role = routeRole;
    } else {
      this.signupData.role = 'User';
    }
  }

  onSubmit() {
    this.authService.signup(this.signupData).subscribe({
      next: (res: AuthResponse) => {
        this.authService.saveSession(res);
        console.log('Signup success', res);
        this.router.navigate(this.getRedirectRoute(res.role ?? this.signupData.role));
      },
      error: (err) => {
        this.errorMessage = err?.error?.message ?? 'Failed to sign up';
        console.error('Signup error', err);
      },
    });
  }

  private getRedirectRoute(role?: string): string[] {
    if (role === 'Admin') {
      return ['/admin/infosys/eventmanagementportal-4819/dashboard'];
    }

    if (role === 'Organizer') {
      return ['/organizer/infosys/eventmanagementportal-7426/dashboard'];
    }

    return ['/profile'];
  }
}
