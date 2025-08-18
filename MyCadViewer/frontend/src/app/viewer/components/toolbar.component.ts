import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-toolbar',
  standalone: true,
  template: `
  <div style="position:absolute;top:8px;left:8px;display:flex;gap:8px;background:rgba(0,0,0,0.4);padding:8px;border-radius:6px;color:#fff;">
    <input type="file" (change)="onFile($event)" />
    <button (click)="measure.emit()">Measure</button>
    <button (click)="section.emit()">Section</button>
    <button (click)="toggleOrtho.emit()">Ortho</button>
    <button (click)="toggleWire.emit()">Wire</button>
    <button (click)="downloadSvg.emit()">SVG</button>
  </div>
  `
})
export class ToolbarComponent {
  @Output() upload = new EventEmitter<File>();
  @Output() measure = new EventEmitter<void>();
  @Output() section = new EventEmitter<void>();
  @Output() toggleOrtho = new EventEmitter<void>();
  @Output() toggleWire = new EventEmitter<void>();
  @Output() downloadSvg = new EventEmitter<void>();

  onFile(ev: Event) {
    const input = ev.target as HTMLInputElement;
    const file = input.files?.item(0);
    if (file) this.upload.emit(file);
  }
}