# PdfToPdfA1b

PdfToPdfA1b is a .net standard assembly capable to add PdfA-1b meta data to an existing Pdf.

[![Build](https://github.com/Codeuctivity/PdfToPdfA1b/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Codeuctivity/PdfToPdfA1b/actions/workflows/dotnet.yml) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/0ebe9e94ea8e4533a5283085f86277e4)](https://www.codacy.com/gh/Codeuctivity/PdfToPdfA1b?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Codeuctivity/PdfToPdfA1b&amp;utm_campaign=Badge_Grade) [![Nuget](https://img.shields.io/nuget/v/PdfToPdfA1b.svg)](https://www.nuget.org/packages/PdfToPdfA1b/)

```csharp
public void AddsPdfAMetadata()
{
    var validPdfA1b = PdfToPdfA1b.Convert("./normalPdf.pdf");
    File.WriteAllBytes("./superValidPdfA-1b.pdf", validPdfA1b);
}
```

## Dependencies

Embedding fonts currently bases on ghostscript. If you run on linux you need to install ghostscript using

```bash
sudo apt install ghostscript
```
