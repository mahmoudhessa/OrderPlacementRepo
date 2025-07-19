import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ApiBaseService {
  private baseUrls = ['http://localhost:5001', 'http://localhost:5000'];

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string, options?: any): Observable<T> {
    const opts1 = options ? { ...options } : undefined;
    const opts2 = options ? { ...options } : undefined;
    if (opts1 && opts1.body) delete opts1.body;
    if (opts2 && opts2.body) delete opts2.body;
    return this.tryAllGet<T>(endpoint, opts1, opts2);
  }

  post<T>(endpoint: string, body: any, options?: any): Observable<T> {
    return this.tryAllPost<T>(endpoint, body, options, options);
  }

  put<T>(endpoint: string, body: any, options?: any): Observable<T> {
    return this.tryAllPut<T>(endpoint, body, options, options);
  }

  delete<T>(endpoint: string, options?: any): Observable<T> {
    const opts1 = options ? { ...options } : undefined;
    const opts2 = options ? { ...options } : undefined;
    if (opts1 && opts1.body) delete opts1.body;
    if (opts2 && opts2.body) delete opts2.body;
    return this.tryAllDelete<T>(endpoint, opts1, opts2);
  }

  private tryAllGet<T>(endpoint: string, options1: any, options2: any): Observable<T> {
    const url1 = this.baseUrls[0];
    const url2 = this.baseUrls[1];
    const call1 = options1 ? this.http.get<T>(`${url1}${endpoint}`, options1) : this.http.get<T>(`${url1}${endpoint}`);
    const call2 = options2 ? this.http.get<T>(`${url2}${endpoint}`, options2) : this.http.get<T>(`${url2}${endpoint}`);
    return (call1 as Observable<T>).pipe(
      catchError(err1 => (call2 as Observable<T>).pipe(
        catchError(err2 => {
          if (err1.status === 0 && err2.status === 0) {
            return throwError(() => ({ backendDown: true }));
          }
          return throwError(() => err1.status === 0 ? err2 : err1);
        })
      ))
    );
  }

  private tryAllPost<T>(endpoint: string, body: any, options1: any, options2: any): Observable<T> {
    const url1 = this.baseUrls[0];
    const url2 = this.baseUrls[1];
    const call1 = options1 ? this.http.post<T>(`${url1}${endpoint}`, body, options1) : this.http.post<T>(`${url1}${endpoint}`, body);
    const call2 = options2 ? this.http.post<T>(`${url2}${endpoint}`, body, options2) : this.http.post<T>(`${url2}${endpoint}`, body);
    return (call1 as Observable<T>).pipe(
      catchError(err1 => (call2 as Observable<T>).pipe(
        catchError(err2 => {
          if (err1.status === 0 && err2.status === 0) {
            return throwError(() => ({ backendDown: true }));
          }
          return throwError(() => err1.status === 0 ? err2 : err1);
        })
      ))
    );
  }

  private tryAllPut<T>(endpoint: string, body: any, options1: any, options2: any): Observable<T> {
    const url1 = this.baseUrls[0];
    const url2 = this.baseUrls[1];
    const call1 = options1 ? this.http.put<T>(`${url1}${endpoint}`, body, options1) : this.http.put<T>(`${url1}${endpoint}`, body);
    const call2 = options2 ? this.http.put<T>(`${url2}${endpoint}`, body, options2) : this.http.put<T>(`${url2}${endpoint}`, body);
    return (call1 as Observable<T>).pipe(
      catchError(err1 => (call2 as Observable<T>).pipe(
        catchError(err2 => {
          if (err1.status === 0 && err2.status === 0) {
            return throwError(() => ({ backendDown: true }));
          }
          return throwError(() => err1.status === 0 ? err2 : err1);
        })
      ))
    );
  }

  private tryAllDelete<T>(endpoint: string, options1: any, options2: any): Observable<T> {
    const url1 = this.baseUrls[0];
    const url2 = this.baseUrls[1];
    const call1 = options1 ? this.http.delete<T>(`${url1}${endpoint}`, options1) : this.http.delete<T>(`${url1}${endpoint}`);
    const call2 = options2 ? this.http.delete<T>(`${url2}${endpoint}`, options2) : this.http.delete<T>(`${url2}${endpoint}`);
    return (call1 as Observable<T>).pipe(
      catchError(err1 => (call2 as Observable<T>).pipe(
        catchError(err2 => {
          if (err1.status === 0 && err2.status === 0) {
            return throwError(() => ({ backendDown: true }));
          }
          return throwError(() => err1.status === 0 ? err2 : err1);
        })
      ))
    );
  }
} 