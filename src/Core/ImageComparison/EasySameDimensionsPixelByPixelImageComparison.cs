using SkiaSharp;

namespace Testar.ChangeDetection.Core.ImageComparison;

public class EasySameDimensionsPixelByPixelImageComparison : ICompareImages
{
    public byte[] Comparer(string controlFileName, string testFileName)
    {
        var controlImage = SKBitmap.Decode(controlFileName);
        var testImage = SKBitmap.Decode(testFileName);

        if (controlImage.Height != testImage.Height || controlImage.Width != testImage.Width)
        {
            Array.Empty<byte>();
        }

        var comparisonImage = new SKBitmap(controlImage.Width, controlImage.Height);

        for (var x = 0; x < controlImage.Width; x++)
        {
            for (var y = 0; y < controlImage.Height; y++)
            {
                var controlPixel = controlImage.GetPixel(x, y);
                var testPixel = testImage.GetPixel(x, y);

                var colour = controlPixel.Equals(testPixel)
                    ? SKColor.Parse("#000000")
                    : SKColor.Parse(testPixel.ToString());

                comparisonImage.SetPixel(x, y, colour);
            }
        }

        return comparisonImage
            .Encode(SKEncodedImageFormat.Png, 50)
            .ToArray();
    }
}