using System.IO;

namespace PdfToPdfA
{
    /// <summary>
    /// Converts plain Pdfs to PdfA-1b
    /// </summary>
    public static class PdfToPdfA1b
    {
        /// <summary>
        /// Converts a plain Pdf to a PdfA-1b, not embedding fonts
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <returns>Pdf</returns>
        public static byte[] Convert(byte[] sourcePdf)
        {
            using (var sourcePdfStream = new MemoryStream(sourcePdf))
            {
                using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
                {
                    return pdfToPdfA1bStream.Convert(sourcePdfStream, false).ToArray();
                }
            }
        }

        /// <summary>
        /// Converts a plain Pdf to a PdfA-1b
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <param name="embedFonts">default is false</param>
        /// <returns>Pdf</returns>
        public static byte[] Convert(byte[] sourcePdf, bool embedFonts)
        {
            using (var sourcePdfStream = new MemoryStream(sourcePdf))
            using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
            {
                return pdfToPdfA1bStream.Convert(sourcePdfStream, embedFonts).ToArray();
            }
        }

        /// <summary>
        /// Converts a plain Pdf to a PdfA-1b, not embedding fonts
        /// </summary>
        /// <param name="pathSourcePdf"></param>
        /// <returns>Pdf</returns>
        public static byte[] Convert(string pathSourcePdf)
        {
            using (var sourcePdfStream = new FileStream(pathSourcePdf, FileMode.Open, FileAccess.Read))
            using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
            {
                return pdfToPdfA1bStream.Convert(sourcePdfStream, false).ToArray();
            }
        }

        /// <summary>
        /// Converts a plain Pdf to a PdfA-1b
        /// </summary>
        /// <param name="pathSourcePdf"></param>
        /// <param name="embedFonts">default is false</param>
        /// <returns>Pdf</returns>
        public static byte[] Convert(string pathSourcePdf, bool embedFonts)
        {
            using (var sourcePdfStream = new FileStream(pathSourcePdf, FileMode.Open, FileAccess.Read))
            using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
            {
                return pdfToPdfA1bStream.Convert(sourcePdfStream, embedFonts).ToArray();
            }
        }
    }
}