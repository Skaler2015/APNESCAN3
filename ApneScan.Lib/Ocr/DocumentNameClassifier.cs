using System.Text;
using System.Text.RegularExpressions;

namespace ApneScan.Ocr;

/// <summary>
/// Turns raw OCR text from a scanned page into a suggested, human-friendly document name.
/// This is a lightweight, offline, keyword-based classifier - it never talks to the network and
/// gracefully returns null when it can't make a confident guess.
/// </summary>
public static class DocumentNameClassifier
{
    // Each rule maps a document type (used as the file name) to keywords that identify it.
    // The first rule whose keywords are found in the text wins, so more specific/uncommon
    // documents are listed before generic ones.
    private static readonly (string Name, string[] Keywords)[] Rules =
    [
        ("Aadhaar Card", ["aadhaar", "aadhar", "uidai", "unique identification", "आधार"]),
        ("PAN Card", ["permanent account number", "income tax department", "pan card"]),
        ("Passport", ["passport", "republic of india", "type p<"]),
        ("Driving Licence", ["driving licence", "driving license", "transport department", "dl no"]),
        ("Voter ID", ["election commission", "elector", "voter", "epic no"]),
        ("Ration Card", ["ration card", "food and civil supplies", "public distribution"]),
        ("Bank Statement", ["statement of account", "account statement", "closing balance", "ifsc", "transaction details"]),
        ("Cheque", ["pay to", "account payee", "or bearer", "ifsc code"]),
        ("Invoice", ["invoice", "tax invoice", "bill to", "gstin", "hsn"]),
        ("Receipt", ["receipt", "amount paid", "payment received"]),
        ("Marksheet", ["marksheet", "mark sheet", "grade", "examination", "roll no", "board of"]),
        ("Certificate", ["certificate", "hereby certify", "this is to certify"]),
        ("Salary Slip", ["salary slip", "pay slip", "payslip", "net pay", "earnings", "deductions"]),
        ("Health Scheme", ["ayushman", "health scheme", "insurance", "policy no", "medical"]),
        ("Prescription", ["prescription", "rx", "tablet", "dosage", "diagnosis"]),
        ("Agreement", ["agreement", "hereby agree", "terms and conditions", "party of the"]),
        ("Resume", ["curriculum vitae", "resume", "work experience", "objective", "skills"]),
        ("Letter", ["dear sir", "dear madam", "yours faithfully", "yours sincerely", "subject:"]),
    ];

    /// <summary>
    /// Suggests a document name from OCR text, or null if the text is empty/unusable.
    /// </summary>
    public static string? Suggest(string? ocrText)
    {
        if (string.IsNullOrWhiteSpace(ocrText))
        {
            return null;
        }

        var normalized = ocrText.ToLowerInvariant();

        foreach (var (name, keywords) in Rules)
        {
            if (keywords.Any(k => normalized.Contains(k)))
            {
                return name;
            }
        }

        // No known document type matched - fall back to the first meaningful line of text
        // (e.g. a heading), which is often a good title.
        var heading = FirstMeaningfulLine(ocrText);
        return heading;
    }

    private static string? FirstMeaningfulLine(string text)
    {
        foreach (var rawLine in text.Split('\n'))
        {
            var line = CleanForFileName(rawLine);
            // A useful heading has a few real words and isn't just noise.
            var letterCount = line.Count(char.IsLetter);
            if (line.Length >= 4 && letterCount >= 3 && line.Split(' ').Length <= 8)
            {
                return Truncate(line, 40);
            }
        }
        return null;
    }

    // Strips characters that aren't valid in file names and collapses whitespace.
    private static string CleanForFileName(string s)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            if (Array.IndexOf(invalid, c) >= 0 || c == ':' || c == '\\' || c == '/')
            {
                sb.Append(' ');
            }
            else
            {
                sb.Append(c);
            }
        }
        return Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s.Substring(0, max).Trim();
}
