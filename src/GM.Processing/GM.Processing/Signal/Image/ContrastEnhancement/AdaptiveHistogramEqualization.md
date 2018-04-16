# GM.Processing -> Signal -> Image -> Contrast Enhancement -> AHE / CLAHE

**Implementation**: [(Contrast Limited) Adaptive Histogram Equalization (AHE / CLAHE)](AdaptiveHistogramEqualization.cs)

**Wikipedia**: https://en.wikipedia.org/wiki/Adaptive_histogram_equalization

Implementation of (Contrast Limited) Adaptive histogram equalization (AHE (or CLAHE if limited)) algorithm that improves image contrast using the image's histogram and also adapts to local changes in contrast.<br/>
This method is useful in images that contain regions that are significantly lighter or darker than most of the image. It is suitable for improving the local contrast and enhancing the definitions of edges in each region of an image.

### Parameters:
- **clip limit**: A value that limits the amplification.
- **window size**: The size of the neighbourhood region. It constitutes a characteristic length scale: contrast at smaller scales is enhanced, while contrast at larger scales is reduced.

## Examples

![AHE & CLAHE](/Documentation/Signal/Image/Contrast%20Enhancement/AHE/AHE%20&%20CLAHE%20Schonbrunn%20garden%201.gif "(Contrast Limited) Adaptive Histogram Equalization ((CL)AHE)")
![AHE & CLAHE](/Documentation/Signal/Image/Contrast%20Enhancement/AHE/AHE%20&%20CLAHE%20Schonbrunn%20garden%202.gif "(Contrast Limited) Adaptive Histogram Equalization ((CL)AHE)")
![AHE & CLAHE](/Documentation/Signal/Image/Contrast%20Enhancement/AHE/AHE%20&%20CLAHE%20Schonbrunn%20entrance.gif "(Contrast Limited) Adaptive Histogram Equalization ((CL)AHE)")

[Back to Contrast Enhancement](/src/GM.Processing/GM.Processing/Signal/Image/ContrastEnhancement)

## Author
Grega Mohorko ([www.mohorko.info](https://www.mohorko.info))
