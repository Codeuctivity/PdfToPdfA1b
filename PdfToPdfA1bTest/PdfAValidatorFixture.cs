using System;

namespace PdfToPdfA1bTest
{
    public class PdfAValidatorFixture : IDisposable
    {
        public PdfAValidatorFixture()
        {
            Validator = new PdfAValidator.PdfAValidator();
        }

        public static PdfAValidator.PdfAValidator Validator { get; private set; }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Validator.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}