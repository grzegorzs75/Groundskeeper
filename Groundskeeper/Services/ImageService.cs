using SkiaSharp;

namespace Groundskeeper.Services;

public interface IImageService
{
    Task<ProcessedImage> SaveImageAsync(
        Stream inputStream,
        string originalFileName,
        string relativeDirectory);
}

public class ProcessedImage
{
    /// <summary>
    /// Generated filename of the stored JPEG.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Web-relative path, for example:
    /// /uploads/2026/08/3fa85f64....jpg
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;

    private const int MaxDimension = 1800;
    private const int JpegQuality = 80;

    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<ProcessedImage> SaveImageAsync(
        Stream inputStream,
        string originalFileName,
        string relativeDirectory)
    {
        if (inputStream == null)
            throw new ArgumentNullException(nameof(inputStream));

        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new ArgumentException(
                "Original filename must be supplied.",
                nameof(originalFileName));

        if (string.IsNullOrWhiteSpace(relativeDirectory))
            throw new ArgumentException(
                "Destination directory must be supplied.",
                nameof(relativeDirectory));

        // Read the uploaded file into memory.
        using var memoryStream = new MemoryStream();

        await inputStream.CopyToAsync(memoryStream);

        memoryStream.Position = 0;

        // Decode the image.
        using var originalBitmap = SKBitmap.Decode(memoryStream);

        if (originalBitmap == null)
            throw new InvalidOperationException(
                "The uploaded file could not be decoded as an image.");

        var sourceWidth = originalBitmap.Width;
        var sourceHeight = originalBitmap.Height;

        // Work out the required output dimensions.
        var (targetWidth, targetHeight) =
            CalculateDimensions(
                sourceWidth,
                sourceHeight,
                MaxDimension);

        SKBitmap outputBitmap;

        if (targetWidth != sourceWidth ||
            targetHeight != sourceHeight)
        {
            outputBitmap = originalBitmap.Resize(
                new SKImageInfo(
                    targetWidth,
                    targetHeight,
                    originalBitmap.ColorType,
                    originalBitmap.AlphaType),
                SKSamplingOptions.Default);

            if (outputBitmap == null)
                throw new InvalidOperationException(
                    "The image could not be resized.");
        }
        else
        {
            // We don't own originalBitmap independently here,
            // so create a copy for consistent disposal below.
            outputBitmap = originalBitmap.Copy();
        }

        using (outputBitmap)
        {
            using var image = SKImage.FromBitmap(outputBitmap);

            using var encodedData = image.Encode(
                SKEncodedImageFormat.Jpeg,
                JpegQuality);

            if (encodedData == null)
                throw new InvalidOperationException(
                    "The image could not be encoded as JPEG.");

            // We generate our own filename rather than trusting
            // the filename supplied by the browser.
            var storedFileName =
                $"{Guid.NewGuid():N}.jpg";

            var physicalDirectory = Path.Combine(
                _environment.WebRootPath,
                relativeDirectory);

            Directory.CreateDirectory(physicalDirectory);

            var physicalPath = Path.Combine(
                physicalDirectory,
                storedFileName);

            await using (var outputStream =
                new FileStream(
                    physicalPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
            {
                encodedData.SaveTo(outputStream);

                await outputStream.FlushAsync();
            }

            var fileInfo = new FileInfo(physicalPath);

            // Convert Windows '\' separators to URL '/' separators.
            var webPath =
                "/" +
                Path.Combine(
                        relativeDirectory,
                        storedFileName)
                    .Replace('\\', '/');

            return new ProcessedImage
            {
                FileName = storedFileName,
                FilePath = webPath,
                FileSize = fileInfo.Length,
                Width = targetWidth,
                Height = targetHeight
            };
        }
    }

    private static (int Width, int Height) CalculateDimensions(
        int width,
        int height,
        int maxDimension)
    {
        // Don't enlarge smaller images.
        if (width <= maxDimension &&
            height <= maxDimension)
        {
            return (width, height);
        }

        var scale = Math.Min(
            (double)maxDimension / width,
            (double)maxDimension / height);

        var newWidth =
            (int)Math.Round(width * scale);

        var newHeight =
            (int)Math.Round(height * scale);

        return (
            Math.Max(1, newWidth),
            Math.Max(1, newHeight));
    }
}