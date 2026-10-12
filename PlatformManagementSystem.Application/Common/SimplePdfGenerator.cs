using System;
using System.IO;
using System.Text;

namespace PlatformManagementSystem.Application.Common;

public static class SimplePdfGenerator
{
    public static byte[] GenerateCertificate(string studentName, string courseTitle, DateTime issueDate, string certificateId)
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.ASCII);

        string EscapePdf(string text) =>
            text.Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace("\r", "")
                .Replace("\n", " ");

        var safeStudent = EscapePdf(studentName);
        var safeCourse = EscapePdf(courseTitle);
        var safeCertId = EscapePdf(certificateId);
        var safeDate = EscapePdf(issueDate.ToString("MMMM dd, yyyy"));

        var content = new StringBuilder();

        // Outer and inner borders
        content.AppendLine("q");
        content.AppendLine("3 w");
        content.AppendLine("0.05 0.35 0.75 RG");
        content.AppendLine("30 30 782 535 re S");
        content.AppendLine("1.5 w");
        content.AppendLine("0.85 0.65 0.13 RG");
        content.AppendLine("38 38 766 519 re S");
        content.AppendLine("0.5 w");
        content.AppendLine("0.8 0.8 0.8 RG");
        content.AppendLine("120 480 602 0 re S");
        content.AppendLine("120 180 602 0 re S");
        content.AppendLine("Q");

        // Typography
        content.AppendLine("BT /F1 18 Tf 0.1 0.2 0.4 rg 1 0 0 1 260 500 Tm (PLATFORM MANAGEMENT SYSTEM) Tj ET");
        content.AppendLine("BT /F2 10 Tf 0.5 0.5 0.5 rg 1 0 0 1 350 486 Tm (E-LEARNING ACADEMY) Tj ET");

        content.AppendLine("BT /F1 28 Tf 0.05 0.35 0.75 rg 1 0 0 1 195 430 Tm (CERTIFICATE OF COMPLETION) Tj ET");

        content.AppendLine("BT /F3 13 Tf 0.4 0.4 0.4 rg 1 0 0 1 310 385 Tm (This is proudly presented to) Tj ET");

        content.AppendLine($"BT /F1 24 Tf 0.1 0.55 0.25 rg 1 0 0 1 240 335 Tm ({safeStudent}) Tj ET");

        content.AppendLine("BT /F2 12 Tf 0.35 0.35 0.35 rg 1 0 0 1 230 295 Tm (for successfully completing all curriculum requirements for) Tj ET");

        content.AppendLine($"BT /F1 20 Tf 0.15 0.15 0.2 rg 1 0 0 1 240 255 Tm ({safeCourse}) Tj ET");

        // Metadata Left
        content.AppendLine($"BT /F2 10 Tf 0.4 0.4 0.4 rg 1 0 0 1 100 145 Tm (Issue Date: {safeDate}) Tj ET");
        content.AppendLine($"BT /F2 10 Tf 0.4 0.4 0.4 rg 1 0 0 1 100 128 Tm (Certificate ID: {safeCertId}) Tj ET");
        content.AppendLine("BT /F2 10 Tf 0.4 0.4 0.4 rg 1 0 0 1 100 111 Tm (Status: Verified & Authenticated) Tj ET");

        // Signatures Right
        content.AppendLine("q 1 w 0.2 0.2 0.2 RG 560 145 180 0 re S Q");
        content.AppendLine("BT /F1 11 Tf 0.2 0.2 0.2 rg 1 0 0 1 600 130 Tm (Academic Director) Tj ET");
        content.AppendLine("BT /F2 9 Tf 0.5 0.5 0.5 rg 1 0 0 1 575 115 Tm (Platform Management System) Tj ET");

        var contentBytes = Encoding.ASCII.GetBytes(content.ToString());
        var offsets = new long[8];

        writer.Write("%PDF-1.4\n");
        writer.Flush();

        offsets[1] = ms.Position;
        writer.Write("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
        writer.Flush();

        offsets[2] = ms.Position;
        writer.Write("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");
        writer.Flush();

        offsets[3] = ms.Position;
        writer.Write("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 842 595] /Resources << /Font << /F1 4 0 R /F2 5 0 R /F3 6 0 R >> >> /Contents 7 0 R >>\nendobj\n");
        writer.Flush();

        offsets[4] = ms.Position;
        writer.Write("4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>\nendobj\n");
        writer.Flush();

        offsets[5] = ms.Position;
        writer.Write("5 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n");
        writer.Flush();

        offsets[6] = ms.Position;
        writer.Write("6 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Oblique >>\nendobj\n");
        writer.Flush();

        offsets[7] = ms.Position;
        writer.Write($"7 0 obj\n<< /Length {contentBytes.Length} >>\nstream\n");
        writer.Flush();
        ms.Write(contentBytes, 0, contentBytes.Length);
        writer.Write("\nendstream\nendobj\n");
        writer.Flush();

        var xrefPos = ms.Position;
        writer.Write("xref\n0 8\n");
        writer.Write("0000000000 65535 f \n");
        for (int i = 1; i <= 7; i++)
        {
            writer.Write($"{offsets[i]:D10} 00000 n \n");
        }
        writer.Write($"trailer\n<< /Size 8 /Root 1 0 R >>\nstartxref\n{xrefPos}\n%%EOF\n");
        writer.Flush();

        return ms.ToArray();
    }
}
