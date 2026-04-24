import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-signup',
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './signup.html',
  styleUrl: './signup.css',
})
export class Signup {
  signupData = {
    firstname: '',
    lastname: '',
    email: '',
    password: '',
    phone: '',
  };
  errorMessage = '';

  constructor(
    private authService: Auth,
    private router: Router,
  ) {}

  onSubmit() {
    this.authService.signup(this.signupData).subscribe({
      next: (res) => {
        console.log('Signup success', res);
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorMessage = 'Failed to sign up';
        console.error('Signup error', err);
      },
    });
  }
}
