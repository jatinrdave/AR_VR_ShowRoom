import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild, inject } from '@angular/core';
import * as THREE from 'three';
import { SceneService } from '../services/scene.service';
import { ModelService } from '../services/model.service';
import { ToolbarComponent } from './toolbar.component';
import { LayerPanelComponent } from './layer-panel.component';
import { ModelListComponent } from './model-list.component';
import { ApiService } from '../../core/api.service';
import { LayerService } from '../services/layer.service';
import { MeasurementService } from '../services/measurement.service';
import { ClippingService } from '../services/clipping.service';

@Component({
  selector: 'app-viewer',
  standalone: true,
  imports: [ToolbarComponent, LayerPanelComponent, ModelListComponent],
  template: `
  <div class="canvas-container">
    <canvas #canvas3d></canvas>
    <app-toolbar (upload)="onUpload($event)" (measure)="onMeasure()" (section)="onSection()" (toggleOrtho)="onToggleOrtho()" (toggleWire)="onToggleWire()" (downloadSvg)="onDownloadSvg()"></app-toolbar>
    <app-layer-panel [layers]="layers" (toggle)="onToggleLayer($event)"></app-layer-panel>
    <app-model-list (picked)="onPickModel($event)"></app-model-list>
  </div>
  `,
  styles: [``],
  providers: [SceneService]
})
export class ViewerComponent implements AfterViewInit, OnDestroy {
  @ViewChild('canvas3d', { static: true }) canvasRef!: ElementRef<HTMLCanvasElement>;
  private sceneService = inject(SceneService);
  private modelService = inject(ModelService);
  private api = inject(ApiService);
  private layersService = inject(LayerService);
  private measureService = inject(MeasurementService);
  private clippingService = inject(ClippingService);

  layers: Array<{ name: string; color?: string; visible: boolean }> = [];
  private disposeMeasure?: () => void;
  private isWire = false;
  private lastModelId: string | null = null;

  ngAfterViewInit(): void {
    this.sceneService.init(this.canvasRef.nativeElement);
    const id = localStorage.getItem('lastModelId');
    this.lastModelId = id;
    this.modelService.loadLatestIfAny(this.sceneService);
  }

  ngOnDestroy(): void {
    if (this.disposeMeasure) this.disposeMeasure();
    this.sceneService.dispose();
  }

  onUpload(file: File) {
    this.api.upload(file).subscribe(({ id }) => {
      localStorage.setItem('lastModelId', id);
      this.lastModelId = id;
      this.api.pollStatus(id).subscribe(status => {
        if (status.state === 'Completed') {
          this.loadModel(id);
        }
      });
    });
  }

  onPickModel(id: string) { this.loadModel(id); }

  private loadModel(id: string) {
    this.lastModelId = id;
    this.modelService.loadGltf(id, this.sceneService);
    this.api.getLayers(id).subscribe(ls => {
      this.layersService.setLayers(ls);
      this.layers = ls;
      this.layersService.applyToScene(this.sceneService.getScene());
    });
  }

  onMeasure() {
    if (this.disposeMeasure) { this.disposeMeasure(); this.disposeMeasure = undefined; return; }
    this.disposeMeasure = this.measureService.enable(this.canvasRef.nativeElement, this.sceneService.getScene(), this.sceneService.getCamera());
  }

  onSection() { this.clippingService.toggle(this.sceneService.getRenderer()); }

  onToggleLayer(name: string) {
    this.layersService.toggleLayer(name, this.sceneService.getScene());
    this.layers = this.layersService.getLayers();
  }

  onToggleWire() {
    this.isWire = !this.isWire;
    this.sceneService.getScene().traverse((obj: any) => {
      const mesh = obj as THREE.Mesh;
      const mat = mesh.material as THREE.Material | THREE.Material[];
      if (!mat) return;
      const setWire = (m: any) => { if ('wireframe' in m) m.wireframe = this.isWire; };
      if (Array.isArray(mat)) mat.forEach(setWire); else setWire(mat);
    });
  }

  onToggleOrtho() { this.sceneService.toggleOrtho(); }

  onDownloadSvg() {
    if (!this.lastModelId) return;
    this.api.getSvg(this.lastModelId).subscribe(svg => {
      const blob = new Blob([svg], { type: 'image/svg+xml' });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${this.lastModelId}.svg`;
      a.click();
      URL.revokeObjectURL(url);
    });
  }
}