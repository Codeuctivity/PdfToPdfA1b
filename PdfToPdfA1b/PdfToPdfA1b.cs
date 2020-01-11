using System;
using System.IO;

namespace PdfToPdfA
{
    /// <summary>
    /// Converts plain Pdfs to PdfA-1b
    /// </summary>
    static public class PdfToPdfA1b
    {
        /// <summary>
        /// Converts a plain Pdf to a PdfA-1b
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <returns></returns>
        static public byte[] Convert(byte[] sourcePdf)
        {
            using (var sourcePdfStream = new MemoryStream(sourcePdf))
            using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
            {
                return pdfToPdfA1bStream.Convert(sourcePdfStream).ToArray();
            }
        }

        /// <summary>
        /// Converts a plain Pdf to a PdfA-1b
        /// </summary>
        /// <param name="pathSourcePdf"></param>
        /// <returns></returns>
        static public byte[] Convert(string pathSourcePdf)
        {
            using (var sourcePdfStream = new FileStream(pathSourcePdf, FileMode.Open, FileAccess.Read))
            using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
            {
                return pdfToPdfA1bStream.Convert(sourcePdfStream).ToArray();
            }
        }
    }
}