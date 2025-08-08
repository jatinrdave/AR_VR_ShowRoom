<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" style="height:64px;margin-right:32px"/>

# Integrated AR Visualization \& Virtual Showroom Solution for Windows-and-Doors

**Main takeaway:** Combine an Android AR app (Unity + AR Foundation + on-device generative segmentation) with an Angular/.NET WebXR showroom backed by an LLM service layer. The two clients share ³ common micro-services (catalog, recommendation, chat) and a vector database, giving field reps and end-customers a seamless “configure-anywhere, visualise-everywhere” experience.

## 1. Business Goals

* Close the “imagination gap” by letting users point a phone at an opening and see realistic windows/doors composited into the live camera feed[^1][^2].
* Offer a virtual showroom that answers questions, guides tours, and remembers customer intent via ChatGPT-style dialogue[^3][^4].
* Re-use existing .NET back-end, Angular skills, and MS SQL while adding a free-tier vector store (Qdrant) for embeddings[^5][^6].


## 2. High-Level Architecture

| Tier | Key Components | Tech Stack |
| :-- | :-- | :-- |
| **Client AR App (Android)** | Unity 2023 LTS, AR Foundation (ARCore provider), on-device SAM-Lite segmentation (ONNX), gRPC talk to back-end | C\#, IL2CPP ARM64 |
| **Web Showroom (Desktop/Mobile Browser)** | Angular 17, Three.js + @WebXR for immersive room; A-Frame/AR.js fallback for mid-range devices | TypeScript |
| **AI \& Data Services** | ASP.NET Core 8 micro-services: Catalog, Pricing, Chat, Recommendation | C\# |
|  | Vector DB for embeddings | Qdrant (Docker) |
|  | LLM gateway | Azure OpenAI / Local llama3 via Ollama |
|  | Image-Gen \& Segmentation | Meta Segment-Anything (ONNX); Stable-Diffusion-XL via API |
| **Ops** | Azure DevOps CI/CD, Docker, K8s, Grafana/Prometheus |  |

## 3. AR Visualization App (Android)

### 3.1 Pipeline

1. **Plane \& Anchor Detection** – ARCore detects walls/floors[^7][^8].
2. **Generative Segmentation** – Live frame passes through on-device SAM-Lite (≈28 MB model) via Barracuda; returns mask of aperture.
3. **Pose Estimation** – Mask bounds → window plane; anchor updated each frame for stable placement.
4. **3-D Model Fit** – Selected SKU (glb) scales to mask; PBR materials loaded from AssetBundle.
5. **User Interaction** – Swipe to swap designs, tap to open spec sheet (fetched via REST).
6. **Snapshot \& Quote** – “Save design” captures PNG + metadata → uploads to Quote API.

### 3.2 Key Unity Packages

- `com.unity.xr.arfoundation` 6.x[^9]
- `com.unity.barracuda` for ONNX inference
- `UniTask` for async loading
- `UnityGLTF` for runtime GLB import


### 3.3 Critical Settings

- **Min API level 24**, IL2CPP, ARM64[^8]
- Disable Vulkan, use OpenGLES3 for ARCore[^10].


## 4. Virtual Showroom (Web)

### 4.1 Scene Composition

* **Room Shell**: glTF exported from Blender; dynamically swaps wall textures for themes.
* **Product Blocks**: Angular component  custom 3-D directive wraps Three.js meshes.
* **Navigation**: WASD / touch; on mobile VR headset = WebXR immersive-vr session[^11].


### 4.2 ChatGPT-Style Guide

* UI widget powered by Lobe-Chat fork[^12]; talks via SignalR to Chat API.
* Chat API performs RAG over:
    - Catalog \& tech sheets (stored as chunks in Qdrant)
    - FAQ table in MS SQL
* Function-calling enables **“change room style”, “show aluminium casement”** → emits JSON action to Angular which updates 3-D scene[^13][^14].


## 5. Shared AI Service Layer

