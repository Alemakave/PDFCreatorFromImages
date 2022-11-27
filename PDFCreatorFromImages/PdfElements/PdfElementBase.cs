using PdfSharp;
using PdfSharp.Pdf;

namespace PDFCreatorFromImages.PdfElements
{
    public abstract class PdfElement
    {
        public virtual void DrawPage(PdfPage page)
        {
            page.Size = PageSize.A4;
        }

        public abstract void Dispose();
    }
}