using System.Linq;
using NUnit.Framework;

namespace ImageDiff.Data.Tests
{
    [TestFixture]
    public class PixelTests
    {
        [Test]
        public void Test_Constructor()
        {
            int x = 1;
            int y = 1;

            var pixel = new ImagePixel(x, y);
            Assert.AreEqual(pixel.X, x);
            Assert.AreEqual(pixel.Y, y);
        }

        [Test]
        public void Get_Neighbours_In_Middle_Of_Image()
        {
            var pixel = new ImagePixel(1, 1);
            var neightbours = pixel.GetNeighbouringPixels(3, 3);

            Assert.AreEqual(neightbours.Count, 8);
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 0 && f.Y == 0));
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 1 && f.Y == 0));
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 2 && f.Y == 0));
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 0 && f.Y == 1));
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 2 && f.Y == 1));
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 0 && f.Y == 2));
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 1 && f.Y == 2));
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 2 && f.Y == 2));
        }

        [Test]
        public void Get_Neighbours_At_The_Edge_Of_Image()
        {
            var pixel = new ImagePixel(0, 0);
            var neightbours = pixel.GetNeighbouringPixels(3, 3);

            Assert.AreEqual(neightbours.Count, 3);
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 1 && f.Y == 0));
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 1 && f.Y == 1));
            Assert.IsNotNull(neightbours.FirstOrDefault(f => f.X == 0 && f.Y == 1));
        }

        [Test]
        public void Is_Left_Edge()
        {
            var pixel = new ImagePixel(0, 0);
            Assert.IsTrue(pixel.IsLeftEdge());

            pixel = new ImagePixel(1, 0);
            Assert.IsFalse(pixel.IsLeftEdge());
        }

        [Test]
        public void Get_Left_Neighbour()
        {
            var pixel = new ImagePixel(1, 1);
            var leftNeighbour = pixel.GetLeftNeighbour();
            Assert.AreEqual(leftNeighbour, new ImagePixel(0, 1));
        }

        [Test]
        public void Is_Right_Edge()
        {
        }

        [Test]
        public void Get_Right_Neighbour()
        {
        }

        //...etc
    }
}