| Service | Responsibilities | Libraries |
| :-- | :-- | :-- |
| **Embedding-Indexer** | Convert PDFs, CAD BOMs → sentence-transformer embeddings, upsert to Qdrant | SentenceTransformers-.NET |
| **Chat API** | OpenAI chat/completions + RAG + function calls | Azure.AI.OpenAI, LangChain-C\# |
| **Recommender** | Hybrid: CF (ML.NET MatrixFactorization) + vector-sim | ML.NET 4.0, Qdrant.Client |

All services expose minimal REST + gRPC; protected by Azure AD B2C.

## 6. Data Flow Example (“Replace door style”)

1. User in showroom says: “Show me a sliding patio door.”
2. Angular posts audio → Whisper endpoint, gets text.
3. Chat API (LLM) calls `search_catalog(term="sliding patio")`.
4. Catalog service returns top SKUs; Chat replies with carousel + triggers scene update.
5. Angular loads new GLB; if on Android AR app, the same SKU ID arrives via SignalR and model swap occurs in Unity (code-shared DTOs).

## 7. Security \& Privacy

- All mobile inference runs on-device – camera frames never leave handset.
- Telemetry anonymised, user consents stored (GDPR).
- LLM requests routed through API gateway with rate-limits, content filter (LLM-Guard)[^14].


## 8. DevOps Blueprint

1. **Monorepo** (Nx) hosting Angular, .NET, Unity package.
2. Pipeline stages: build -> test –> Docker publish –> Helm deploy.
3. Unity Cloud Build exports Android AAB; Play Store Internal track.
4. Release gating via synthetic XR tests (OpenTelemetry spans).

## 9. Incremental Roll-Out Plan (20 weeks)

| Sprint | Milestone |
| :-- | :-- |
| 1-2 | Skeleton .NET APIs, Angular shell, Unity AR plane-detection prototype |
| 3-4 | Segment-Anything ONNX integration, basic model overlay |
| 5-6 | Catalog micro-service, product selection UI |
| 7-8 | Qdrant infra, embedding pipeline |
| 9-10 | Chat API w/ RAG over FAQs |
| 11-12 | WebXR showroom, LLM function-calls control |
| 13-14 | Hybrid recommender, quote export |
| 15-16 | Security hardening, GDPR, cache |
| 17-18 | Closed beta (field reps) |
| 19-20 | Public web launch \& Play Store rollout |

## 10. KPI Targets

* AR placement latency ≤ 120 ms p95[^15].
* Chat answer relevance ≥ 85% (BLEU vs ground-truth) in user tests[^14].
* Conversion uplift ≥ 30% in A/B vs legacy quote tool.


## 11. Key Libraries \& Licences

| Area | Library | Licence |
| :-- | :-- | :-- |
| Segmentation | Meta SAM-Lite ONNX | Apache-2.0[^5] |
| AR | Unity AR Foundation | Unity Companion |
| WebXR | @angular-three/core, A-Frame | MIT \& Apache-2.0[^16] |
| LLM | Azure OpenAI / llama3 via Ollama | Azure SLA / GPLv3 |
| Vector DB | Qdrant | Apache-2.0 |

## 12. Next-Step POCs

1. Run SAM-Lite inference on Pixel 7 at 30 fps – validate battery draw.
2. Prototype Chat→Function pipeline: “rotate door 45°” triggers Three.js animation.
3. Migrate one real CAD assembly to glTF + USDZ for iOS preview.

Deploy this stack and you deliver an end-to-end, future-proof sales platform where customers can **see**, **ask**, and **buy** without leaving their couch—or the jobsite.

<div style="text-align: center">⁂</div>

[^1]: https://www.digitalbuilding.solutions/our-solutions/dbs-windo-imaging/

[^2]: https://github.com/Rishabh-Pahwa/AR-Door-Viewer

[^3]: https://scottamyx.com/2023/05/25/chatgpt-for-virtual-reality-enhancing-immersive-experiences/

[^4]: https://www.convai.com

[^5]: https://pmc.ncbi.nlm.nih.gov/articles/PMC7235323/

[^6]: https://www.softwaretestinghelp.com/ar-app-development-using-unity-ar-foundation/

[^7]: https://developers.google.com/ar/develop/unity-arf/getting-started-ar-foundation

