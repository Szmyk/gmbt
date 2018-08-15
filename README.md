<h1 align="center">
  Gothic Mod Build Tool  
  <br>
</h1>

<h4 align="center">A simple tool designed to help in testing and building Gothic and Gothic 2 Night of the Raven mods.</h4>

<p align="center">
  <a href="https://github.com/Szmyk/gmbt/releases/latest">
    <img src="https://img.shields.io/github/release/szmyk/gmbt.svg" alt="Latest GitHub release">
  </a>
  
  <a href="https://github.com/Szmyk/gmbt/blob/master/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="License">
  </a>
  
  <a href="https://github.com/Szmyk/gmbt/releases/latest">
    <img src="https://img.shields.io/github/downloads/szmyk/gmbt/total.svg" alt="Downloads of Github Releases">
  </a>
  
  <a href="https://discord.gg/N4eGsJj">
    <img src="https://discordapp.com/api/guilds/472863935943409665/embed.png" alt="Join the Discord chat">
  </a>
  
  <a href="https://gitter.im/gothic-mod-build-tool">
    <img src="https://badges.gitter.im/Join%20Chat.svg" alt="Join the Gitter chat">
  </a>
                                                             
  <br>
</p>

This project was developed primarily for the purpose of assisting the SoulFire team with the development of [The Chronicles of Myrtana].

## Table of Contents

