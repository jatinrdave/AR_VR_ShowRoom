import { Injectable } from '@angular/core';
import * as THREE from 'three';

@Injectable({ providedIn: 'root' })
export class MeasurementService {
  private raycaster = new THREE.Raycaster();
  private pointer = new THREE.Vector2();
  private pendingPoint: THREE.Vector3 | null = null;

  enable(canvas: HTMLCanvasElement, scene: THREE.Scene, camera: THREE.Camera) {
    const onClick = (ev: MouseEvent) => {
      const rect = canvas.getBoundingClientRect();
      this.pointer.x = ((ev.clientX - rect.left) / rect.width) * 2 - 1;
      this.pointer.y = -((ev.clientY - rect.top) / rect.height) * 2 + 1;
      this.raycaster.setFromCamera(this.pointer, camera);
      const intersects = this.raycaster.intersectObjects(scene.children, true);
      if (intersects.length > 0) {
        const p = intersects[0].point.clone();
        if (!this.pendingPoint) {
          this.pendingPoint = p;
        } else {
          const geom = new THREE.BufferGeometry().setFromPoints([this.pendingPoint, p]);
          const mat = new THREE.LineBasicMaterial({ color: 0xffcc00 });
          const line = new THREE.Line(geom, mat);
          scene.add(line);
          this.pendingPoint = null;
        }
      }
    };
    canvas.addEventListener('click', onClick);
    return () => canvas.removeEventListener('click', onClick);
  }
}