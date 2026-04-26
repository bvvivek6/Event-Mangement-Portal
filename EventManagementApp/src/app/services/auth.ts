import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

export interface AuthAccount {
  id: string;
  firstname: string | null;
  lastname: string | null;
  email: string;
  role: string;
  phone?: string | null;
  bio?: string | null;
  organizationname?: string | null;
  profileimageurl?: string | null;
  bannerimageurl?: string | null;
  contactphone?: string | null;
  addressline?: string | null;
  city?: string | null;
  state?: string | null;
  country?: string | null;
  pincode?: string | null;
  website?: string | null;
  facebookurl?: string | null;
  twitterurl?: string | null;
  instagramurl?: string | null;
  linkedinurl?: string | null;
}

export interface AuthResponse {
  message: string;
  token?: string;
  role?: string;
  account?: AuthAccount;
}

export interface AuthRequest {
  email: string;
  password: string;
  role?: string;
}

export interface SignupRequest {
  firstname?: string;
  lastname?: string;
  email: string;
  password: string;
  phone?: string;
  bio?: string;
  organizationName?: string;
  profileImageUrl?: string;
  bannerImageUrl?: string;
  contactPhone?: string;
  addressLine?: string;
  city?: string;
  state?: string;
  country?: string;
  pincode?: string;
  website?: string;
  facebookUrl?: string;
  twitterUrl?: string;
  instagramUrl?: string;
  linkedInUrl?: string;
  role?: string;
  registrationKey?: string;
}

export interface AdminDashboardResponse {
  role: string;
  admin: AuthAccount;
  stats: {
    users: number;
    admins: number;
    organizers: number;
    events: number;
    pendingEvents: number;
  };
}

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private apiUrl = 'http://localhost:5247/api';

  constructor(private http: HttpClient) {}

  login(credentials: AuthRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, credentials);
  }

  signup(userData: SignupRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/signup`, userData);
  }

  getAdminDashboard(): Observable<AdminDashboardResponse> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.getToken()}`,
    });

    return this.http.get<AdminDashboardResponse>(`${this.apiUrl}/admin/dashboard`, {
      headers,
    });
  }

  getProfile(): Observable<any> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.getToken()}`,
    });
    return this.http.get(`${this.apiUrl}/UserProfile/profile`, { headers });
  }

  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  saveSession(response: AuthResponse): void {
    if (response.token) {
      this.saveToken(response.token);
    }

    if (response.role) {
      localStorage.setItem('role', response.role);
    }

    if (response.account) {
      localStorage.setItem('account', JSON.stringify(response.account));
    }
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getRole(): string | null {
    return localStorage.getItem('role') ?? this.getRoleFromToken();
  }

  getAccount(): AuthAccount | null {
    const account = localStorage.getItem('account');
    if (!account) {
      return null;
    }

    try {
      return JSON.parse(account) as AuthAccount;
    } catch {
      return null;
    }
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  hasRole(roles: string[]): boolean {
    const currentRole = this.getRole();
    return !!currentRole && roles.includes(currentRole);
  }

  logout(): Observable<AuthResponse> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.getToken()}`,
    });

    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/logout`, {}, { headers })
      .pipe(tap(() => this.clearSession()));
  }

  clearSession(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('account');
  }

  private getRoleFromToken(): string | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }

    const payload = this.decodeTokenPayload(token);
    if (!payload) {
      return null;
    }

    return (
      this.readTokenClaim(
        payload,
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
      ) ?? this.readTokenClaim(payload, 'role')
    );
  }

  private readTokenClaim(payload: Record<string, unknown>, key: string): string | null {
    const value = payload[key];
    return typeof value === 'string' ? value : null;
  }

  private decodeTokenPayload(token: string): Record<string, unknown> | null {
    const payloadPart = token.split('.')[1];
    if (!payloadPart) {
      return null;
    }

    const normalizedPayload = payloadPart.replace(/-/g, '+').replace(/_/g, '/');
    const paddedPayload = normalizedPayload.padEnd(
      normalizedPayload.length + ((4 - (normalizedPayload.length % 4)) % 4),
      '=',
    );

    try {
      return JSON.parse(atob(paddedPayload)) as Record<string, unknown>;
    } catch {
      return null;
    }
  }
}
