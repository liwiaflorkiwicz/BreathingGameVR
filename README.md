# BreathingGameVR

A virtual reality game designed to support calm, controlled breathing exercises using Oculus Quest.

## Table of Contents

- [About](#about)
- [Features](#features)
- [Requirements](#requirements)
- [Installation & Setup](#installation--setup)
- [Usage](#usage)
- [How to Play](#how-to-play)
- [Project Structure](#project-structure)

## About

BreathingGameVR is a VR application created to help users practice guided breathing in a peaceful, immersive environment.  
It can be used for relaxation, stress reduction, or mindful breathing training.

## Features

- Designed for Oculus Quest
- Guided breathing cycles
- Simple VR interactions
- Calm visuals supporting the breathing rhythm
- Easy to modify and extend in Unity

## Requirements

- Oculus Quest 1
- Unity (use the same version as in the project, recommended Unity 2021+)
- XR Plugin Management / Oculus Integration depending on your setup

## Installation & Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/liwiaflorkiwicz/BreathingGameVR.git
   ```
2. Open the project in Unity.
3. Ensure VR support packages are installed and enabled.
4. Build the project for Android (Oculus Quest target).
5. Install the build on your headset using the Meta Quest Developer Hub or adb.

## Usage

Turn on the headset and launch the app.
Follow breathing instructions displayed inside the virtual environment.

### How to Play

Observe the visual cues indicating inhale and exhale timing.
Match your breathing to the rhythm shown.
Continue for as long as desired to complete a session.

### Project Structure

BreathingGameVR/
 ├── Assets/
 │   ├── Scripts/         # Code for breathing mechanics and interactions
 │   ├── Scenes/          # Unity scenes
 │   ├── Models/          # 3D models (if included)
 │   ├── Materials/       # Materials and textures
 │   └── Prefabs/         # Prefab objects
 ├── ProjectSettings/
 └── Packages/
