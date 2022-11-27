using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PDFCreatorFromImages.Utils
{
    public class ResourcesManager
    {
        public static ImageSource GetImage(string key)
        {
            BitmapFrame image;

            switch (key)
            {
                case "add":
                    using (Stream stream = GetResource("PDFCreatorFromImages.Assets.Add"))
                    {
                        image = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                    break;
                case "clear":
                    using (Stream stream = GetResource("PDFCreatorFromImages.Assets.Clear"))
                    {
                        image = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                    break;
                case "save":
                    using (Stream stream = GetResource("PDFCreatorFromImages.Assets.Save"))
                    {
                        image = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                    break;
                case "git":
                    using (Stream stream = GetResource("PDFCreatorFromImages.Assets.Git"))
                    {
                        image = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                    break;
                default:
                    using (Stream stream = GetResource("PDFCreatorFromImages.Assets.Error"))
                    {
                        image = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                    break;
            }

            return CreateResizedImage(image, 32, 32, 2);
        }

        public static Stream GetResource(string name)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        }

        private static BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin)
        {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            var resizedImage = new RenderTargetBitmap(
                width, height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return BitmapFrame.Create(resizedImage);
        }
    }
}