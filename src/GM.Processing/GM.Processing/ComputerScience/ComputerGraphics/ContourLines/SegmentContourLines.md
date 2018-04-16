# GM.Processing -> Computer Science -> Computer Graphics -> Contour Lines -> Segment Contour Lines

**Implementation**: [Segment Contour Lines](SegmentContourLines.cs)

The most simple implementation of drawing contour lines. It simply compares each pixel to the 8 surrounding values (that are not yet determined to be part of a contour line) and draws a contour if at least 2 of them differs from the current pixel.<br/>
This method is actually only usable when the image is segmented.

## Examples

Lines between segments were drawn using this algorithm.

![SLIC k=1024](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=1024.gif "Simple Linear Iterative Clustering (SLIC) k=1024")
![SLIC k=32](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=32.gif "Simple Linear Iterative Clustering (SLIC) k=32")

[Back to Contour Lines](/src/GM.Processing/GM.Processing/ComputerScience/ComputerGraphics/ContourLines)

## Author
Grega Mohorko ([www.mohorko.info](https://www.mohorko.info))
