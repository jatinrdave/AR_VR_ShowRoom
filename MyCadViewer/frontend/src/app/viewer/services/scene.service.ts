import { Injectable, NgZone } from '@angular/core';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';

@Injectable()
export class SceneService {
  private renderer!: THREE.WebGLRenderer;
  private scene!: THREE.Scene;
  private perspCamera!: THREE.PerspectiveCamera;
  private orthoCamera!: THREE.OrthographicCamera;
  private currentCamera!: THREE.Camera;
  private controls!: OrbitControls;
  private animationId: number | null = null;
  private useOrtho = false;

  constructor(private zone: NgZone) {}

  init(canvas: HTMLCanvasElement) {
    const width = canvas.clientWidth || window.innerWidth;
    const height = canvas.clientHeight || window.innerHeight;
    this.renderer = new THREE.WebGLRenderer({ canvas, antialias: true });
    this.renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    this.renderer.setSize(width, height);
    this.renderer.outputColorSpace = THREE.SRGBColorSpace;

    this.scene = new THREE.Scene();
    this.scene.background = new THREE.Color(0x0f0f10);

    this.perspCamera = new THREE.PerspectiveCamera(60, width / height, 0.1, 5000);
    this.perspCamera.position.set(5, 5, 5);

    const aspect = width / height;
    const frustum = 5;
    this.orthoCamera = new THREE.OrthographicCamera(-frustum*aspect, frustum*aspect, frustum, -frustum, 0.1, 5000);
    this.orthoCamera.position.set(5, 5, 5);
    this.orthoCamera.lookAt(0, 0, 0);

    this.currentCamera = this.perspCamera;

    const light = new THREE.HemisphereLight(0xffffff, 0x222244, 1.0);
    this.scene.add(light);

    this.controls = new OrbitControls(this.currentCamera, this.renderer.domElement);
    this.controls.enableDamping = true;

    window.addEventListener('resize', this.onResize);

    this.zone.runOutsideAngular(() => this.animate());
  }

  private animate = () => {
    this.animationId = requestAnimationFrame(this.animate);
    this.controls.update();
    this.renderer.render(this.scene, this.currentCamera);
  };

  private updateCamerasForSize(width: number, height: number) {
    this.perspCamera.aspect = width / height;
    this.perspCamera.updateProjectionMatrix();
    const aspect = width / height;
    const frustum = 5;
    this.orthoCamera.left = -frustum * aspect;
    this.orthoCamera.right = frustum * aspect;
    this.orthoCamera.top = frustum;
    this.orthoCamera.bottom = -frustum;
    this.orthoCamera.updateProjectionMatrix();
  }

  toggleOrtho() {
    this.useOrtho = !this.useOrtho;
    const next = this.useOrtho ? this.orthoCamera : this.perspCamera;
    // keep target and position
    next.position.copy((this.currentCamera as any).position);
    next.quaternion.copy((this.currentCamera as any).quaternion);
    this.currentCamera = next;
    // rebind controls
    this.controls.object = this.currentCamera as any;
    this.controls.update();
  }

  add(object: THREE.Object3D) { this.scene.add(object); }
  clear() { this.scene.clear(); }

  getScene(): THREE.Scene { return this.scene; }
  getCamera(): THREE.Camera { return this.currentCamera; }
  getRenderer(): THREE.WebGLRenderer { return this.renderer; }

  dispose() {
    window.removeEventListener('resize', this.onResize);
    if (this.animationId !== null) cancelAnimationFrame(this.animationId);
    this.scene.traverse(obj => {
      const mesh = obj as THREE.Mesh;
      if (mesh.geometry) mesh.geometry.dispose();
      const material = (mesh.material as THREE.Material);
      if (material) material.dispose();
    });
    this.renderer.dispose();
  }

  private onResize = () => {
    const width = window.innerWidth;
    const height = window.innerHeight;
    this.updateCamerasForSize(width, height);
    this.renderer.setSize(width, height);
  };
}