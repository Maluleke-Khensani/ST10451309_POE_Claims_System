using Claims_System.Models;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;


namespace Claims_System.Services
{
    // Assisted by ChatGPT to implement PDF generation functionality
    // as I found it challenging to create well-formatted PDF reports, decide to create a basic report structure
    public static class PdfGeneratorHelper
    {

        public static byte[] CreateLecturerReport(LecturerProfile lecturer, IEnumerable<LecturerClaim> claims)
        {
            PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);

            document.Add(new Paragraph("Lecturer Claims Report")
          .SetTextAlignment(TextAlignment.CENTER)
          .SetFontSize(20)
          .SetFont(boldFont)
          .SetMarginBottom(20));



            // Lecturer Info
            var lecturerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2 }))
                .UseAllAvailableWidth()
                .SetMarginBottom(20);

            lecturerTable.AddCell("Full Name:");
            lecturerTable.AddCell(lecturer.FullName);

            lecturerTable.AddCell("Email:");
            lecturerTable.AddCell(lecturer.Email);

            lecturerTable.AddCell("Hourly Rate:");
            lecturerTable.AddCell($"R {lecturer.HourlyRate:N2}");

            lecturerTable.AddCell("Generated On:");
            lecturerTable.AddCell(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            document.Add(lecturerTable);

            // Claims Table
            var claimsTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1 }))
                .UseAllAvailableWidth();

            claimsTable.AddHeaderCell("Month/Year");
            claimsTable.AddHeaderCell("Hours Worked");
            claimsTable.AddHeaderCell("Rate (R)");
            claimsTable.AddHeaderCell("Total (R)");
            claimsTable.AddHeaderCell("Status");

            decimal grandTotal = 0;
            foreach (var claim in claims)
            {
                claimsTable.AddCell($"{claim.Month}/{claim.Year}");
                claimsTable.AddCell(claim.HoursWorked.ToString("N2"));
                claimsTable.AddCell(claim.Rate.ToString("N2"));
                claimsTable.AddCell(claim.Total.ToString("N2"));
                string status = claim.ManagerStatus == "Approved" && claim.CoordinatorStatus == "Approved" ? "Approved" :
                                claim.ManagerStatus == "Rejected" || claim.CoordinatorStatus == "Rejected" ? "Rejected" : "Pending";
                claimsTable.AddCell(status);

                if (status == "Approved")
                    grandTotal += claim.Total;
            }

            document.Add(claimsTable);

            // Grand Total
            document.Add(new Paragraph("Grand Total: R " + grandTotal.ToString("N2"))
      .SetTextAlignment(TextAlignment.RIGHT)
      .SetFont(boldFont)
      .SetMarginTop(10));
    

            document.Close();
            return ms.ToArray();
        }
    }
}
