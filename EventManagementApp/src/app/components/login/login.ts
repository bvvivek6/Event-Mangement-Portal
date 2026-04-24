import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginData = { email: '', password: '' };
  errorMessage = '';

  constructor(
    private authService: Auth,
    private router: Router,
  ) {}

  onSubmit() {
    this.authService.login(this.loginData).subscribe({
      next: (res) => {
        // Handle successful login, store token, etc.
        if (res && res.token) {
          this.authService.saveToken(res.token);
        }
        console.log('Login success', res);
        this.router.navigate(['/profile']);
      },
      error: (err) => {
        this.errorMessage = 'Invalid email or password';
        console.error('Login error', err);
      },
    });
  }
}
