using System;
using Xunit;
using PdfToPdfA;
using System.IO;

namespace PdfToPdfA1bTest
{
    public class UnitTestIntegration
    {
        private const string PathSourcePdf = "./TestFiles/FromLibreOfficeNonPdfA.pdf";

        [Fact]
        public void ShouldConvertToCompliantPdfA1bFromStream()
        {
            var sourceFile = File.ReadAllBytes(PathSourcePdf);
            using (var sourcePdfStream = new FileStream(PathSourcePdf, FileMode.Open, FileAccess.Read))
            {
                using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
                {
                    var validPdfA1b = pdfToPdfA1bStream.Convert(sourcePdfStream);
                    File.WriteAllBytes("ConvertedFromLibreOfficeNonPdfA.pdf", validPdfA1b.ToArray());
                }

                using (var validator = new PdfAValidator.PdfAValidator())
                    Assert.True(validator.Validate("ConvertedFromLibreOfficeNonPdfA.pdf"));
            }
        }

        [Fact]
        public void ShouldConvertToCompliantPdfA1bFromFilePath()
        {
            var validPdfA1b = PdfToPdfA1b.Convert(PathSourcePdf);

            File.WriteAllBytes("ConvertedFromLibreOfficeNonPdfA.pdf", validPdfA1b);

            using (var validator = new PdfAValidator.PdfAValidator())
                Assert.True(validator.Validate("ConvertedFromLibreOfficeNonPdfA.pdf"));
        }

        [Fact]
        public void ShouldConvertToCompliantPdfA1bFromByteArray()
        {
            var sourceFile = File.ReadAllBytes(PathSourcePdf);
            var validPdfA1b = PdfToPdfA1b.Convert(sourceFile);
            File.WriteAllBytes("ConvertedFromLibreOfficeNonPdfA.pdf", validPdfA1b);

            using (var validator = new PdfAValidator.PdfAValidator())
                Assert.True(validator.Validate("ConvertedFromLibreOfficeNonPdfA.pdf"));
        }
    }
}