import { Component, EventEmitter, Output, inject } from '@angular/core';
import { ApiService } from '../../core/api.service';

@Component({
  selector: 'app-model-list',
  standalone: true,
  template: `
  <div style="position:absolute;top:60px;right:8px;max-height:60vh;overflow:auto;background:rgba(0,0,0,0.4);padding:8px;border-radius:6px;color:#fff;min-width:240px;">
    <div style="font-weight:600;margin-bottom:6px;">Models</div>
    <div *ngFor="let m of models" (click)="select(m.id)" style="cursor:pointer;padding:4px 6px;border-radius:4px;" [style.background]="'#1e1e1e'">
      <div>{{m.name}}</div>
      <small>{{m.id}} — {{m.state}}</small>
    </div>
  </div>
  `
})
export class ModelListComponent {
  private api = inject(ApiService);
  models: Array<{ id: string; name: string; state: string }> = [];
  @Output() picked = new EventEmitter<string>();

  constructor() {
    this.api['http'].get<Array<{ id: string; name: string; state: string }>>(`${this.api['cfg'].apiBaseUrl}/api/models`).subscribe(m => this.models = m);
  }

  select(id: string) {
    this.picked.emit(id);
  }
}