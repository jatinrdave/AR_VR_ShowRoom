import { Injectable, NgZone } from '@angular/core';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';

@Injectable()
export class SceneService {
  private renderer!: THREE.WebGLRenderer;
  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private orthoCamera!: THREE.OrthographicCamera;
  private controls!: OrbitControls;
  private animationId: number | null = null;

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

    this.camera = new THREE.PerspectiveCamera(60, width / height, 0.1, 5000);
    this.camera.position.set(5, 5, 5);

    const aspect = width / height;
    const frustum = 5;
    this.orthoCamera = new THREE.OrthographicCamera(-frustum*aspect, frustum*aspect, frustum, -frustum, 0.1, 5000);

    const light = new THREE.HemisphereLight(0xffffff, 0x222244, 1.0);
    this.scene.add(light);

    this.controls = new OrbitControls(this.camera, this.renderer.domElement);
    this.controls.enableDamping = true;

    window.addEventListener('resize', this.onResize);

    this.zone.runOutsideAngular(() => this.animate());
  }

  private animate = () => {
    this.animationId = requestAnimationFrame(this.animate);
    this.controls.update();
    this.renderer.render(this.scene, this.camera);
  };

  add(object: THREE.Object3D) {
    this.scene.add(object);
  }

  clear() {
    this.scene.clear();
  }

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
    this.camera.aspect = width / height;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(width, height);
  };
}