[TOC]

# MGS.License

## Summary

- License Settings and Builder and Activate for develop Unity product.

## Ability

- License Settings in Unity Editor.
- License Build in Unity Editor.
- License Activate in App runtime.

## Install

- Unity --> Window --> Package Manager --> "+" --> Add package from git URL...

  ```text
  https://github.com/mogoson/MGS.License.git?path=/Assets
  ```
## Usage

- Unity menu "Toos/License/Settings" to set license parameters.
  - Should not change (Re Generate) the keys after project published.
- Unity menu "Toos/License/Builder" to build license.
  - Trial button to get a license request text for trial.
  - The license build from trial request without bind device.
- Unity menu "Toos/License/Clear" to clear activate information from current device.
- Import the UI sample, add the LicenseUI prefab to your start scene Canvas.
  - Verify license when start scene run.
  - Activate license from file named {productName}.lic at path persistentDataPath or streamingAssetsPath.
  - Create license request file named {productName}.lre at path persistentDataPath if verify failed.

## Samples

- Unity --> Window --> Package Manager --> Packages-Mogoson --> License --> Samples.

------

Copyright © 2026 Mogoson.	mogoson@outlook.com