[^8]: https://www.andreasjakl.com/ar-foundation-fundamentals-with-unity-part-1/

[^9]: https://docs.unity3d.com/6000.1/Documentation/Manual/com.unity.xr.arfoundation.html

[^10]: https://github.com/mklewandowski/unity-ar-example

[^11]: https://www.youtube.com/watch?v=Obw7xb8VVuc

[^12]: https://github.com/lobehub/lobe-chat

[^13]: https://github.com/PacktPublishing/ChatGPT-for-Conversational-AI-and-Chatbots

[^14]: https://github.blog/ai-and-ml/llms/the-architecture-of-todays-llm-applications/

[^15]: https://developers.google.com/ar/develop

[^16]: https://arvrjourney.com/xr-tutorial-how-to-build-a-webxr-with-angular-js-ar-js-and-a-frame-js-4ca432038bc2

[^17]: https://www.buildings.com/smart-buildings/article/33018615/augmented-reality-apps-for-architects-engineers-contractors-and-owners

[^18]: https://play.google.com/store/apps/details?id=de.dbs.imaging.viewer\&hl=en_IN

[^19]: https://renderatelier.com/augmented-reality-apps/

[^20]: https://arxiv.org/abs/2305.18164

[^21]: https://discourse.threejs.org/t/how-to-create-window-and-door-openings-in-the-wall/20473

[^22]: https://www.digitalbuilding.solutions/our-solutions/dbs-windo-imaging/windowviewer/

[^23]: https://gamma-ar.com/revolutionize-your-construction-projects-with-our-augmented-reality-app/

[^24]: https://www.sciencedirect.com/science/article/abs/pii/S001048252100857X

[^25]: https://prototechsolutions.com/cad-notes/augmented-reality-development-ar/

[^26]: https://www.graberblinds.com/visualization/visualizer-mobile-app/

[^27]: https://smarttek.solutions/blog/augmented-reality-in-construction/

[^28]: https://www.nature.com/articles/s41467-025-61754-6

[^29]: https://aframe.io/docs/1.7.0/introduction/developing-with-threejs.html

[^30]: https://fourthedesign.gr/en/portfolio/aluminum-system-visualisation/

[^31]: https://www.archdaily.com/878408/the-top-5-virtual-reality-and-augmented-reality-apps-for-architects

[^32]: https://www.sciencedirect.com/science/article/pii/S0957417425016975

[^33]: https://stemkoski.github.io/AR-Examples/

[^34]: https://arinsider.co/2024/04/09/7-steps-to-elevate-product-showrooms-with-vr/

[^35]: https://www.travancoreanalytics.com/conversational-ai-chatbot/

[^36]: https://blog.devgenius.io/ar-in-unity-fc61fb641c68

[^37]: https://rockpaperreality.com/insights/ar-use-cases/ar-virtual-showrooms-augmented-reality-vr/

[^38]: https://arxiv.org/html/2411.04671v1

[^39]: https://www.digitalexperience.live/ai-based-tools-simplified-arvr-design

[^40]: https://www.brandxr.io/a-complete-guide-to-virtual-showrooms-sell-your-products-using-augmented-and-virtual-reality

[^41]: https://arxiv.org/pdf/2402.03907.pdf

[^42]: https://botpress.com/blog/ultimate-guide-to-artificial-intelligence-ai-and-augmented-reality-ar

[^43]: https://dotnet.microsoft.com/en-us/apps/games/unity

[^44]: https://www.ienhance.co/insights/virtual-showrooms-why-they-are-the-next-big-thing-in-shopping

[^45]: https://arvrjourney.com/chatgpt-a-sneak-peek-into-its-abilities-limitation-and-how-to-make-the-best-use-of-it-ae180094139b

[^46]: https://www.engati.com/glossary/conversational-ai

[^47]: https://docs.unity3d.com/6000.1/Documentation/Manual/AROverview.html

[^48]: https://www.designersx.us/virtual-showroom-development-costs-and-implementation/

[^49]: https://www.tandfonline.com/doi/full/10.1080/10447318.2025.2504188?src=

