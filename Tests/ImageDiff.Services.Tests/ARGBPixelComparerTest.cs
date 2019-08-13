using System.Drawing;
using ImageDiff.Services.PixelComparers;
using NUnit.Framework;

namespace ImageDiff.Services.Tests
{
    public class ARGBPixelComparerTest
    {
        [Test]
        public void Equal_True_When_Colors_Are_Same()
        {
            var comparer = new ARGBPixelComparer(0);
            Assert.IsTrue(comparer.Equal(Color.White, Color.White));
            Assert.IsFalse(comparer.Equal(Color.White, Color.Black));
        }
    }
}