import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map, switchMap, timer, takeWhile } from 'rxjs';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private cfg = inject(ConfigService);

  upload(file: File): Observable<{ id: string }> {
    const form = new FormData();
    form.append('file', file, file.name);
    return this.http.post<{ id: string }>(`${this.cfg.apiBaseUrl}/api/models`, form);
  }

  pollStatus(id: string): Observable<{ state: string; error?: string }> {
    return timer(0, 1500).pipe(
      switchMap(() => this.http.get<{ state: string; error?: string }>(`${this.cfg.apiBaseUrl}/api/models/${id}`)),
      takeWhile(resp => resp.state !== 'Completed' && resp.state !== 'Failed', true)
    );
  }

  getLayers(id: string) {
    return this.http.get<Array<{ name: string; color?: string; visible: boolean }>>(`${this.cfg.apiBaseUrl}/api/layers/${id}`);
  }
}