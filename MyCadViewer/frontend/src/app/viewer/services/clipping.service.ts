import { Injectable } from '@angular/core';
import * as THREE from 'three';

@Injectable({ providedIn: 'root' })
export class ClippingService {
  private plane = new THREE.Plane(new THREE.Vector3(0, 1, 0), 0);
  private enabled = false;

  toggle(renderer: THREE.WebGLRenderer) {
    this.enabled = !this.enabled;
    renderer.clippingPlanes = this.enabled ? [this.plane] : [];
    renderer.localClippingEnabled = this.enabled;
  }
}