## [0.22] - 2024-01-27

### Improved
- Hooks: Log when return code is not 0. Log stdout and stderr on Detailed verbosity ([#42](https://github.com/Szmyk/gmbt/pull/42))
- Hooks: Halt execution when hook fails. Parameter to ignore failures
- Hooks: Add `hooks-forward-parameter` parameter
- Hooks: Add `VdfsPack` hook
- Pack: Add `skipmerge` parameter
- Options: Add `log` parameter

## [0.21] - 2023-12-03

### Improved
- Removed Rollbar crash reporting

## [0.20] - 2019-10-13

### New features
- Options: Add `nohooks` parameter

## [0.19] - 2019-08-07

### New features
- Options: Add `pack` mode ([#19](https://github.com/Szmyk/gmbt/issues/19))
- Options: Add `compile` mode ([#20](https://github.com/Szmyk/gmbt/issues/20))
- Options: Add `noreparse` parameter ([#22](https://github.com/Szmyk/gmbt/issues/22))
- zSpy: Add filtering of messages ([#21](https://github.com/Szmyk/gmbt/issues/21))
- Config: Add possibility to copy directories using `install` ([#15](https://github.com/Szmyk/gmbt/issues/15))

### Fixed
- Gothic: Fix InvalidOperationException unhandled exception
- Options: Fix predefined options ([#18](https://github.com/Szmyk/gmbt/issues/18))
- Test: Fix world existence detecting ([#17](https://github.com/Szmyk/gmbt/issues/17))

### Improved
- OU: Throwing exception when SRC file doesn't exists ([#16](https://github.com/Szmyk/gmbt/issues/16))

## [0.18] - 2019-05-15

### New features
- Options: Passing custom additional Gothic parameters

## [0.17.2] - 2019-04-07

### Fixed
- Fix enabling/disabling DX11 wrapper
- OU: Throwing exception when directory doesn't exists
- Config: Replace throwing exceptions with logging fatal errors
- Fix throwing exception when format of Gothic.ini override is wrong

## [0.17.1] - 2019-01-02

### Fixed
- Fix problem with LeGo's saves system (#14)
- Options: Add missing Slovak language info

### Other
- Update YamlDotNet to v5.0.0 (vulnerability founded)

## [0.17] - 2018-09-18

### New features
- Slovak localization

## [0.16.3] - 2018-09-17

### Fixed
- Fix default internationalization

## [0.16.2] - 2018-09-16

### Fixed
- Test: Fix subtitles updating condition

## [0.16.1] - 2018-08-27

### Fixed
- Logger: Creating `logs` directory if not exists

## [0.16] - 2018-08-26

### Fixed
- OU: Fix line endings
- OU: Handle SRC files errors

### Improved
- Logger: Add removing old logs
- Logger: Add auto flushing of stream
- Logger: Enhance logging exceptions
- Installer: Change compression to solid LZMA
- Installer: Deleting uninstall registry key before installation

## [0.15.1] - 2018-08-25

### Fixed
- Updater: Fix version comparing

## [0.15] - 2018-08-24

### New features
- Add [Rollbar](https://rollbar.com) integration
- Options: Add `devmode` parameter
- OU: Displaying dialogues duplicates (`--show-duplicated-subtitles` parameter)
- Config: Add `projectName` and `minimalVersion` keys
- Receiving zSpy messages in-app

### Improved
- Compiling textures by Gothic instead of external tools
- Replace extracting original assets with `vdfs:physicalfirst`
- Replace GothicVDFS with [VdfsSharp](https://github.com/Szmyk/VdfsSharp) library
- Detecting if ZEN selected to run is not exists (#10)
- Detecting wrong options combinations (#11)
- Gothic: Remove checking EXE header checksum
- Allow to declare many commands for each hook
- Replace NLog with own logger

### Fixed
- OU: Ignore empty comments

### Migration guide from v0.x to v0.15

- Add `projectName` string key in config file
- If you use hooks, you have to fix syntax. More information at hooks section in [ReadMe](https://github.com/Szmyk/gmbt/blob/master/README.md#hooks).

## [0.14.1] - 2018-08-12

### Fixed

- ignore unmatched properties in config parsing

## [0.14] - 2018-07-13

### New features

- `--comment` option (build mode)

### Improved

- Improve config changes detection

### Fixed

- Fix exceptions messages formatting in config file parsing

## [0.13] - 2018-05-06

### New features

- Add hooks system

### Fixed

- Fix "Could not create SSL/TLS secure channel" errors when checking if update is available

## [0.12] - 2018-01-31

### New features

- Add possibility to use zSpy in Build Mode
- New config sameness checking method (xxHash algorithm)

### Fixed

- Fix checking of another running instances of Gothic game
- Add download error handling in updater

## [0.11.1] - 2018-01-24

### Fixed

- Initializing of internationalization
- Case sensitive in verb commands and `--lang` option

## [0.11] - 2018-01-24

### New features

- New internationalization system

## [0.10] - 2018-01-20

### Added

- Simple in-app updater that checks and downloads releases from GitHub releases

### Fixed

- Setting config path in GOTHIC.INI. Now a config path is sets at the very beginning of installation to prevent errors when the assets do not unpack completely.

### Removed

- Unnecessary GOTHIC.INI overrides

## [0.9.3] - 2018-01-20

### Fixed

- Added missing meta value of `--output` option 

## [0.9.2] - 2018-01-14

### Fixed

- Path with which the `GothicVDFS.exe` is running
- Translation of `CompletedIn` key

## [0.9.1] - 2018-01-13

### Removed

- Unnecessary console output when merging of assets

[Unreleased]: https://github.com/szmyk/gmbt/compare/v0.22...HEAD
[0.22]: https://github.com/szmyk/gmbt/compare/v0.21...v0.22
[0.21]: https://github.com/szmyk/gmbt/compare/v0.20...v0.21
[0.20]: https://github.com/szmyk/gmbt/compare/v0.19...v0.20
[0.19]: https://github.com/szmyk/gmbt/compare/v0.18...v0.19
[0.18]: https://github.com/szmyk/gmbt/compare/v0.17.2...v0.18
[0.17.2]: https://github.com/szmyk/gmbt/compare/v0.17.1...v0.17.2
[0.17.1]: https://github.com/szmyk/gmbt/compare/v0.17...v0.17.1
[0.17]: https://github.com/szmyk/gmbt/compare/v0.16.3...v0.17
[0.16.3]: https://github.com/szmyk/gmbt/compare/v0.16.2...v0.16.3
[0.16.2]: https://github.com/szmyk/gmbt/compare/v0.16.1...v0.16.2
[0.16.1]: https://github.com/szmyk/gmbt/compare/v0.16...v0.16.1
[0.16]: https://github.com/szmyk/gmbt/compare/v0.15.1...v0.16
[0.15.1]: https://github.com/szmyk/gmbt/compare/v0.15...v0.15.1
[0.15]: https://github.com/szmyk/gmbt/compare/v0.14.1...v0.15
[0.14.1]: https://github.com/szmyk/gmbt/compare/v0.14...v0.14.1
[0.14]: https://github.com/szmyk/gmbt/compare/v0.13...v0.14
[0.13]: https://github.com/szmyk/gmbt/compare/v0.12...v0.13
[0.12]: https://github.com/szmyk/gmbt/compare/v0.11.1...v0.12
[0.11.1]: https://github.com/szmyk/gmbt/compare/v0.11...v0.11.1
[0.11]: https://github.com/szmyk/gmbt/compare/v0.10...v0.11
[0.10]: https://github.com/szmyk/gmbt/compare/v0.9.3...v0.10
[0.9.3]: https://github.com/szmyk/gmbt/compare/v0.9.2...v0.9.3
[0.9.2]: https://github.com/szmyk/gmbt/compare/v0.9.1...v0.9.2
[0.9.1]: https://github.com/szmyk/gmbt/compare/v0.9...v0.9.1
