using System;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PDFCreatorFromImages.PdfElements
{
    public class PdfImagePage : PdfElement
    {
        private String _imagePath;
        private XImage _image;

        public PdfImagePage(String imagePath)
        {
            _imagePath = imagePath;
            _image = XImage.FromFile(_imagePath);
        }

        public override void DrawPage(PdfPage page)
        {
            base.DrawPage(page);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawImage(gfx);
        }

        private void DrawImage(XGraphics gfx)
        {
            double x = gfx.PageSize.Width / 2 - _image.PointWidth / 2;
            double y = gfx.PageSize.Height / 2 - _image.PointHeight / 2;
            gfx.DrawImage(_image, x, y);
        }

        public override void Dispose()
        {
            _image.Dispose();
        }
    }
}