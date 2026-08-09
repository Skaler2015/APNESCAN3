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
        ("ECHS Card", ["echs", "ex-serviceman contributory", "ex servicemen contributory", "ex-servicemen contributory"]),
        ("Army ID Card", ["indian army", "army no", "regimental", "regt no", " esm ", "esm :", "ex-serviceman", "ex serviceman", "army number"]),
        ("Aadhaar Card", ["aadhaar", "aadhar", "uidai", "unique identification", "आधार"]),
        ("PAN Card", ["permanent account number", "income tax department", "pan card"]),
        ("Passport", ["passport no", "republic of india", "type p<"]),
        ("Driving Licence", ["driving licence", "driving license", "transport department", "dl no"]),
        ("Voter ID", ["election commission", "electoral", "epic no", "voter id"]),
        ("Ration Card", ["ration card", "food and civil supplies", "public distribution"]),
        ("Bank Statement", ["statement of account", "account statement", "closing balance", "transaction details"]),
        ("Cheque", ["account payee", "or bearer", "ifsc code"]),
        ("Invoice", ["tax invoice", "invoice no", "bill to", "gstin"]),
        ("Receipt", ["receipt no", "amount paid", "payment received"]),
        ("Marksheet", ["marksheet", "mark sheet", "board of secondary", "roll no", "examination"]),
        ("Salary Slip", ["salary slip", "pay slip", "payslip", "net pay", "earnings", "deductions"]),
        ("Discharge Summary", ["discharge summary", "discharge card", "date of discharge"]),
        ("Prescription", ["prescription", "diagnosis", "dosage"]),
        ("Health Scheme", ["ayushman", "health scheme", "policy no", "mediclaim"]),
        ("Medical Report", ["medical report", "pathology", "laboratory", "test report", "haemoglobin"]),
        ("Certificate", ["hereby certify", "this is to certify", "certificate"]),
        ("Agreement", ["hereby agree", "terms and conditions", "party of the first part"]),
        ("Resume", ["curriculum vitae", "work experience", "career objective"]),
        ("Letter", ["dear sir", "dear madam", "yours faithfully", "yours sincerely"]),
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

        var normalized = " " + ocrText.ToLowerInvariant() + " ";

        foreach (var (name, keywords) in Rules)
        {
            if (keywords.Any(k => normalized.Contains(k)))
            {
                return name;
            }
        }

        // No known document type matched - fall back to the best heading-like line of text. This is
        // deliberately strict: a bad guess (OCR noise like "ie en") is worse than no name at all, in
        // which case we return null and the page just keeps its number.
        return BestHeadingLine(ocrText);
    }

    // Picks the most title-like line: enough real letters, at least two words, and containing a proper
    // word (>= 4 letters). Returns the strongest candidate, or null if nothing qualifies.
    private static string? BestHeadingLine(string text)
    {
        string? best = null;
        int bestScore = 0;
        foreach (var rawLine in text.Split('\n'))
        {
            var line = CleanForFileName(rawLine);
            if (line.Length < 6)
            {
                continue;
            }
            var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int letterCount = line.Count(char.IsLetter);
            int longWords = words.Count(w => w.Count(char.IsLetter) >= 4);
            // Require real content: mostly letters, at least two words, and a genuine word (not "ie en").
            if (letterCount < 8 || words.Length < 2 || longWords < 1)
            {
                continue;
            }
            // Reject lines that are mostly digits/symbols.
            if (letterCount < line.Replace(" ", "").Length / 2)
            {
                continue;
            }
            int score = letterCount + longWords * 3;
            if (score > bestScore)
            {
                bestScore = score;
                best = Truncate(line, 40);
            }
        }
        return best;
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
