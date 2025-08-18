import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-layer-panel',
  standalone: true,
  template: `
  <div style="position:absolute;top:60px;left:8px;max-height:60vh;overflow:auto;background:rgba(0,0,0,0.4);padding:8px;border-radius:6px;color:#fff;">
    <div *ngFor="let layer of layers; let i = index" style="display:flex;align-items:center;gap:8px;">
      <input type="checkbox" [checked]="layer.visible" (change)="toggle.emit(layer.name)" />
      <span [style.color]="layer.color || '#fff'">{{layer.name}}</span>
    </div>
  </div>
  `
})
export class LayerPanelComponent {
  @Input() layers: Array<{ name: string; color?: string; visible: boolean }> = [];
  @Output() toggle = new EventEmitter<string>();
}