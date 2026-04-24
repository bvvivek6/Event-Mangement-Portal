import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private apiUrl = 'http://localhost:5247/api';

  constructor(private http: HttpClient) {}

  login(credentials: Object): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/login`, credentials);
  }

  signup(userData: Object): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/signup`, userData);
  }

  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout(): void {
    localStorage.removeItem('token');
  }

  getProfile(): Observable<any> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.getToken()}`,
    });
    return this.http.get(`${this.apiUrl}/UserProfile/profile`, { headers });
  }
}
