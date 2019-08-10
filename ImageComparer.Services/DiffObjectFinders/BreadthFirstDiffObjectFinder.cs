using System;
using System.Collections.Generic;
using System.Drawing;
using ImageDiff.CommonAbstractions;
using ImageDiff.Data;
using Rectangle = ImageDiff.Data.Rectangle;

namespace ImageDiff.Services.DiffObjectFinders
{
    public class BreadthFirstDiffObjectsFinder : IDiffObjectsFinder
    {
        private readonly IBitmapComparer _imageComparer;
        private Bitmap _image1;
        private Bitmap _image2;
        private byte[,] _imagePixelsStatus;
        private byte _foundObjectLabel = (byte) PixelStatus.Equal + 1;
        private readonly Queue<ImagePixel> _pixelsQueue = new Queue<ImagePixel>();

        public BreadthFirstDiffObjectsFinder(IBitmapComparer imageComparer)
        {
            _imageComparer = imageComparer ?? throw new ArgumentException(nameof(imageComparer));
        }

        public IEnumerable<Rectangle> FindAllDiffObjects(Bitmap image1, Bitmap image2)
        {
            if(image1 is null) throw new ArgumentException(nameof(image1));
            if(image2 is null) throw new ArgumentException(nameof(image2));
            
            Initialize(image1, image2);
            for (int yCoord = 0; yCoord < ImageHeight; yCoord++)
            {
                for (int xCoord = 0; xCoord < ImageWidth; xCoord++)
                {
                    var currentCoordinate = new ImagePixel(xCoord, yCoord);

                    if (_imagePixelsStatus[yCoord, xCoord] == (byte)PixelStatus.NotChecked)
                    {
                        if (_imageComparer.PixelsEqual(image1, image2, currentCoordinate))
                        {
                            _imagePixelsStatus[yCoord, xCoord] = (byte)PixelStatus.Equal;
                        }
                        else
                        {
                            _imagePixelsStatus[yCoord, xCoord] = _foundObjectLabel;
                            yield return GetObjectRectangle(currentCoordinate);
                            _foundObjectLabel++;
                        }
                    }
                }
            }
        }

        private void Initialize(Bitmap image1, Bitmap image2)
        {
            _image1 = image1 ?? throw new ArgumentException("Image1 cannot be null");
            _image2 = image2 ?? throw new ArgumentException("Image2 cannot be null");
            if (_imagePixelsStatus == null)
                _imagePixelsStatus = new byte[ImageHeight, ImageWidth];
        }

        private int ImageHeight => _image1?.Height ?? 0;
        private int ImageWidth => _image1?.Width ?? 0;


        /// <summary>
        /// Finds object's coordinates starting from the passed in parameter. Uses breadth-first search algorithm
        /// </summary>
        /// <param name="initialPixel">The initial coordinate of a found object</param>
        /// <returns>Rectangle surrounding the found object</returns>
        private Rectangle GetObjectRectangle(ImagePixel initialPixel)
        {
            ImagePixel rectangleTopLeft = initialPixel;
            ImagePixel rectangleBottomRight = initialPixel;

            _pixelsQueue.Enqueue(initialPixel);

            do
            {
                var pixel = _pixelsQueue.Dequeue();
                UpdateTopLeftRectangleCoordinate(ref rectangleTopLeft, pixel);
                UpdateBottomRightRectangleCoordinate(ref rectangleBottomRight, pixel);
                AnalyzeNeighbourPixels(pixel);
            } while (_pixelsQueue.Count > 0);

            return new Rectangle(rectangleTopLeft, rectangleBottomRight);
        }

        private void UpdateTopLeftRectangleCoordinate(ref ImagePixel topLeftPixel, ImagePixel currentPixel)
        {
            if (currentPixel.X < topLeftPixel.X)
            {
                topLeftPixel = new ImagePixel(currentPixel.X, topLeftPixel.Y);
            }
            else if (currentPixel.Y < topLeftPixel.Y)
            {
                topLeftPixel = new ImagePixel(topLeftPixel.X, currentPixel.Y);
            }
        }

        private void UpdateBottomRightRectangleCoordinate(ref ImagePixel bottomRightPixel, ImagePixel currentPixel)
        {
            if (currentPixel.X > bottomRightPixel.X)
            {
                bottomRightPixel = new ImagePixel(currentPixel.X, bottomRightPixel.Y);
            }
            else if (currentPixel.Y > bottomRightPixel.Y)
            {
                bottomRightPixel = new ImagePixel(bottomRightPixel. X, currentPixel.Y);
            }
        }

        private void AnalyzeNeighbourPixels(ImagePixel pixel)
        {
            foreach (var neighbourCoord in pixel.GetNeighbouringPixels(ImageWidth, ImageHeight))
            {
                AnalyzePixel(neighbourCoord);
            }
        }

        private void AnalyzePixel(ImagePixel pixel)
        {
            if (_imagePixelsStatus[pixel.Y, pixel.X] == (byte)PixelStatus.NotChecked)
            {
                if (_imageComparer.PixelsEqual(_image1, _image2, pixel))
                {
                    _imagePixelsStatus[pixel.Y, pixel.X] = (byte)PixelStatus.Equal;
                }
                else
                {
                    _imagePixelsStatus[pixel.Y, pixel.X] = _foundObjectLabel;
                    _pixelsQueue.Enqueue(pixel);
                }
            }
        }
    }
}