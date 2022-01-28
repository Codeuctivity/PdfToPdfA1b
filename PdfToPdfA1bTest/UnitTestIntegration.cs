using Codeuctivity;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Codeuctivity1bTest
{
    public class UnitTestIntegration : IClassFixture<PdfAValidatorFixture>
    {
        private const string PathSourcePdf = "./TestFiles/FromLibreOfficeNonPdfA.pdf";
        private const string PathSourceMozillaPdf = "./TestFiles/Mozilla.pdf";

        [Fact]
        public async Task ShouldConvertToCompliantPdfA1bFromStreamAsync()
        {
            using var sourcePdfStream = new FileStream(PathSourcePdf, FileMode.Open, FileAccess.Read);
            using var pdfToPdfA1bStream = new PdfToPdfA1BStreamable();
            var validPdfA1b = pdfToPdfA1bStream.Convert(sourcePdfStream);
            await AssertPdfA(validPdfA1b);
        }

        [Fact]
        public async Task ShouldConvertToCompliantPdfA1bFromFilePathAsync()
        {
            var validPdfA1b = PdfToPdfA1B.Convert(PathSourcePdf);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            await AssertPdfA(validPdfA1bStream);
        }

        [Theory]
        // TODO find out, why embedding second time makes this test green
        //[InlineData(true)]
        [InlineData(false)]
        public async Task ShouldConvertToCompliantPdfA1bFromFileOverloadEmbeddFontsPathAsync(bool embedFonts)
        {
            var validPdfA1b = PdfToPdfA1B.Convert(PathSourcePdf, embedFonts);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            await AssertPdfA(validPdfA1bStream);
        }

        [Theory]
        [InlineData(true, PathSourceMozillaPdf)]
        [InlineData(true, PathSourcePdf)]
        [InlineData(false, PathSourcePdf)]
        public async Task ShouldConvertToCompliantPdfA1bCascadingAsync(bool embedFonts, string path)
        {
            var validPdfA1b = PdfToPdfA1B.Convert(path, embedFonts);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            using var pdfToPdfA1bStreamable = new PdfToPdfA1BStreamable();
            await AssertPdfA(pdfToPdfA1bStreamable.Convert(validPdfA1bStream, embedFonts));
        }

        [Fact]
        public async Task ShouldConvertToCompliantPdfA1bFromByteArrayAsync()
        {
            var sourceFile = File.ReadAllBytes(PathSourcePdf);
            var validPdfA1b = PdfToPdfA1B.Convert(sourceFile);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            await AssertPdfA(validPdfA1bStream);
        }

        [Theory]
        // TODO find out, why embedding second time makes this test green
        //[InlineData(true)]
        [InlineData(false)]
        public async Task ShouldConvertToCompliantPdfA1bFromByteArrayOverloadEmbeddFontsPathAsync(bool embedFonts)
        {
            var sourceFile = File.ReadAllBytes(PathSourcePdf);
            var validPdfA1b = PdfToPdfA1B.Convert(sourceFile, embedFonts);

            using var validPdfA1bStream = new MemoryStream(validPdfA1b);
            await AssertPdfA(validPdfA1bStream);
        }

        [Fact]
        public async Task ShouldConvertPdfWithReferencedFontsAsync()
        {
            using var sourcePdfStream = new FileStream(PathSourceMozillaPdf, FileMode.Open, FileAccess.Read);
            using var pdfToPdfA1bStream = new PdfToPdfA1BStreamable();
            var validPdfA1b = pdfToPdfA1bStream.Convert(sourcePdfStream, true);
            await AssertPdfA(validPdfA1b);
        }

        private static async Task AssertPdfA(MemoryStream validPdfA1b)
        {
            var tempPdfFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".pdf");

            File.WriteAllBytes(tempPdfFileName, validPdfA1b.ToArray());

            var report = await PdfAValidatorFixture.Validator.ValidateWithDetailedReportAsync(tempPdfFileName);

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
            using var pdfToPdfA1bStreamable = new PdfToPdfA1BStreamable();
            _ = pdfToPdfA1bStreamable.Convert(sourcePdfStream, embedFonts);
#pragma warning disable S3966 // Objects should not be disposed more than once, but it should be supported
            pdfToPdfA1bStreamable.Dispose();
            pdfToPdfA1bStreamable.Dispose();
#pragma warning restore S3966 // Objects should not be disposed more than once, but it should be supported
        }

        [Fact]
        public void ShouldDisposeWithoutExceptionWithoutUsingConvert()
        {
#pragma warning disable S3966 // Objects should not be disposed more than once, but it should be supported
            using (var pdfToPdfA1bStreamable = new PdfToPdfA1BStreamable())
            {
                pdfToPdfA1bStreamable.Dispose();
                pdfToPdfA1bStreamable.Dispose();
#pragma warning restore S3966 // Objects should not be disposed more than once, but it should be supported
            }
        }
    }
}