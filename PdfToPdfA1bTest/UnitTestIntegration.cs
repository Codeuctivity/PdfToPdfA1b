using PdfAValidator;
using PdfToPdfA;
using System.IO;
using Xunit;

namespace PdfToPdfA1bTest
{
    public class UnitTestIntegration : IClassFixture<PdfAValidatorFixture>
    {
        private const string PathSourcePdf = "./TestFiles/FromLibreOfficeNonPdfA.pdf";
        private const string PathSourceMozillaPdf = "./TestFiles/Mozilla.pdf";

        [Fact]
        public void ShouldConvertToCompliantPdfA1bFromStream()
        {
            using var sourcePdfStream = new FileStream(PathSourcePdf, FileMode.Open, FileAccess.Read);
            using var pdfToPdfA1bStream = new PdfToPdfA1bStreamable();
            var validPdfA1b = pdfToPdfA1bStream.Convert(sourcePdfStream);
            AssertPdfA(validPdfA1b);
        }

        [Fact]
        public void ShouldConvertToCompliantPdfA1bFromFilePath()
        {
            var validPdfA1b = PdfToPdfA1b.Convert(PathSourcePdf);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            AssertPdfA(validPdfA1bStream);
        }

        [Theory]
        // TODO find out, why embedding second time makes this test green
        //[InlineData(true)]
        [InlineData(false)]
        public void ShouldConvertToCompliantPdfA1bFromFileOverloadEmbeddFontsPath(bool embedFonts)
        {
            var validPdfA1b = PdfToPdfA1b.Convert(PathSourcePdf, embedFonts);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            AssertPdfA(validPdfA1bStream);
        }

        [Theory]
        [InlineData(true, PathSourceMozillaPdf)]
        [InlineData(true, PathSourcePdf)]
        [InlineData(false, PathSourcePdf)]
        public void ShouldConvertToCompliantPdfA1bCascading(bool embedFonts, string path)
        {
            var validPdfA1b = PdfToPdfA1b.Convert(path, embedFonts);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            using var pdfToPdfA1bStreamable = new PdfToPdfA1bStreamable();
            AssertPdfA(pdfToPdfA1bStreamable.Convert(validPdfA1bStream, embedFonts));
        }

        [Fact]
        public void ShouldConvertToCompliantPdfA1bFromByteArray()
        {
            var sourceFile = File.ReadAllBytes(PathSourcePdf);
            var validPdfA1b = PdfToPdfA1b.Convert(sourceFile);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            AssertPdfA(validPdfA1bStream);
        }

        [Theory]
        // TODO find out, why embedding second time makes this test green
        //[InlineData(true)]
        [InlineData(false)]
        public void ShouldConvertToCompliantPdfA1bFromByteArrayOverloadEmbeddFontsPath(bool embedFonts)
        {
            var sourceFile = File.ReadAllBytes(PathSourcePdf);
            var validPdfA1b = PdfToPdfA1b.Convert(sourceFile, embedFonts);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            AssertPdfA(validPdfA1bStream);
        }

        [Fact]
        public void ShouldConvertPdfWithReferencedFonts()
        {
            using var sourcePdfStream = new FileStream(PathSourceMozillaPdf, FileMode.Open, FileAccess.Read);
            using var pdfToPdfA1bStream = new PdfToPdfA1bStreamable();
            var validPdfA1b = pdfToPdfA1bStream.Convert(sourcePdfStream, true);
            AssertPdfA(validPdfA1b);
        }

        private static void AssertPdfA(MemoryStream validPdfA1b)
        {
            var tempPdfFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pdf");

            File.WriteAllBytes(tempPdfFileName, validPdfA1b.ToArray());

            var report = PdfAValidatorFixture.Validator.ValidateWithDetailedReport(tempPdfFileName);

            Assert.True(report.Jobs.Job.ValidationReport.IsCompliant, GetValidationProblemDescriptions(report, tempPdfFileName));

            File.Delete(tempPdfFileName);
        }

        private static string GetValidationProblemDescriptions(Report report, string tempPdfFileName)
        {
            var summary = "Failed rules failed for " + tempPdfFileName + ": ";

            foreach (var rule in report.Jobs.Job.ValidationReport.Details.Rule)
            {
                summary += rule.Clause + " " + rule.Description;
            }
            return summary;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldDisposeWithoutException(bool embedFonts)
        {
            using var sourcePdfStream = new FileStream(PathSourceMozillaPdf, FileMode.Open, FileAccess.Read);
            using var pdfToPdfA1bStreamable = new PdfToPdfA1bStreamable();
            _ = pdfToPdfA1bStreamable.Convert(sourcePdfStream, embedFonts);
            pdfToPdfA1bStreamable.Dispose();
            pdfToPdfA1bStreamable.Dispose();
        }

        [Fact]
        public void ShouldDisposeWithoutExceptionWithoutUsingConvert()
        {
            using (var pdfToPdfA1bStreamable = new PdfToPdfA1bStreamable())
            {
                pdfToPdfA1bStreamable.Dispose();
                pdfToPdfA1bStreamable.Dispose();
            }
        }
    }
}