import { Injectable, inject } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ConfigService {
  readonly apiBaseUrl = environment.apiBaseUrl;
}