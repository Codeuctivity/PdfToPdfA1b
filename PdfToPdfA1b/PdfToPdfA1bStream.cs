using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Reflection;

// archived iText forum http://itext-general.2136553.n4.nabble.com/ for iTexSharp Lgpl from the old days
// set page size before calling NewPage() to get support for multiple page size in one document http://kuujinbo.info/iTextSharp/pageResize.aspx

namespace Codeuctivity
{
    /// <summary>
    /// Converts plain Pdfs to PdfA-1b
    /// </summary>
    public class PdfToPdfA1bStreamable : IDisposable
    {
        private const string IEC61966 = "sRGB IEC61966-2.1";
        private bool disposedValue;
        private readonly MemoryStream convertedPdfA1b;
        private Stream IccFileInputStream { get; }

        /// <summary>
        /// default ctor
        /// </summary>
        public PdfToPdfA1bStreamable()
        {
            convertedPdfA1b = new MemoryStream();
            // Got ICC profile from http://www.color.org/srgbprofiles.xalter
            var assembly = Assembly.GetExecutingAssembly();
            IccFileInputStream = assembly.GetManifestResourceStream("Codeuctivity.sRGB2014.icc");
            IccFileInputStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Converts a plain Pdf to a PdfA-1b
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <returns></returns>
        public MemoryStream Convert(Stream sourcePdf)
        {
            return Convert(sourcePdf, false);
        }

        /// <summary>
        /// Converts a plain Pdf to a PdfA-1b
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <param name="embedFonts">default is false</param>
        /// <returns></returns>
        public MemoryStream Convert(Stream sourcePdf, bool embedFonts)
        {
            if (sourcePdf is null)
            {
                throw new ArgumentNullException(nameof(sourcePdf));
            }

            if (!embedFonts)
            {
                return EmbedMetadata(sourcePdf);
            }

            return EmbedFonts(sourcePdf);
        }

        private MemoryStream EmbedFonts(Stream sourcePdf)
        {
            var tempPdfFileNameEmbeddedFonts = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pdf");
            var tempPdfFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pdf");
            try
            {
                using (var fileStream = new FileStream(tempPdfFileName, FileMode.Create, FileAccess.Write))
                {
                    if (sourcePdf.CanSeek)
                    {
                        sourcePdf.Seek(0, SeekOrigin.Begin);
                    }

                    sourcePdf.CopyTo(fileStream);
                }

                using (var postscriptValidator = new PostScriptValidator.PostScriptValidator())
                {
                    postscriptValidator.EmbedFonts(tempPdfFileName, tempPdfFileNameEmbeddedFonts);
                }

                using (var memoryStreamEmbeddedFonts = new MemoryStream())
                using (var file = new FileStream(tempPdfFileNameEmbeddedFonts, FileMode.Open, FileAccess.Read))
                {
                    file.CopyTo(memoryStreamEmbeddedFonts);
                    memoryStreamEmbeddedFonts.Seek(0, SeekOrigin.Begin);
                    return EmbedMetadata(memoryStreamEmbeddedFonts);
                }
            }
            finally
            {
                DeleteFileIfExists(tempPdfFileNameEmbeddedFonts);
                DeleteFileIfExists(tempPdfFileName);
            }
        }

        private MemoryStream EmbedMetadata(Stream sourcePdf)
        {
            var pdfReader = new PdfReader(sourcePdf);
            var sourcePageCount = pdfReader.NumberOfPages;
            var pageSizeFirstPage = pdfReader.GetPageSize(1);

            // step 1: creation of a document-object
            var document = new Document(pageSizeFirstPage, 0, 0, 0, 0);

            // step 2: we create a writer that listens to the document
            var pdfWriter = PdfWriter.GetInstance(document, convertedPdfA1b);
            pdfWriter.PdfxConformance = PdfWriter.PDFA1B;

            // step 3: we open the document
            document.Open();

            // step 3a: add pdfA flavor
            var pdfDictionary = new PdfDictionary(PdfName.Outputintent);
            pdfDictionary.Put(PdfName.Outputconditionidentifier, new PdfString(IEC61966));
            pdfDictionary.Put(PdfName.Info, new PdfString(IEC61966));
            pdfDictionary.Put(PdfName.S, PdfName.GtsPdfa1);
            var pdfAFriendlyIccProfile = new PdfIccBased(IccProfile.GetInstance(IccFileInputStream));
            pdfAFriendlyIccProfile.Remove(PdfName.Alternate);
            pdfDictionary.Put(PdfName.Destoutputprofile, pdfWriter.AddToBody(pdfAFriendlyIccProfile).IndirectReference);
            pdfWriter.ExtraCatalog.Put(PdfName.Outputintents, new PdfArray(pdfDictionary));

            // step 4: we add content
            var directContent = pdfWriter.DirectContent;

            for (var i = 1; i <= sourcePageCount; i++)
            {
                document.SetPageSize(pdfReader.GetPageSize(i));
                document.NewPage();
                var currentPage = pdfWriter.GetImportedPage(pdfReader, i);
                directContent.AddTemplate(currentPage, 0, 0);
            }

            // step 4a: more pdfA flavor
            pdfWriter.CreateXmpMetadata();

            // step 5: we close the document
            document.Close();
            return convertedPdfA1b;
        }

        private static void DeleteFileIfExists(string tempPdfFileName)
        {
            if (File.Exists(tempPdfFileName))
            {
                File.Delete(tempPdfFileName);
            }
        }

        /// <summary>
        /// disposes resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    IccFileInputStream?.Dispose();
                    convertedPdfA1b?.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}