using Codeuctivity;
using System;

namespace PdfToPdfA1bTest
{
    public class PdfAValidatorFixture : IDisposable
    {
        public PdfAValidatorFixture()
        {
            Validator = new PdfAValidator();
        }

        public static PdfAValidator Validator { get; private set; }

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