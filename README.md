# Fika-Installer
A console application for installing Fika, Fika-Server, and Fika-Headless.

## Objective
The goal is to make the tool as simple as possible with minimal user interaction. No bloated features, no complex GUI, and no configuration files - just execute and install. It is not a launcher nor a configuration tool, and never will be.

## Features
* Install the latest version of Fika (Fika-Core and Fika-Server).
* Install Fika Headless with profile management (create or use an existing headless profile).
* Update Fika or Fika-Headless.
* Symlink SPT folder to your Fika/Fika Headless folder to save disk space.
* Command-line arguments are supported for automation.

## Usage
* Place the Fika-Installer executable in your SPT folder.
* Run the Fika-Installer executable.
* Choose "Install Fika".
* Wait for installation to finish.
* Enjoy Fika!

## Advanced usage
There is an "Advanced options" menu with a few additional features:
* "Install Fika in current folder" allows you to install Fika-Core and Fika-Server in the directory where Fika-Installer is located. The SPT files will be copied or symlinked.
* "Install Fika Headless" allows you to install Fika-Core and Fika-Headless in the directory where Fika-Installer is located. The SPT files will be copied or symlinked.

## CLI
### Supported arguments:
```md
  install fika [--path <spt_path>] [--method <HardCopy> <Symlink>]
  install headless [--path <spt_path>] [--method <HardCopy> <Symlink>] [--profileId <headless_profile_id>]
  update fika
  update headless
  uninstall
```
### Optional arguments:
```md
  --path is the EFT/SPT folder path to copy the files from to create a duplicate instance.
  --method is the install method (HardCopy or Symlink). Only works if -path is defined.
  --profileId is the headless profile id. If not specified, a new headless profile will be created.
```
## Admin requirement
Fika-Installer requires admin elevation for firewall configuration and symlinking. Fika-Installer does not perform any other modifications to your system.

## Linux Support
Linux support is not planned. Feel free to submit a PR for Linux support.
