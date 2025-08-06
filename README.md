# Fika-Installer
A work-in-progress console application for installing Fika, Fika-Server, and Fika-Headless.

## Objective
The goal is to make the tool as simple as possible with minimal user interaction. No bloated features, no complex GUI, and no configuration files - just execute and install. It is not a launcher nor a configuration tool, and never will be.

## Features
* Prepares a Fika instance by hard copying or symlinking the SPT directory.
* Downloads the latest version of Fika.Core, Fika.Server and Fika.Headless.
* Creates or uses an existing headless profile and copies the script.
* Updates an existing Fika / Fika Headless instance.

## Linux Support
Linux support is not planned. However, only minimal code modifications should be required for Linux compatibility. Feel free to submit a PR.
