using System;
using Xunit;
using PdfToPdfA;
using System.IO;

namespace PdfToPdfA1bTest
{
    public class UnitTestIntegration : IClassFixture<PdfAValidatorFixture>
    {
        private const string PathSourcePdf = "./TestFiles/FromLibreOfficeNonPdfA.pdf";
        private const string PathSourceMozillaPdf = "./TestFiles/Mozilla.pdf";

        [Fact]
        public void ShouldConvertToCompliantPdfA1bFromStream()
        {
            using (var sourcePdfStream = new FileStream(PathSourcePdf, FileMode.Open, FileAccess.Read))
            {
                using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
                {
                    var validPdfA1b = pdfToPdfA1bStream.Convert(sourcePdfStream);
                    AssertPdfA(validPdfA1b);
                }
            }
        }

        [Fact]
        public void ShouldConvertToCompliantPdfA1bFromFilePath()
        {
            var validPdfA1b = PdfToPdfA1b.Convert(PathSourcePdf);

            using (var validPdfA1bStream = new MemoryStream(validPdfA1b))
                AssertPdfA(validPdfA1bStream);
        }

        [Fact]
        public void ShouldConvertToCompliantPdfA1bFromByteArray()
        {
            var sourceFile = File.ReadAllBytes(PathSourcePdf);
            var validPdfA1b = PdfToPdfA1b.Convert(sourceFile);

            using (var validPdfA1bStream = new MemoryStream(validPdfA1b))
                AssertPdfA(validPdfA1bStream);
        }

        [Fact]
        public void ShouldConvertMozilla()
        {
            using (var sourcePdfStream = new FileStream(PathSourceMozillaPdf, FileMode.Open, FileAccess.Read))
            {
                using (var pdfToPdfA1bStream = new PdfToPdfA1bStreamable())
                {
                    var validPdfA1b = pdfToPdfA1bStream.Convert(sourcePdfStream, true);
                    AssertPdfA(validPdfA1b);
                }
            }
        }

        private static void AssertPdfA(MemoryStream validPdfA1b)
        {
            var tempPdfFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pdf");

            try
            {
                File.WriteAllBytes(tempPdfFileName, validPdfA1b.ToArray());

                Assert.True(PdfAValidatorFixture.Validator.Validate(tempPdfFileName));
            }
            finally
            {
                File.Delete(tempPdfFileName);
            }
        }
    }


}