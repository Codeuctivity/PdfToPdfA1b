using System;
using System.IO;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;

// archived itext forum http://itext-general.2136553.n4.nabble.com/ for itexSharp Lgpl from the old days
// set page size before calling NewPage() to get support for multiple pagesize in one document http://kuujinbo.info/iTextSharp/pageResize.aspx

namespace PdfToPdfA
{
    /// <summary>
    /// Converts plain Pdfs to PdfA-1b
    /// </summary>
    public class PdfToPdfA1bStreamable : IDisposable
    {
        private bool disposedValue;
        private readonly MemoryStream convertedPdfA1b;
        private Stream iccFileInputStream { get; }
        /// <summary>
        /// default ctor
        /// </summary>
        public PdfToPdfA1bStreamable()
        {
            convertedPdfA1b = new MemoryStream();
            // Got Icc profile from http://www.color.org/srgbprofiles.xalter
            iccFileInputStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PdfToPdfA1b.sRGB2014.icc");
        }

        /// <summary>
        /// Converts a plain Pdf to a PdfA-1b
        /// </summary>
        /// <param name="sourcePdf"></param>
        /// <param name="embeddFonts">default is false</param>
        /// <returns></returns>
        public MemoryStream Convert(Stream sourcePdf, bool embeddFonts = false)
        {
            var tempPdfFileNameEmbeddedFonts = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pdf");
            try
            {
                if (embeddFonts)
                {
                    var tempPdfFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pdf");
                    using (var fileStream = new FileStream(tempPdfFileName, FileMode.Create, System.IO.FileAccess.Write))
                    {
                        var bytes = new byte[sourcePdf.Length];
                        sourcePdf.Read(bytes, 0, (int)sourcePdf.Length);
                        fileStream.Write(bytes, 0, bytes.Length);
                        sourcePdf.Close();
                    }
                    try
                    {
                        using (var postscriptValidator = new PostScriptValidator.PostScriptValidator())
                            postscriptValidator.EmbedFonts(tempPdfFileName, tempPdfFileNameEmbeddedFonts);
                        using (var memoryStreamEmbeddedFonts = new MemoryStream())
                        using (FileStream file = new FileStream(tempPdfFileNameEmbeddedFonts, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            memoryStreamEmbeddedFonts.Write(bytes, 0, (int)file.Length);
                            memoryStreamEmbeddedFonts.Seek(0, SeekOrigin.Begin);
                            return embeddMetadata(memoryStreamEmbeddedFonts);
                        }
                    }
                    finally
                    {
                        DeleteFileIfExists(tempPdfFileName);
                    }
                }

                return embeddMetadata(sourcePdf);
            }
            finally
            {
                if (embeddFonts)
                {
                    DeleteFileIfExists(tempPdfFileNameEmbeddedFonts);
                }
            }
        }

        private MemoryStream embeddMetadata(Stream sourcePdf)
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

            // step 3a: add pdfA flavour
            var pdfDictonary = new PdfDictionary(PdfName.Outputintent);
            pdfDictonary.Put(PdfName.Outputconditionidentifier, new PdfString("sRGB IEC61966-2.1"));
            pdfDictonary.Put(PdfName.Info, new PdfString("sRGB IEC61966-2.1"));
            pdfDictonary.Put(PdfName.S, PdfName.GtsPdfa1);
            var pdfAFriendlyIccProfile = new PdfIccBased(IccProfile.GetInstance(iccFileInputStream));
            pdfAFriendlyIccProfile.Remove(PdfName.Alternate);
            pdfDictonary.Put(PdfName.Destoutputprofile, pdfWriter.AddToBody(pdfAFriendlyIccProfile).IndirectReference);
            pdfWriter.ExtraCatalog.Put(PdfName.Outputintents, new PdfArray(pdfDictonary));

            // step 4: we add content
            var directContent = pdfWriter.DirectContent;

            for (int i = 1; i <= sourcePageCount; i++)
            {
                document.SetPageSize(pdfReader.GetPageSize(i));
                document.NewPage();
                var currentPage = pdfWriter.GetImportedPage(pdfReader, i);
                directContent.AddTemplate(currentPage, 0, 0);
            }

            // step 4a: more pdfA flavour
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
                    iccFileInputStream.Dispose();
                    convertedPdfA1b.Dispose();
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
        }
    }
}