using System;
using System.IO;

namespace PdfToPdfA
{
    static public class PdfToPdfA1b
    {
        static public byte[] Convert(byte[] sourcePdf)
        {
            using (var sourcePdfStream = new MemoryStream(sourcePdf))
            using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
            {
                return pdfToPdfA1bStream.Convert(sourcePdfStream).ToArray();
            }
        }
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