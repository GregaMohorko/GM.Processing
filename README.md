# GM.Processing
.NET library with many algorithms and processing utilities of different science branches.

## Overview

The namespaces try to follow the same naming convention as in [this list of algorithms](https://en.wikipedia.org/wiki/List_of_algorithms).

### 1. Computer Science
#### 1.1. Computer graphics
- **Contour lines**
  - [SegmentContourLines](src/GM.Processing/GM.Processing/ComputerScience/ComputerGraphics/ContourLines/SegmentContourLines.cs)
### 2. Signal processing
 #### 2.1 Image processing
- **Contrast Enhancement**
  - [Histogram equalization (HE)](src/GM.Processing/GM.Processing/Signal/Image/ContrastEnhancement/HistogramEqualization.cs)
  - [Adaptive histogram equalization (AHE)](src/GM.Processing/GM.Processing/Signal/Image/ContrastEnhancement/AdaptiveHistogramEqualization.cs)
- **Segmentation**
  - **Clustering**
    - [Simple Linear Iterative Clustering (SLIC)](src/GM.Processing/GM.Processing/Signal/Image/Segmentation/Clustering/SLIC.cs)

## Requirements
.NET Framework 4.6.1

## Author and License
Grega Mohorko ([www.mohorko.info](https://www.mohorko.info))

Copyright (c) 2018 Grega Mohorko

[MIT License](./LICENSE)