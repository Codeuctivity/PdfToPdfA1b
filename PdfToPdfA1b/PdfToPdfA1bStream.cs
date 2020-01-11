using System;
using System.IO;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;

// archived itext forum http://itext-general.2136553.n4.nabble.com/ for itexSharp Lgpl from the old days
// set page size before calling NewPage() to get support for multiple pagesize in one document http://kuujinbo.info/iTextSharp/pageResize.aspx

namespace PdfToPdfA
{
    public class PdfToPdfA1bStreamable : IDisposable
    {
        private bool disposedValue;
        private readonly MemoryStream convertedPdfA1b;
        private Stream iccFileInputStream { get; }
        public PdfToPdfA1bStreamable()
        {
            convertedPdfA1b = new MemoryStream();
            // Got Icc profile from http://www.color.org/srgbprofiles.xalter
            iccFileInputStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PdfToPdfA1b.sRGB2014.icc");
        }

        public MemoryStream Convert(Stream sourcePdf)
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

        public void Dispose()
        {
            Dispose(true);
        }
    }
}