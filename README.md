# Fika-Installer
A work-in-progress console application for installing Fika, Fika-Server, and Fika-Headless.

## Objective
The goal is to make the tool as simple as possible with minimal user interaction. No bloated features, no complex GUI, and no configuration files - just execute and install. It is not a launcher nor a configuration tool, and never will be.

## Features
* Prepares a Fika instance by hard copying or symlinking the SPT directory.
* Downloads the latest version of Fika.Core, Fika.Server and Fika.Headless.
* Creates or uses an existing headless profile and copies the script.
* Configures the SPT launcher path to the Fika instance location.
* Updates an existing Fika / Fika Headless instance.

## Usage
* Place the Fika-Installer executable in your SPT folder *or* in a new folder outside of SPT folder (not required).
* Run the Fika-Installer executable.
* Choose whether to install a new Fika instance or a new Fika Headless instance. NOTE: Installing Fika-Headless requires you to have the Fika-Installer executable in the same machine where your SPT server with Fika-Server is installed.
* If you run the Fika-Installer from a fresh new folder, you will be prompted to browse for the SPT folder. The SPT and game files will be copied or symlinked to your directory based on your installation choice.
* Once the files are copied and Fika is downloaded, you may close the installer and enjoy Fika!

## Linux Support
Linux support is not planned. However, only minimal code modifications should be required for Linux compatibility. Feel free to submit a PR.
