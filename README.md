# MangaRipper

This software helps you download manga (Japanese Comic) from several websites for your offline viewing.

| BRANCH | STATUS |
| --- | --- |
| master | [![Build status](https://ci.appveyor.com/api/projects/status/92scfmfbep7b9ieo/branch/master?svg=true)](https://ci.appveyor.com/project/NguyenDanPhuong/mangaripper/branch/master) |
| develop | [![Build status](https://ci.appveyor.com/api/projects/status/92scfmfbep7b9ieo/branch/develop?svg=true)](https://ci.appveyor.com/project/NguyenDanPhuong/mangaripper/branch/develop) |

### Supported Sites:
- KissManga
- MangaFox
- MangaHere
- Batoto
- MangaReader
- MangaShare
- MangaStream

### [Download](https://github.com/NguyenDanPhuong/MangaRipper/releases/latest)

### [Wiki](https://github.com/NguyenDanPhuong/MangaRipper/wiki)

Help is appreciated. Please create pull request to develop branch.

## Software Design:

### Overview:

![Image of Yaktocat](Document/Overview.png)

- MangaRipper.Core:
    - The core of application.
    - Define all interfaces and providing ultility.
- Plugin Manager:
    - Load plugins and configuration from dll files.
    - Return correct plugin for inputed url. Fox ex: user input a url of a manga on mangafox.com. It returns MangaFox plugin.
- Plugin:
    - Ex: MangaFox plugin support to parse the html of mangafox.com to get the chapters and images information.
    - There's several plugins to choose from.
- UI (Winform):
    - The UI build on Winform. In the future we may create multiple UI along with Winform. Ex: Console, WPF...