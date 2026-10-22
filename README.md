# AutoParking

Autonomous parallel parking simulation built in Unity for **CSCI218**. A car uses raycast distance sensors and a fuzzy logic controller to find a parking space, reverse into it, straighten, and move forward to center itself.

The repository includes the editable Unity project and a separate MATLAB fuzzy inference model with test scenarios.

## Play the Windows game

1. Download **[Game.zip from the latest release](https://github.com/SallmanAkram/AutoParking/releases/latest)**.
2. Extract the entire ZIP file.
3. Open the extracted `Game` folder and run `AutoParking.exe`.

The simulation starts automatically. Keep `AutoParking_Data`, `UnityPlayer.dll`, `MonoBleedingEdge`, and the other supplied runtime files beside the executable. Unity is not required to play the Windows build.

The release also includes the supplied demo recordings.

## Open the Unity project

**Unity Editor version: `6000.2.12f1` (Unity 6.2).** Use this version for the closest match to the original project.

1. Clone this repository:

   ```sh
   git clone https://github.com/SallmanAkram/AutoParking.git
   ```

2. Install Unity Hub and Unity Editor `6000.2.12f1`.
3. In Unity Hub, choose **Add project from disk** and select the cloned repository's **`AutoParking` subfolder**, which contains `Assets`, `Packages`, and `ProjectSettings`.
4. Open the project. Allow Unity to restore its packages and import the assets on the first launch; an internet connection is needed for package restoration.
5. In the Project window, open **`Assets/CSCI218_project.unity`**.
6. Press **Play** to start the simulation.

Unity recreates `Library` and other generated folders locally. All source assets and their `.meta` files, package dependencies and lockfile, and project settings are included in Git.

To create a new Windows executable, install the Windows Build Support module for this editor version, open **File > Build Profiles**, select a Windows profile, add `Assets/CSCI218_project.unity` to its scene list, and build. The supplied project does not have a scene enabled in its original build settings.

## How it works

The controller in [`FuzzyParkingPilot.cs`](AutoParking/Assets/FuzzyParkingPilot.cs) reads front, rear, and back-right raycast sensors. Its state machine progresses through:

1. **Scanning:** measure the gap beside the car and look for sufficient space.
2. **Entry turn:** reverse and steer right using fuzzy steering and speed decisions.
3. **Straighten:** reverse and steer left until the car aligns or approaches the rear obstacle.
4. **Auto-center:** move forward until the available front and rear space is approximately balanced.
5. **Parked:** stop after centering.

The Unity controller implements its logic directly in C#. It runs independently of MATLAB.

## Run the MATLAB scenarios

Requires **MATLAB with Fuzzy Logic Toolbox**. Change MATLAB's current folder to `MATLAB_FINAL`, then run:

```matlab
parkingFIS = readfis('2ParallelParkingFIS.fis');
test_parking_fuzzy
```

Load `parkingFIS` first because the supplied test script expects it in the workspace. The script evaluates one example and five parking scenarios, prints steering and speed results, and plots the outputs.

The separate MATLAB model is a Mamdani fuzzy inference system with four inputs (front distance, rear distance, angle, and lateral offset), two outputs (steering and speed), and 24 rules.

## Repository contents

| Path | Contents |
| --- | --- |
| `AutoParking/Assets/` | Scenes, C# controller, materials, car models, textures, and Unity metadata |
| `AutoParking/Packages/` | Unity package manifest and dependency lockfile |
| `AutoParking/ProjectSettings/` | Unity project configuration and editor version |
| `MATLAB_FINAL/` | Fuzzy inference model and MATLAB scenario script |
| `Instructions.txt` | Original course submission instructions |

The original instructions mention a report and submission filenames that are not part of the supplied files. The steps and paths above describe this repository.

Generated Unity caches, logs, local editor preferences, extracted archive metadata, source ZIP files, and built game files are excluded from Git. The Windows game and demo recordings are distributed as release assets to keep source clones small. Third-party asset documentation is retained under `AutoParking/Assets/Awb-Free Low Poly Cars/Important!!.pdf`.
