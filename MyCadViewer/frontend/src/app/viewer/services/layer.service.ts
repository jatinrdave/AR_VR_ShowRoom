import { Injectable } from '@angular/core';
import * as THREE from 'three';

@Injectable({ providedIn: 'root' })
export class LayerService {
  private layerStates = new Map<string, boolean>();

  setLayers(layers: Array<{ name: string; color?: string; visible: boolean }>) {
    this.layerStates.clear();
    for (const l of layers) this.layerStates.set(l.name, l.visible);
  }

  getLayers(): Array<{ name: string; color?: string; visible: boolean }> {
    return Array.from(this.layerStates.entries()).map(([name, vis]) => ({ name, visible: vis }));
  }

  toggleLayer(name: string, scene: THREE.Scene) {
    const current = this.layerStates.get(name) ?? true;
    this.layerStates.set(name, !current);
    this.applyToScene(scene);
  }

  applyToScene(scene: THREE.Scene) {
    scene.traverse(obj => {
      if (obj.name?.startsWith('Layer:')) {
        const lname = obj.name.substring('Layer:'.length);
        const vis = this.layerStates.get(lname);
        if (vis !== undefined) obj.visible = vis;
      }
    });
  }
}