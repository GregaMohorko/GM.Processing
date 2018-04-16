# GM.Processing -> Signal -> Image -> Bracketing -> Exposure Bracketing

**Implementation**: [Exposure Bracketing](ExposureBracketing.cs)

**Paper**: [Paul E. Debevec and Jitendra Malik. Recovering High Dynamic Range Radiance Maps from Photographs. In SIGGRAPH 97, August 1997](http://www.pauldebevec.com/Research/HDR/debevec-siggraph97.pdf)

**Wikipedia**: https://en.wikipedia.org/wiki/Bracketing#Exposure_bracketing

Implementation of Exposure Bracketing, a postprocessing technique to create a high dynamic range image that exposes different portions of the image by different amounts.<br/>
Exposure bracketing usually deals with high-contrast subjects.<br/>
The photographer chooses to take one picture at a given exposure, one or more brighter, and one or more darker, in order to select the most satisfactory image.

### Parameters:
- **N**: Number of random pixel locations to sample from. Computational complexity considerations make it impractical to use every pixel location in this algorithm.
- **smoothness**: Determines the amount of smoothness. It weights the smoothness term relative to the data fitting term, and should be chosen appropriately for the amount of noise expected in the measurements.

## Examples

![EXPOSURE BRACKETING](/Documentation/Signal/Image/Bracketing/ExposureBracketing/EXPOSURE%20BRACKETING%20Corridor.gif "Exposure Bracketing")
![EXPOSURE BRACKETING](/Documentation/Signal/Image/Bracketing/ExposureBracketing/EXPOSURE%20BRACKETING%20Desktop01.gif "Exposure Bracketing")
![EXPOSURE BRACKETING](/Documentation/Signal/Image/Bracketing/ExposureBracketing/EXPOSURE%20BRACKETING%20Desktop02.gif "Exposure Bracketing")
![EXPOSURE BRACKETING](/Documentation/Signal/Image/Bracketing/ExposureBracketing/EXPOSURE%20BRACKETING%20Digimax%20Gate.gif "Exposure Bracketing")
![EXPOSURE BRACKETING](/Documentation/Signal/Image/Bracketing/ExposureBracketing/EXPOSURE%20BRACKETING%20Scene.gif "Exposure Bracketing")
![EXPOSURE BRACKETING](/Documentation/Signal/Image/Bracketing/ExposureBracketing/EXPOSURE%20BRACKETING%20Window.gif "Exposure Bracketing")

[Back to Bracketing](/src/GM.Processing/GM.Processing/Signal/Image/Bracketing)

## Author
Grega Mohorko ([www.mohorko.info](https://www.mohorko.info))
