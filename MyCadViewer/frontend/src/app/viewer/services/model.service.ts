import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../../core/config.service';
import { GLTFLoader } from 'three/examples/jsm/loaders/GLTFLoader.js';
import { DRACOLoader } from 'three/examples/jsm/loaders/DRACOLoader.js';
import * as THREE from 'three';
import { SceneService } from './scene.service';

@Injectable({ providedIn: 'root' })
export class ModelService {
  private http = inject(HttpClient);
  private config = inject(ConfigService);

  private createLoader() {
    const loader = new GLTFLoader();
    try {
      const draco = new DRACOLoader();
      draco.setDecoderPath('assets/draco/');
      loader.setDRACOLoader(draco);
    } catch (e) {
      console.warn('DRACO not configured; continuing without compression support');
    }
    return loader;
  }

  loadGltf(modelId: string, sceneService: SceneService) {
    const url = `${this.config.apiBaseUrl}/api/models/${modelId}/gltf`;
    this.http.get(url, { responseType: 'arraybuffer' }).subscribe(buf => {
      const loader = this.createLoader();
      loader.parse(buf as ArrayBuffer, '', (gltf) => {
        sceneService.add(gltf.scene);
      }, (err) => console.error(err));
    });
  }

  // demo: load latest by probing an index or local storage
  loadLatestIfAny(sceneService: SceneService) {
    const id = localStorage.getItem('lastModelId');
    if (id) this.loadGltf(id, sceneService);
    else {
      const axes = new THREE.AxesHelper(5);
      sceneService.add(axes);
    }
  }
}