# GM.Processing
.NET library with many algorithms and processing utilities of different science branches.

## Overview

The namespaces try to follow the same naming convention as in [this list of algorithms](https://en.wikipedia.org/wiki/List_of_algorithms).

### 1. [Computer Science](src/GM.Processing/GM.Processing/ComputerScience)
#### 1.1. [Computer graphics](src/GM.Processing/GM.Processing/ComputerScience/ComputerGraphics)
- [**Contour lines**](src/GM.Processing/GM.Processing/ComputerScience/ComputerGraphics/ContourLines)
  - [SegmentContourLines](src/GM.Processing/GM.Processing/ComputerScience/ComputerGraphics/ContourLines/SegmentContourLines.md)
### 2. [Signal processing](src/GM.Processing/GM.Processing/Signal)
 #### 2.1 [Image processing](src/GM.Processing/GM.Processing/Signal/Image)
- [**Bracketing**](src/GM.Processing/GM.Processing/Signal/Image/Bracketing)
  - [Exposure Bracketing](src/GM.Processing/GM.Processing/Signal/Image/Bracketing/ExposureBracketing.md)
- [**Contrast Enhancement**](src/GM.Processing/GM.Processing/Signal/Image/ContrastEnhancement)
  - [Histogram equalization (HE)](src/GM.Processing/GM.Processing/Signal/Image/ContrastEnhancement/HistogramEqualization.md)
  - [Adaptive histogram equalization (AHE)](src/GM.Processing/GM.Processing/Signal/Image/ContrastEnhancement/AdaptiveHistogramEqualization.md)
- [**Segmentation**](src/GM.Processing/GM.Processing/Signal/Image/Segmentation)
  - [**Clustering**](src/GM.Processing/GM.Processing/Signal/Image/Segmentation/Clustering)
    - [Simple Linear Iterative Clustering (SLIC)](src/GM.Processing/GM.Processing/Signal/Image/Segmentation/Clustering/SLIC.md)

## Requirements
.NET Framework 4.6.1

## Author and License
Grega Mohorko ([www.mohorko.info](https://www.mohorko.info))

Copyright (c) 2018 Grega Mohorko

[MIT License](./LICENSE)
