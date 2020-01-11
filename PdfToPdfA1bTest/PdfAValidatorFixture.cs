using System;
using Xunit;
using PdfToPdfA;
using System.IO;

namespace PdfToPdfA1bTest
{
    public class PdfAValidatorFixture : IDisposable
    {
        public PdfAValidatorFixture()
        {
            Validator = new PdfAValidator.PdfAValidator();
        }

        public void Dispose()
        {
            Validator.Dispose();
        }

        public static PdfAValidator.PdfAValidator Validator { get; private set; }
    }
}