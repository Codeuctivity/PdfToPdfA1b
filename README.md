# PdfToPdfA1b
PdfToPdfA1b is a .net standard assembly cappable to add PdfA-1b metadata to an existing Pdf.

```csharp
public void AddsPdfAMetadata()
{
    var validPdfA1b = PdfToPdfA1b.Convert("./normalPdf.pdf");
    File.WriteAllBytes("./superValidPdfA-1b.pdf", validPdfA1b);
}
```