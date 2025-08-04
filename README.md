# Fika-Installer

A work-in-progress console application for installing Fika, Fika-Server, and Fika-Headless.

## Objective

The goal is to make the tool as simple as possible with minimal user interaction. No bloated features, no complex GUI, and no configuration files - just execute and install. It is not a launcher or a configuration tool, and will never be.

## Linux Support

Linux support is not planned. However, only minimal code modifications should be required for Linux compatibility. Specifically, target .NET Core and replace the Windows Forms file browser with an alternative that works on Linux. Feel free to submit a PR for Linux support.
