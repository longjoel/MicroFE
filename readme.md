# MicroFE - A minimalistic frontend

## About MicroFE

MicroFE is a bare-bones emulator frontend for Windows, Mac, and Linux.

## MicroFE Features

* **Minimalism** - Only the most necessary features are provided. You get a text based menu driven interface that you can navigate by keyboard or gamepad.
A universal quit combo for Xbox compatable gamepads. Only a D-pad is needed to operate the interface. No databases, just a file system.

* **Playlist Support** - Simply create a text file with a `.txt` extension and have a list of file names. This file goes in your roms folder. The 
name of the text file is what will be displayed as the file name.

## Getting the latest version of MicroFE

* *Download Link Goes Here*

## Settings Up MicroFE - Windows

Here is a template you can use for the config.json file.
```json
{
  "Theme": {
    "BackgroundColor": "Black",
    "BorderColor": "Green",
    "TextColor": "Green",
    "TitleColor": "Green",
    "SelectedTextColor": "Black",
    "SelectedTextBackgroundColor": "Green"
  },
  "Actions": [
    {
      "Name": "Launch RetroArch Gui",
      "Path": "C:/Games/Emulation/Emulators/retroarch/RetroArch.exe"
    }
  ],
  "Emulators": [
    {
      "System": "Nintendo Entertainment System",
      "WorkingDirectory": "C:/Games/Emulation/Emulators/retroarch",
      "EmuPath": "C:/Games/Emulation/Emulators/retroarch/RetroArch.exe",
      "EmuArgs": [
        "-f",
        "-L",
        "C:/Games/Emulation/Emulators/retroarch/Cores/nestopia_libretro.dll",
        "%ROM%"
      ],
      "RomPath": "C:/Games/Emulation/Roms/Nes",
      "RomFilter": [ ".zip", ".nes" ]
    }
  ]
}

```

### Theme

The theme is composed of 6 different elements. If no theme is provided, the default black and green one will be used.

### Actions

This section is for launching one off programs. Perhaps you need to run your emulators to set them up. This is optional as well.

### emulators

The emulator section contains a list of emulators and the things that MicroFE needs to launch it and find the games.

* `System` - is the text that will appear inside MicroFE to represent the emulator.

* `WorkingDirectory` - is the absolute path where the emulator should be launched from. If the emulator path is `c:/Emulators/Mame/Mame.exe`, the  
`WorkingDirectory` should be `C:/Emulators/Mame`

* `EmuPath` - is the absolute path to the emulator's exe file.

* `EmuArgs` - is the collection of arguments that get passed to the emulator. This varries emulator to emulator. But for Retroarch, in the 
example file above, `-f` is for full screen, `-L` is for load core. `C:/Games/Emulation/Emulators/retroarch/Cores/nestopia_libretro.dll` 
is the core to load.

`%ROM%` is a special macro argument used by MicroFE. This is the name of the rom. This is passed in as an argument. Some emulators have special
syntax for passing in the name of the game to play. This is the only required emulator argument. 

* `RomPath` - is the directory where the games are stored.

* `RomFilter` is a filter for making sure that only games get displayed, and not extra files like playlists or save states. Most emulators will
handle `.zip` files just fine.


## Running MicroFE - Windows

## Setting Up MicroFE - Linux

## Running MicroFE - Linux

## Building MicroFE

