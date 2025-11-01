# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Camera Provider

> Quick overview: Zero‑setup, global access to the “active” camera in Play and Edit Mode, plus its world height (Y) and distance from origin. Auto‑updates each frame via a PlayerLoop hook; override manually when needed.

A tiny static utility that tracks a reasonable “current camera” and exposes it anywhere as `CameraProvider.Active`. It also publishes `CameraProvider.Height` (Y position) and `CameraProvider.Distance` (magnitude of world position). In Play Mode it prefers `Camera.main`, then `Camera.current`, then a camera with a `targetTexture`, then the first camera; in Edit Mode it uses the focused Scene View camera. You can override the selection by calling `SetCameraInfo(Camera)` yourself.

![screenshot](Documentation/Screenshot.png)

## Features
- Global camera access
  - `CameraProvider.Active` gives you the current camera; `Height` and `Distance` are derived every frame
- Smart selection, Play vs. Edit
  - Play Mode priority: `Camera.main` → `Camera.current` → any with `targetTexture` → first in `Camera.allCameras`
  - Edit Mode: uses the focused Scene View camera (when the Scene view has focus)
- Auto‑update via PlayerLoop
  - Hooks into Unity’s `Update` loop on load and refreshes values each frame
- Manual override when needed
  - Call `CameraProvider.SetCameraInfo(myCamera)` to force values for custom camera rigs or sequences
- Lightweight and allocation‑free per frame
  - Simple polling; no GameObjects or components required in your scene

## Requirements
- Unity Editor 6000.0+
- Dependencies (Unity Essentials modules)
  - Editor Hooks – PlayerLoop: provides `PlayerLoopHook` used to register an `Update` callback

Tip: If you have multiple cameras and want to control which one is reported, tag your preferred camera as MainCamera or call `SetCameraInfo()` from your camera controller.

## Usage
Read values anywhere
```csharp
using UnityEngine;
using UnityEssentials;

public class SunTracker : MonoBehaviour
{
    void Update()
    {
        var cam = CameraProvider.Active; // may be null if no camera
        var height = CameraProvider.Height; // world-space Y
        var dist = CameraProvider.Distance; // distance from world origin
        if (cam) Debug.Log($"Active: {cam.name}, Height: {height:F1}, Dist: {dist:F1}");
    }
}
```

Prefer a specific camera (manual override)
```csharp
// For custom camera systems, call once per frame when you switch cameras:
CameraProvider.SetCameraInfo(myGameplayCamera);
```

Multi‑camera setups
- Ensure your primary camera is tagged `MainCamera` so it wins the selection in Play Mode
- Or call `SetCameraInfo()` when you switch virtual cameras (e.g., cutscenes, replays)

## How It Works
- On load
  - `RuntimeInitializeOnLoadMethod(AfterSceneLoad)` registers `GetCurrentRenderingCameraInfo` into the PlayerLoop `Update` phase via `PlayerLoopHook.Add<Update>(...)`
- Each frame
  - In Play Mode, picks a camera by priority (`main` → `current` → any `targetTexture` → first) and calls `SetCameraInfo`
  - In Edit Mode, uses the focused Scene View camera when available
  - `SetCameraInfo` stores:
    - `Active = camera`
    - `Height = camera.transform.position.y`
    - `Distance = camera.transform.position.magnitude` (distance from world origin)

## Notes and Limitations
- Distance meaning
  - `Distance` is the magnitude of the camera’s world position vector (distance to origin), not distance to a target
- Null cases
  - If no suitable camera is found (e.g., empty scene, unfocused Scene view), `Active` can be null
- Scene View focus
  - In Edit Mode, the Scene View camera is used only when that view has focus
- Selection heuristics
  - If your setup uses stacked/overlay cameras or VR rigs, you may want to call `SetCameraInfo()` explicitly
- PlayerLoop dependency
  - Requires the Unity Essentials PlayerLoop Hook module to inject the update delegate

## Files in This Package
- `Runtime/CameraProvider.cs` – Static provider for `Active`, `Height`, and `Distance`; PlayerLoop registration
- `Runtime/UnityEssentials.CameraProvider.asmdef` – Runtime assembly definition
- `package.json` – Package manifest metadata

## Tags
unity, camera, provider, active camera, maincamera, scene view, height, distance, playerloop, runtime
