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

- Unity menu "Toos/License/Builder" to build license.
- Unity menu "Toos/License/Clear" to clear activate information.
- Import the UI sample, add the LicenseUI prefab to your start scene Canvas.
  - Auto verify license when start scene run.
  - Auto activate license from file named {productName}.lic at path persistentDataPath or streamingAssetsPath.

## Samples

- Unity --> Window --> Package Manager --> Packages-Mogoson --> License --> Samples.

------

Copyright © 2026 Mogoson.	mogoson@outlook.com