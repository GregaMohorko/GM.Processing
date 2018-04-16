# GM.Processing -> Signal -> Image -> Contrast Enhancement -> HE / CLHE

**Implementation**: [(Contrast Limited) Histogram Equalization (HE / CLHE)](HistogramEqualization.cs)

**Wikipedia**: https://en.wikipedia.org/wiki/Histogram_equalization

Implementation of (Contrast Limited) Histogram equalization (HE (or CLHE if limited)) algorithm that improves image contrast using the image's histogram.<br/>
This method is useful in images with backgrounds and foregrounds that are both bright or both dark, where the distribution of pixel values is similar throughout the image.

### Parameters:
- **clip limit**: A value that limits the amplification.

## Examples

![HE & CLHE](/Documentation/Signal/Image/Contrast%20Enhancement/HE/HE%20&%20CLHE%20Church.gif "(Contrast Limited) Histogram Equalization ((CL)HE)")
![HE & CLHE](/Documentation/Signal/Image/Contrast%20Enhancement/HE/HE%20&%20CLHE%20Courtyard.gif "(Contrast Limited) Histogram Equalization ((CL)HE)")

[Back to Contrast Enhancement](/src/GM.Processing/GM.Processing/Signal/Image/ContrastEnhancement)

## Author
Grega Mohorko ([www.mohorko.info](https://www.mohorko.info))
