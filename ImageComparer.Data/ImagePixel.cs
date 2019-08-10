using System;
using System.Collections.Generic;

namespace ImageDiff.Data
{
    public struct ImagePixel : IEquatable<ImagePixel>
    {
        public readonly int X;
        public readonly int Y;

        public ImagePixel(int x, int y)
        {
            X = x;
            Y = y;
        }

        public ICollection<ImagePixel> GetNeighbouringPixels(int imageWidth, int imageHeight)
        {
            List<ImagePixel> result = new List<ImagePixel>();

            if (!IsLeftEdge())
                result.Add(GetLeftNeighbour());

            if (!IsTopEdge())
                result.Add(GetTopNeighbour());

            if (!IsTopEdge() && !IsLeftEdge())
                result.Add(GetTopLeftNeighbour());

            if (!IsRightEdge(imageWidth))
                result.Add(GetRightNeighbour());

            if (!IsTopEdge() && !IsRightEdge(imageWidth))
                result.Add(GetTopRightNeighbour());

            if (!IsBottomEdge(imageHeight))
                result.Add(GetBottomNeighbour());

            if (!IsRightEdge(imageWidth) && !IsBottomEdge(imageHeight))
                result.Add(GetBottomRightNeighbour());

            if (!IsLeftEdge() && !IsBottomEdge(imageHeight))
                result.Add(GetBottomLeftNeighbour());

            return result;
        }

        public ImagePixel GetLeftNeighbour()
        {
            return new ImagePixel(X - 1, Y);
        }

        public bool IsLeftEdge()
        {
            return X == 0;
        }

        public bool IsTopEdge()
        {
            return Y == 0;
        }

        public ImagePixel GetTopNeighbour()
        {
            return new ImagePixel(X, Y - 1);
        }

        public ImagePixel GetTopLeftNeighbour()
        {
            return new ImagePixel(X - 1, Y - 1);
        }

        public bool IsRightEdge(int imageWidth)
        {
            return X == imageWidth - 1;
        }

        public ImagePixel GetRightNeighbour()
        {
            return new ImagePixel(X + 1, Y);
        }

        public ImagePixel GetTopRightNeighbour()
        {
            return new ImagePixel(X + 1, Y - 1);
        }

        public bool IsBottomEdge(int imageHeight)
        {
            return Y == imageHeight - 1;
        }

        public ImagePixel GetBottomNeighbour()
        {
            return new ImagePixel(X, Y + 1);
        }

        public ImagePixel GetBottomRightNeighbour()
        {
            return new ImagePixel(X + 1, Y + 1);
        }

        public ImagePixel GetBottomLeftNeighbour()
        {
            return new ImagePixel(X - 1, Y + 1);
        }

        public override int GetHashCode()
        {
            int hCode = X ^ Y;
            return hCode.GetHashCode();
        }

        public bool Equals(ImagePixel other)
        {
            return X == other.X && Y == other.Y;
        }

        public static bool operator ==(ImagePixel lhs, ImagePixel rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ImagePixel lhs, ImagePixel rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
