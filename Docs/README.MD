# Configuring Cache Paths

By default, Precursor is setup to pull the paths for each supported engine version from the specified input file.

The paths for each build should only contain the cache files themselves, and any other dependencies located inside the `maps` folder of most Blam! engine titles (fonts, map info, images, texture or sound resource files, etc).

The Blam! titles supported by Precursor line up roughly with engine versions currently supported by TagTool-NET.

## 

> Default cache paths refer to the default paths defined in the `cache_paths.cmds` script. These are purely to be used as a reference for how to structure the builds, though realistically any file path will function correctly.

> If multiple builds reference the same default path, it means that anyone of those builds can be placed in the input path, and Precursor will still recognise it as a valid build for the specified TagTool Cache Version.

&nbsp;

### Gen1
| Build    | TagTool Cache Version | Default Cache Path |
| -------- | --------------------- | ----------------- |
| 01.09.25.2247 | HaloXbox | Gen1\HaloXbox |
| 01.10.12.2276 | HaloXbox | Gen1\HaloXbox |
| 01.00.00.0564 | HaloPC | Gen1\HaloPC |
| 01.00.00.0609 | HaloCustomEdition | Gen1\HaloCustomEdition |

### Gen2
| Build    | TagTool Cache Version | Default Cache Path |
| -------- | --------------------- | ----------------- |
| 02.01.07.4998 | Halo2Alpha | Gen2\Halo2Alpha |
| 02.06.28.07902 | Halo2Beta | Gen2\Halo2Beta |
| 02.09.27.09809 | Halo2Xbox | Gen2\Halo2Xbox |
| 11081.07.04.30.0934.main | Halo2Vista | Gen2\Halo2Vista |
| 11091.07.05.11.1104.main | Halo2Vista | Gen2\Halo2Vista |
| 11122.07.08.24.1808.main | Halo2Vista | Gen2\Halo2Vista |

### Gen3
| Build    | TagTool Cache Version | Default Cache Path |
| -------- | --------------------- | ----------------- |
| 09699.07.05.01.1534.delta | Halo3Beta | Gen3\Halo3Beta |
| 11855.07.08.20.2317.halo3_ship | Halo3Retail | Gen3\Halo3Retail |
| 12065.08.08.26.0819.halo3_ship | Halo3Retail | Gen3\Halo3MythicRetail |
| 13895.09.04.27.2201.atlas_relea | Halo3ODST | Gen3\Halo3ODST |
| 11860.10.07.24.0147.omaha_relea | HaloReach | Gen3\HaloReach |
| 11883.10.10.25.1227.dlc_1_ship__tag_test | HaloReach11883 | Gen3\HaloReach11883 |

### Gen4
| Build    | TagTool Cache Version | Default Cache Path |
| -------- | --------------------- | ----------------- |
| 20810.12.09.22.1647.main | Halo4 | Gen4\Halo4 |
| 21122.12.11.21.0101.main | Halo4 | Gen4\Halo4 |
| 21165.12.12.12.0112.main | Halo4 | Gen4\Halo4 |
| 21339.13.02.05.0117.main | Halo4 | Gen4\Halo4 |
| 21391.13.03.13.1711.main | Halo4 | Gen4\Halo4 |
| 21401.13.04.23.1849.main | Halo4 | Gen4\Halo4 |
| 21501.13.08.06.2311.main | Halo4 | Gen4\Halo4 |
| 21522.13.10.17.1936.main | Halo4 | Gen4\Halo4 |

### GenHaloOnline
| Build    | TagTool Cache Version | Default Cache Path |
| -------- | --------------------- | ----------------- |
| ElDewrito 0.7.1 | HaloOnlineED | GenHaloOnline\HaloOnlineED |
| 1.106708 cert_ms23  | HaloOnline106708 | GenHaloOnline\HaloOnline106708 |
| 1.235640 cert_ms23  | HaloOnline235640 | GenHaloOnline\HaloOnline235640 |
| 0.0.1.301003 cert_MS26_new | HaloOnline301003 | GenHaloOnline\HaloOnline301003 |
| 0.4.1.327043 cert_MS26_new | HaloOnline327043 | GenHaloOnline\HaloOnline327043 |
| 8.1.372731 Live | HaloOnline372731 | GenHaloOnline\HaloOnline372731 |
| 0.0.416097 Live | HaloOnline416097 | GenHaloOnline\HaloOnline416097 |
| 10.1.430475 Live | HaloOnline430475 | GenHaloOnline\HaloOnline430475 |
| 10.1.454665 Live | HaloOnline454665 | GenHaloOnline\HaloOnline454665 |
| 10.1.449175 Live | HaloOnline449175 | GenHaloOnline\HaloOnline449175 |
| 11.1.498295 Live | HaloOnline498295 | GenHaloOnline\HaloOnline498295 |
| 11.1.530605 Live | HaloOnline530605 | GenHaloOnline\HaloOnline530605 |
| 11.1.532911 Live | HaloOnline532911 | GenHaloOnline\HaloOnline532911 |
| 11.1.554482 Live | HaloOnline554482 | GenHaloOnline\HaloOnline554482 |
| 11.1.571627 Live | HaloOnline571627 | GenHaloOnline\HaloOnline571627 |
| 11.1.604673 cert_ms29 Live | HaloOnline604673 | GenHaloOnline\HaloOnline604673 |
| 12.1.700123 cert_ms30_oct19 | HaloOnline700123 | GenHaloOnline\HaloOnline700123 |

&nbsp;

### MCC

MCC cache files are handled differently from all other builds. Since MCC uses a static folder structure for all its supported engine versions, the only input required is the path to the base MCC installation directory.

| Build    | TagTool Cache Version |
| -------- | --------------------- |
| 01.03.43.0000 | HaloCustomEdition |
| Lastest Build* | Halo2Retail |
| Dec 21 2023 22:31:37 | Halo3Retail |
| May 16 2023 11:44:41 | Halo3ODST |
| Jun 21 2023 15:35:31 | HaloReach |
| Apr  1 2023 17:35:22 | Halo4 |
| Jun 13 2023 20:21:18 | Halo2AMP |

##### * Halo 2 Classic MCC lacks a build string in the cache file header, so the supported builds only include the latest release version (as of 05/05/2025)

##
### Input File Setup Guide Coming Soon
##