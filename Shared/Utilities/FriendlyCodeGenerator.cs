using AbcLettingAgency.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AbcLettingAgency.Shared.Utilities;

public sealed class FriendlyCodeGenerator
{
    private readonly AppDbContext _db;
    private static readonly Regex NonAlnumDash = new("[^A-Z0-9-]", RegexOptions.Compiled);
    private static readonly Regex MultiDash = new("-{2,}", RegexOptions.Compiled);

    public FriendlyCodeGenerator(AppDbContext db) => _db = db;

    /// <summary>
    /// Returns a unique property code (<= 50 chars).
    /// If 'suggested' is provided, it's sanitized and uniquified.
    /// Otherwise, a code is built from address/city and uniquified.
    /// </summary>
    public async Task<string> GenerateForPropertyAsync(
        string? suggested,
        string addressLine1,
        string? city,
        CancellationToken ct)
    {
        // 1) Base
        var baseCode = !string.IsNullOrWhiteSpace(suggested)
            ? Sanitize(suggested!)
            : BuildFromAddress(addressLine1, city);

        if (string.IsNullOrWhiteSpace(baseCode))
            baseCode = "PROP";

        // Ensure base length (leave room for suffix)
        baseCode = TrimToMax(baseCode, 40);

        // 2) Uniquify with short suffix if needed
        var candidate = baseCode;
        const int maxAttempts = 12;
        for (var i = 0; i < maxAttempts; i++)
        {
            var exists = await _db.Properties.AnyAsync(p => p.Code == candidate, ct);
            if (!exists) return candidate;

            var suffix = RandBase36(4); // 4 chars: ~1.6M combos
            candidate = TrimToMax($"{baseCode}-{suffix}", 50);
        }

        // Extremely unlikely fallback
        return TrimToMax($"{baseCode}-{RandBase36(6)}", 50);
    }

    private static string BuildFromAddress(string addressLine1, string? city)
    {
        // Address tokens
        var tokens = (addressLine1 ?? "").Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var houseNo = new string(tokens.FirstOrDefault(t => t.Any(char.IsDigit))?.Where(char.IsLetterOrDigit).ToArray() ?? Array.Empty<char>());

        // Try to pick a meaningful street token (non-numeric)
        var streetWord = tokens.FirstOrDefault(t => !t.Any(char.IsDigit)) ?? "";
        streetWord = Sanitize(streetWord);
        if (streetWord.Length > 12) streetWord = streetWord[..12];

        // City abbrev (first 3 letters)
        var cityAbbr = string.IsNullOrWhiteSpace(city) ? "" : Sanitize(city!);
        if (cityAbbr.Length > 3) cityAbbr = cityAbbr[..3];

        // Compose pieces that exist
        var parts = new[] { cityAbbr, streetWord, houseNo }
            .Where(s => !string.IsNullOrWhiteSpace(s));

        var code = string.Join("-", parts);
        return string.IsNullOrWhiteSpace(code) ? "PROP" : code;
    }

    private static string Sanitize(string input)
    {
        // Uppercase, remove diacritics, keep A-Z 0-9 and '-'
        var up = input.Trim().ToUpperInvariant();
        var de = RemoveDiacritics(up);
        var only = NonAlnumDash.Replace(de, "-");
        only = MultiDash.Replace(only, "-").Trim('-');
        return only;
    }

    private static string RemoveDiacritics(string text)
    {
        var norm = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(capacity: norm.Length);
        foreach (var c in norm)
        {
            var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static string TrimToMax(string s, int max) =>
        s.Length <= max ? s : s[..max].Trim('-');

    private static string RandBase36(int length)
    {
        // Cryptographically strong random base36 string
        Span<byte> bytes = stackalloc byte[length];
        RandomNumberGenerator.Fill(bytes);
        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = alphabet[bytes[i] % alphabet.Length];
        return new string(chars);
    }
}
