using MyPathfinderCampaignTracker.Application.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MyPathfinderCampaignTracker.Infrastructure.Services;

public class ImageProcessingService : IImageProcessingService
{
    private const int AvatarSize = 512;

    public async Task<byte[]> CreateCircularAvatarAsync(Stream inputStream)
    {
        using var image = await Image.LoadAsync<Rgba32>(inputStream);

        var size = Math.Min(image.Width, image.Height);
        image.Mutate(ctx => ctx
            .Crop(new Rectangle(
                (image.Width - size) / 2,
                (image.Height - size) / 2,
                size, size))
            .Resize(AvatarSize, AvatarSize));

        // Clip to circle using DestIn composition: keeps image pixels only where the white ellipse exists
        image.Mutate(ctx =>
        {
            ctx.SetGraphicsOptions(new GraphicsOptions
            {
                Antialias = true,
                AlphaCompositionMode = PixelAlphaCompositionMode.DestIn
            });
            ctx.Fill(Color.White, new EllipsePolygon(AvatarSize / 2f, AvatarSize / 2f, AvatarSize / 2f));
        });

        using var output = new MemoryStream();
        await image.SaveAsync(output, new PngEncoder());
        return output.ToArray();
    }
}
