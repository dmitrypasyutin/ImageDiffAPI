# ImageDiff

Platform: Web API .NET Core 2.2

**Solution structure:**

1. ImageDiff.Api - API and Web pages implementations, Web-dependent services
2. ImageDiff.Data - data structures
3. ImageDiff.CommonAbstractions - service interfaces
4. ImageDiff.Core - core services. Currently contains MemoryCacheStorage, but service like Configuration management, Mapper, etc. also should be kept there.
5. ImageDiff.Services - task-specific web-independent services. Contains services working with images, algorithms for finding different objects, etc
6. Images.Api.Tests, ImagesDiff.Core.Tests, ImagesDiff.Data.Tests, ImagesDiff.Services.Tests - unti test projects

**API endpoints:**

1. api/comparesync - synchronous image comparison. Returns the response only after images are fully compared and all differences are found. The response contains the id of the resulted image, that can be used to acquire the image content
2. api/compareasync - asynchronous image comparison. Returns the response immediately,  before images are fully compared and differences are found. The response contains the id of the resulted image, percentage value indicating how much of the original images have been processed, and the version of the resulting image
3. api/imageprogress/[imageid] - returns the percentage, indicating how much of the original images have been processed, and the version of the resulting image. Can be used by clients to control the process of images asynchronous comparison
4. api/image/[imageid] - returns the resulting image content, showing rectangles around objects found



**Points of extensibility:**

1. ImageDiff.CommonAbstractions.IDiffObjectsFinder - defines an algorithm that finds differences on images. Current implementation BreadthFirstDiffObjectsFinder implements breadth-first search algorithm. Other implementation, such as depth-first or Hoshen–Kopelman algorithm could be provided.
2. ImageDiff.CommonAbstractions.IBitmapComparer - acquires corresponding pixels from the image and calls IPixelComparer.Equal() method
3. ImageDiff.CommonAbstractions.IPixelComparer - compare pixels content. Current solution contains 2 implementations: ARGBPixelComparer and RGBPixelComparer. Other implementations using different color spaces could be provided, if necessary
4. ImageDiff.CommonAbstractions.IImageGenerator - defines a service that generates a resulting image
5. ImageDiff.CommonAbstractions.IResultImageStorage - defines a storage that keeps and provides access to resulting image



**Ideas for further improvement:**

1. Current implementation finds differences using only 1 thread, but the algorithm could be parallelized. For example, the original images could be split in parts with each part processed by a separate thread, then the results are combined.
2. Alternative algorithms for searching differences could be used. For instance, Hoshen–Kopelman algorithm could be more appropriate for synchronous search, as it is more optimized than breadth-first or depth-first search. However, this algorithm cannot be used with asynchronous search, because the results are ready only after the whole image is processed
3. To save web traffic, the response may contain only coordinates of the rectangles of the found object. In that case, the client should be responsible for presenting final result. This approach is appropriate when API is supposed to process very large images.
4. Instead of using memory cache, disk drive could be used to store the resulting images
5. JPEG images may contain compression artifacts that surround objects on the image. Current algorithm detects those artifacts, thus spoiling the results. Parameterizing the algorithm to neglect those artifacts by excluding small found objects, may improve the final result.
6. Exception handling should be implemented via Exception filter rather than try catch blocks