[^50]: https://www.youtube.com/watch?v=OJOdUIoxdF4

[^51]: https://ionic.io/blog/cross-platform-ar-vr-with-the-web-webxr-with-a-frame-angular-and-capacitor-part-ii

[^52]: https://stackoverflow.com/questions/49823299/using-arcore-1-1-0-with-nativeactivity-and-building-with-other-ide-visual-studi

[^53]: https://moldstud.com/articles/p-a-comprehensive-guide-for-uae-developers-to-kickstart-their-journey-with-arvr-in-xamarin

[^54]: https://www.intellectsoft.net/blog/web-application-architecture/

[^55]: https://talent500.com/blog/creating-immersive-ar-web-applications/

[^56]: https://www.queppelin.com/ar-core-development-services/

[^57]: https://devblogs.microsoft.com/xamarin/augmented-reality-xamarin-android-arcore/

[^58]: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

[^59]: https://hackernoon.com/building-ar-vr-with-javascript-and-html-28acd1da0371

[^60]: https://developers.google.com/ar/develop/java/quickstart

[^61]: https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/october/xamarin-augmented-reality-in-xamarin-forms

[^62]: https://www.carmatec.com/blog/web-application-architecture-complete-guide/

[^63]: https://hqsoftwarelab.com/solutions/ar-vr-development/

[^64]: https://www.techaheadcorp.com/blog/exploring-augmented-reality-ar-android-app-development-projects/

[^65]: https://innowise.com/technologies/xamarin-development/

[^66]: https://web.dev/learn/pwa/architecture

[^67]: https://theintellify.com/ar-app-development-company/

[^68]: https://www.youtube.com/watch?v=qIK1F9rGvcI

[^69]: https://learn.microsoft.com/en-us/azure/architecture/web-apps/guides/enterprise-app-patterns/overview

[^70]: https://github.com/saumya-pailwan/AR-Car-Showroom

[^71]: https://www.youtube.com/watch?v=FJAO6jDYljs

[^72]: https://www.linkedin.com/pulse/creating-immersive-augmented-reality-applications-using-srikanth-r-wv6yc

[^73]: https://arxiv.org/html/2401.11923v2

[^74]: https://github.com/araobp/virtual-showroom

[^75]: https://www.youtube.com/watch?v=kt0FrkQgw8w

[^76]: https://www.upgrad.com/blog/augmented-reality-examples/

[^77]: https://github.com/anand-sharan/ShopAssistAI

[^78]: https://www.reddit.com/r/WebVR/comments/sdd7te/aframe_or_threejs_whats_the_best_way_to_embed_a/

[^79]: https://research.aimultiple.com/ar-use-cases/

[^80]: https://www.youtube.com/watch?v=gpaq5bAjya8

[^81]: https://threejs.org/docs/

[^82]: https://dl.acm.org/doi/full/10.1145/3706598.3714224

[^83]: https://unity.com/solutions/xr/ar

[^84]: https://github.com/jeromeetienne/AR.js/blob/master/three.js/examples/arjs-session.html

[^85]: https://www.sciencedirect.com/science/article/abs/pii/S0278612524000967

[^86]: https://nartc.me/blog/convert-threejs-to-angular-three/

[^87]: https://learntodroid.com/consuming-a-rest-api-using-retrofit2-with-the-mvvm-pattern-in-android/

[^88]: https://dev.to/renancferro/understanding-and-implementing-threejs-with-angular-and-creating-a-3d-animation-3eea

[^89]: https://code.tutsplus.com/android-from-scratch-using-rest-apis--cms-27117t

[^90]: https://www.arxiv.org/pdf/2501.00168.pdf

[^91]: https://www.appleute.de/en/app-entwickler-bibliothek/rest-api-design-patterns/

[^92]: https://www.mdpi.com/2078-2489/16/7/556

[^93]: https://discussions.unity.com/t/openxr-vs-arfoundation/742692

[^94]: https://dl.acm.org/doi/abs/10.1145/3706598.3714224

[^95]: https://developers.google.com/ar/reference

[^96]: https://www.pulpstrategy.com/virtual-stores