* [How does it work?](#how-does-it-work)
* [Status](#status)
* [Download](#download)
* [Installation & Requirements](#installation--requirements)
* [Configuration](#configuration)
    * [Example](#example)
* [Usage](#usage)
    *  [Common parameters](#common-parameters)
    *  [Common parameters](#common-parameters-test--build)
    *  [Verb commands](#verb-commands)
        * [`test`](#test)
        * [`build`](#build)
        * [`update`](#update)
    *  [Examples](#examples)
* [Advanced usage](#advanced-usage)
	*  [Hooks](#hooks)
	*  [Predefined options](#predefined-options)
* [Example project](#example-project)
* [License](#license)
* [Built with](#built-with)
* [Used software](#used-software)
* [Acknowledgments](#acknowledgments)
* [Contributing / issues  / contact](#contributing--issues--contact)

# How does it work?

Let's start with some background: the Gothic Mod Build Tool is kind of a breakthrough in Gothic modding, because it is one of the few successful attempts to create a build system which fully automates the process that was previously done manually, every modder had to manually compile assets like textures, meshes and animations and send them to their co-modders. Now, working with version control systems is possible, because each modder has the same version of assets at the same time and at any time can launch the game without need to build a *.mod* and not run into errors or discrepancies due to a lack of or mismatching assets.

This tool serves two very important purposes, to merge and to compile everything. It uses external tools for compiling textures, updating dialogs subtitles but also launches the Gothic game executable and compiles assets like 3D models and animations ingame.

## Modes

There are 3 modes of use:

* **Quick test** - merges assets directories, compiles the necessary assets to run the game, and launches the game. Not everything is compiled, so lag/stuttering can occur because of compiling textures, animations and 3D models "on the fly", in game. Used mainly to check if scripts are parsing when you are not using IDE/syntax checker. It could also be used if a full test is completed.
* **Full test** - compiles everything. This takes more time, but then you can play without problems like lag and stuttering.
* **Make VDF** - compiles everything and builds a *.mod*.

## Speed

On a mid-range PC with an HDD, a no sounds VDF build of a huge addon [The Chronicles of Myrtana] with around 70 MB of worlds, 800 MB of textures, 150 MB of animations and 3D models, takes about 9 - 10 minutes. Similar time with a full test (subtract about a half minute of packing the *.mod*).

# Status

The most important features of the tool are finalized, but of course you use it at your own risk. The tool has not yet been thoroughly tested.

# Download

| Latest stable release | Latest unstable release | Unstable development ([dev](https://github.com/Szmyk/gmbt/tree/dev) branch)
|:---------------------:| :----------------------:|:------------------------------------------------------------------------------:
[![GitHub release](https://img.shields.io/github/release/szmyk/gmbt.svg)](https://github.com/Szmyk/gmbt/releases/latest) | [![GitHub (pre-)release](https://img.shields.io/github/release/Szmyk/gmbt/all.svg)](https://github.com/Szmyk/gmbt/releases) | [![Build status (dev)](https://ci.appveyor.com/api/projects/status/0h4avwoh684c3tg2/branch/dev?svg=true)](https://ci.appveyor.com/project/Szmyk/gmbt/branch/dev/artifacts) 

# Installation & Requirements

* [Download](https://github.com/Szmyk/gmbt/releases/latest) latest version of the tool. The installer will install GMBT to `%APPDATA%\GMBT` and add this path to [`%PATH%`](https://en.wikipedia.org/wiki/PATH_(variable)) variable.

* [.NET Framework 4.7](https://www.microsoft.com/en-us/download/details.aspx?id=55170) is required to run the tool.

* A clean installation of vanilla *Gothic* or *Gothic 2 Night of the Raven* on your PC. You must have a **COMPLETELY** clean copy of game, with no mods, textures packs and other such.

* [Gothic Patch 1.08k](http://www.worldofgothic.de/dl/download_6.htm) or [Gothic II Report-Version 2.6f-rev2](https://www.worldofgothic.de/dl/download_278.htm) 

* [Gothic Player Kit v1.08k](http://www.worldofgothic.de/dl/download_34.htm) or [Gothic 2 Player Kit 2.6f](https://www.worldofgothic.de/dl/download_168.htm).

* Of course, you can also install [SystemPack](https://forum.worldofplayers.de/forum/threads/1340357-Release-Gothic-Ă‚Ëť-Ă‚â€”-SystemPack-%28ENG-DEU%29) if you have problems with the game on your PC.

After installation, you can run the Gothic copy **ONLY** via GMBT. Of course, you can use eg. Spacer, but you have to complete a full test before (the scripts have to be compiled because Spacer needs eg. `GOTHIC.DAT` and `CAMERA.DAT`).

Next you have to [configure paths](#configuration) and run the tool with the command you want ([usage guide](#usage)).

# Configuration

You have to configure a [YAML] config:

* **minimalVersion** - optional _string_
    > Minimal version of GMBT required to run the project. Eg. _v0.14.1_
* **gothicRoot** - _string_
    > Path to game root directory, eg. relative path (`..\..`) or absolute (`C:\Program Files\JoWood\Gothic 2 Gold Edition`)
* **modFiles**  - _structure_
    * **assets**  - _strings list_
        > Paths to assets directories which have to be placed in `_work/Data` directories. You have to prepare right structure inside these directories (same as in `_work/Data`: _\<dir\>\Anims_, _\<dir\>\Scripts_ and so on).
    * **exclude** - _strings list_
        > Exclude files from merging. Only files paths, not directories and wildcarts.
    * **defaultWorld**  - _string_
        > Name (not path) of ZEN, eg. _NEWWORLD.ZEN_
* **modVdf**  - _structure_
    *   **output**  - _string_
        > Path to save *.mod* file.
    *   **comment**  - _string_
        > VDF volume comment.
        >
        > Available special characters:
        >  -  _%%N_ - new line
        >  -  _%%D_ - date and time in UTC
    * **include** - _strings list_
        >  Include some files or directories (wildcarts enabled) to VDF. Path's root is game root, eg. `_work\Data\Scripts\Content\*.d`
    * **exclude** - _strings list_
        >  Exclude some files or directories (wildcarts enabled) from VDF. Path's root is game root, eg. `_work\Data\Worlds\Test\*.zen`
*  **gothicIniOverrides**  - _dictionary_
    > Keys of GOTHIC.INI you want to override when running test or build.
    >
    > Syntax: _['section.key', 'value']_ or _'section.key': 'target'_, eg. '`GAME.playLogoVideos' : '0'`

*  **install**  - _dictionary_
    > Optional files you want to install.
    >
    > Syntax: _[source, target]_ or _source: target_

### Example

Below is an example files structure and configuration used in [The Chronicles of Myrtana] project. Also, the same structure you can see in the [example project](#example-project).

Our developers have to clone modification repository to `_Work` directory, so their local repository is located in `_Work/TheChroniclesOfMyrtana`. We have a complex structure for our files. There are four directories:
* **mdk** - there are eg. original scripts, animations (if needed eg. to compile some new animations)
* **mdk-overrides** - there are overrides of original assets to maintain transparency and organization
* **mod** - own new assets, scripts, music and so on. There are only completely new files. Overrides of originals are in the `mdk-overrides`
* **thirdparty** - some resources from thirdparty libraries and projects on which we have license to use

We have got this config in root of the local repository (`_Work/TheChroniclesOfMyrtana/.gmbt.yml`).

```
gothicRoot: ..\..

modFiles:
  assets:
    - mdk
    - mdk-overrides
    - thirdparty
    - mod

  defaultWorld: KM_WORLD.ZEN

modVdf:
  output:  ..\..\Data\ModVDF\KM.mod
  comment: Gothic 2 - The Chronicles of Myrtana (%%D)%%N(C) 2018 SoulFire

  exclude:
    - _work\Data\Worlds\KM_SUBZENS\*

gothicIniOverrides:
  - 'GAME.playLogoVideos' : '0'
  - 'SKY_OUTDOOR.zSkyDome' : '1'
```

As you can see, there are only relative paths because this file is in our remote Git repository and every collaborator does not have to configure everything by themselves. And this is the main idea of this tool, to enable easy collaboration in version control systems and large teams.

# Usage

At this moment the only way to use the tool is command line interface. GUI application is planned, but [below](#examples) you can find simple examples of Windows Batch files.

## Common parameters 

| Parameter 								| Description 																																| Default value
| ----------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- | ---------
| `-L <en\|pl>, --lang=<en\|pl>`			| Set language of console output.																											| Control Panel -> Regional Settings
| `--help`								    | Print short descriptions of parameters.																									| N/A
| `-V <level>, --verbosity=<level>`		    | Set verbosity level of console output. Levels: quiet\|minimal\|normal\|detailed\|diagnostic.												| _normal_

## Common parameters (`test` & `build`)

| Parameter 								| Description 																																| Default value
| ----------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- | ---------
| `-C <path>, --config=<path>`				| Path of a config file. </br> Guide how to configure this file is [here](#configuration). 													| `.gmbt.yml` in working directory
| `--noupdatesubtitles`						| Do not update (convert to OU.csl) of dialogues subtitles. 																				| N/A
| `--zspy=<none\|low\|medium\|high>` 		| Level of zSpy logging.																													| _none_
| `--show-duplicated-subtitles` 			| Print duplicated subtitles.																												| N/A

## Verb commands

### `test`

> Starts test.

| Parameter 									| Description 																														| Default value
| --------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------
| `-F, --full`                           		| Full test mode. Information about modes is [here](#modes). 																		| N/A
| `--merge=<none\|all\|scripts\|worlds\|sounds>`| Merge option. <br/> Enter eg. `scripts` if you just want to debug scripts and do not want to copy all assets every time. Also, nice option to use is `scripts,worlds` if you want to debug some changes only related to scripts and worlds. | _all_
| `-W <zen>, --world=<zen>` 					| Run game in a specific world. 																									| Set in [config file](#configuration)
| `--windowed` 						 			| Run game in window. 																												| N/A
| `--noaudio` 					     			| Run game without any audio. 																										| N/A
| `--ingametime=<hh:mm>`						| Ingame time. <br/>Syntax: **hour:minute**, eg. _15:59_. 																			| N/A
| `--nodx11` 							 		| If [D3D11-Renderer for Gothic] is installed, this command allows you to temporarily disable this wrapper. 						| N/A
| `--nomenu` 									| Run game without menu showing (starts a new game immediately). 																	| N/A
| `-R, --reinstall`								| Reinstall before start.																											| N/A
| `-D, --devmode`								| Dev mode of game (marvin mode).																									| N/A

### `build`

> Starts a *.mod* build.

| Parameter 					| Description								    | Default value
| ---------------------------- 	| --------------------------------------------- | ------------------------------------
|`-O <file>, --output=<file>`  	| Path of VDF volume (`.mod`) output. 			| Set in [config file](#configuration)
|`--nopacksounds`  				| Do not to pack sounds (WAVs) to mod package.  | N/A
|`--comment`  					| Set or override comment of VDF.  				| Set in [config file](#configuration)

### `update`

> Updates the tool.

| Parameter 					| Description								    
| ---------------------------- 	| ---------------------------------------------
|`-F, --force`  				| Download and update even if it is up to date.
|`--no-confirm`  				| Do not ask for download.

## Examples

Example Batch scripts are also in [example project].

Below are some examples used developing [The Chronicles of Myrtana] project:

* **GMBT_QuickTest.bat**

    `gmbt test --windowed --noaudio`

* **GMBT_QuickTest_ScriptsDebug.bat**

  You can run something like this if you are debbuging scripts only and you do not want to copy all assets every time. Gothic is running windowed, without audio and without main menu.

    `gmbt test --windowed --noaudio --merge=scripts`

* **GMBT_QuickTest_ScriptsAndWorldsDebug.bat**

  Similar to previous with additional merging of worlds.

    `gmbt test --windowed --noaudio --merge=scripts,worlds`

* **GMBT_FullTest.bat**

  Full test. Gothic is running windowed.

    `gmbt test -F --windowed`

* **GMBT_BuildVDF.bat**

  Builds a *.mod*.

    `gmbt build`

# Example project

There is an [example project] which uses this tool. There are some assets from [World of Gothic DE Modderdatenbank] — just for the test. This repository is very nice sandbox which allows you to get acquainted with the tool. The repository has the same structure of files as in [example configuration](#example).

# Advanced usage

## Hooks

Hooks are actions can be set in [config file](#configuration) to trigger actions at certain points in the tool execution.

#### Modes

* `common` - executes in every instance
* `test` - executes when test (quick or full) is executing.
* `quickTest` - executes when quick test is executing
* `fullTest` - executes when full test is executing
* `build` - executes when build is executing

#### Types

* `pre` - before an event
* `post` - after an event

#### Events

* `assetsMerge`
* `subtitlesUpdate`

Hooks have to be set in config file. Example:

```yaml
hooks: 
    common: 
        post:
            - assetsMerge: "tools/script.bat"
    test: 
        pre:
            - subtitlesUpdate: "tools/script2.bat"
```

To better understand the entire process of tool execution, you should look at the diagram:

![](https://i.imgur.com/FMfMoOF.png)

## Predefined options

Predefined options is something like templates for command line parameters.

You can set predefined options sets in config file:

```yaml
predefined:
  - "world": "--world=WORLD.ZEN"
  - "anotherworld": "--world=WORLD2.ZEN"
```

and then, instead of calling `gmbt test --world=WORLD.ZEN`, you can type `gmbt test world`.

# License

```
MIT License

Copyright (c) 2018 Szymon 'Szmyk' Zak <szymonszmykzak@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

# Built with

* [**YamlDotNet**](https://github.com/aaubry/YamlDotNet) Copyright (c) 2008, 2009, 2010, 2011, 2012, 2013, 2014 Antoine Aubry and contributors
* [**CommandLineParser**](https://github.com/commandlineparser/commandline) Copyright (c) 2005 - 2015 Giacomo Stelluti Scala & Contributors
* [**Newtonsoft.Json**](https://github.com/JamesNK/Newtonsoft.Json) Copyright (c) 2007 James Newton-King

Licenses and disclaimers are in the [ThirdPartyNotices.md](https://github.com/Szmyk/gmbt/tree/master/ThirdPartyNotices.md) file.

# Used software

* **GothicVDFS 2.6** Copyright (c) 2001-2003, Nico Bendlin
* **Virtual Disk File System (VDFS)** Copyright (c) 1994-2002, Peter Sabath / TRIACOM Software GmbH
* **zSpy 2.05** Copyright (c) 1997-2000 Bert Speckels, Mad Scientists 1997
* **NSIS (Nullsoft Scriptable Install System)** Copyright (C) 1999-2018 Nullsoft and Contributors

Licenses and disclaimers are in the [tools](https://github.com/Szmyk/gmbt/tree/master/tools) directory.

# Acknowledgments

Big thanks to:

* the whole SoulFire team, especially ['oGon'](https://github.com/oGon991), ['Xardas49'](https://github.com/Xardas49), ['Komuch'](https://github.com/miwisniewski) and [Radoslaw 'Revo' Rak](https://github.com/revo16pl) for testing on the very early stage of production
* [Adam 'Avallach' Golebiowski](https://github.com/Avallach7) for idea of this tool and help on the very early stage of production
* [Mikolaj 'Miko' Sitarek](mailto:Nikolajek@hotmail.com) for proofreading

# Contributing / issues / contact

* [![Contributing](https://img.shields.io/badge/contributing-guidelines-blue.svg)](https://github.com/Szmyk/gmbt/tree/master/.github/CONTRIBUTING.md)
* [![GitHub issues](https://img.shields.io/github/issues/szmyk/gmbt.svg)](https://github.com/Szmyk/gmbt/issues) [![GitHub closed issues](https://img.shields.io/github/issues-closed/szmyk/gmbt.svg)](https://github.com/Szmyk/gmbt/issues)
* [![Join the Discord chat](https://discordapp.com/api/guilds/472863935943409665/embed.png)](https://discord.gg/N4eGsJj)  [![Join the chat](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/gothic-mod-build-tool?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) 

[The Chronicles of Myrtana]: https://kronikimyrtany.pl/en
[D3D11-Renderer for Gothic]: https://forum.worldofplayers.de/forum/threads/1441897-D3D11-Renderer-fÄ‚Ä˝r-Gothic-2-(alpha)-15
[YAML]: https://en.wikipedia.org/wiki/YAML
[World of Gothic DE Modderdatenbank]: https://www.worldofgothic.de/?go=moddb
[example project]: https://github.com/Szmyk/gmbt-example-mod
