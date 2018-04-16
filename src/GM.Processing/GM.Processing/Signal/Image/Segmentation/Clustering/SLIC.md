# GM.Processing -> Signal -> Image -> Segmentation -> Clustering -> SLIC

**Implementation**: [Simple Linear Iterative Clustering (SLIC)](SLIC.cs)

**Paper**: [Radhakrishna Achanta, Appu Shaji, Kevin Smith, Aurelien
Lucchi, Pascal Fua, and Sabine S¨usstrunk, SLIC Superpixels, EPFL Technical
Report 149300, June 2010.](https://infoscience.epfl.ch/record/149300/files/SLIC_Superpixels_TR_2.pdf)

Implementation of Simple Linear Iterative Clustering (SLIC) algorithm for superpixel segmentation. The algorithm adapts a k-means clustering approach to efficiently generate superpixels.<br/>
Superpixel algorithms group pixels into perceptually meaningful atomic regions, which can be used to replace the rigid structure of the pixel grid. They capture image redundancy, provide a convenient primitive from which to compute image features, and greatly reduce the complexity of subsequent image processing tasks. They have become key building blocks of many computer vision algorithms, such as depth estimation, segmentation, body model estimation and object localization.

### Parameters:
- **k**: The desired number of approximately equally-sized superpixels.
- **m**: Controlls the compactness of the superpixels. It allows us to weigh the relative importance between color similarity and spatial proximity. When this value is large, spatial proximity is more important and the resulting superpixels are more compact (i.e. they have a lower area to perimeter ratio). When this value is small, the resulting superpixels adhere more tightly to image boundaries, but have less regular size and shape.

## Examples

![SLIC](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Honda.gif "Simple Linear Iterative Clustering (SLIC)")
![SLIC](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Lenna.gif "Simple Linear Iterative Clustering (SLIC)")
![SLIC](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Umbrella.gif "Simple Linear Iterative Clustering (SLIC)")
![original image](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20original.png "Original image")
![SLIC k=1024](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=1024.gif "Simple Linear Iterative Clustering (SLIC) k=1024")
![SLIC k=512](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=512.gif "Simple Linear Iterative Clustering (SLIC) k=512")
![SLIC k=256](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=256.gif "Simple Linear Iterative Clustering (SLIC) k=256")
![SLIC k=128](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=128.gif "Simple Linear Iterative Clustering (SLIC) k=128")
![SLIC k=64](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=64.gif "Simple Linear Iterative Clustering (SLIC) k=64")
![SLIC k=32](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=32.gif "Simple Linear Iterative Clustering (SLIC) k=32")
![SLIC k=16](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=16.gif "Simple Linear Iterative Clustering (SLIC) k=16")
![SLIC k=8](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=8.gif "Simple Linear Iterative Clustering (SLIC) k=8")
![SLIC k=4](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=4.gif "Simple Linear Iterative Clustering (SLIC) k=4")
![SLIC k=2](/Documentation/Signal/Image/Segmentation/Clustering/SLIC/SLIC%20Yamaha%20k=2.gif "Simple Linear Iterative Clustering (SLIC) k=2")

[Back to Clustering](/src/GM.Processing/GM.Processing/Signal/Image/Segmentation/Clustering)

## Author
Grega Mohorko ([www.mohorko.info](https://www.mohorko.info))
