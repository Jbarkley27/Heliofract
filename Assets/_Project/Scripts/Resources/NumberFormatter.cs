using System;
using System.Globalization;

public enum NumberFormatMode
{
    Incremental,
    Scientific
}

public static class NumberFormatter
{
    private static readonly string[] Suffixes =
    {
        "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc"
    };

    public static string FormatNumber(double value, NumberFormatMode mode = NumberFormatMode.Incremental)
    {
        if (double.IsNaN(value))
        {
            return "0";
        }

        if (double.IsInfinity(value))
        {
            return value > 0 ? "∞" : "-∞";
        }

        return mode == NumberFormatMode.Scientific
            ? FormatScientific(value)
            : FormatIncremental(value);
    }

    public static string FormatRate(double value, NumberFormatMode mode = NumberFormatMode.Incremental)
    {
        return FormatNumber(value, mode) + "/s";
    }

    public static string FormatCost(double value, string resourceName, NumberFormatMode mode = NumberFormatMode.Incremental)
    {
        return "Cost: " + FormatNumber(value, mode) + " " + resourceName;
    }

    private static string FormatIncremental(double value)
    {
        double absValue = Math.Abs(value);

        if (absValue < 1000)
        {
            return TruncateTowardZero(value).ToString("0", CultureInfo.InvariantCulture);
        }

        int suffixIndex = 0;

        while (absValue >= 1000 && suffixIndex < Suffixes.Length - 1)
        {
            value /= 1000;
            absValue /= 1000;
            suffixIndex++;
        }

        if (suffixIndex >= Suffixes.Length - 1 && absValue >= 1000)
        {
            return FormatScientific(value * Math.Pow(1000, suffixIndex));
        }

        value = RoundToSignificantDigits(value, 3);
        absValue = Math.Abs(value);

        if (absValue >= 1000 && suffixIndex < Suffixes.Length - 1)
        {
            value /= 1000;
            suffixIndex++;
        }

        string format;

        if (Math.Abs(value) >= 100)
        {
            format = "0";
        }
        else if (Math.Abs(value) >= 10)
        {
            format = "0.#";
        }
        else
        {
            format = "0.##";
        }

        return value.ToString(format, CultureInfo.InvariantCulture) + Suffixes[suffixIndex];
    }

    private static string FormatScientific(double value)
    {
        if (Math.Abs(value) < 1000)
        {
            return TruncateTowardZero(value).ToString("0", CultureInfo.InvariantCulture);
        }

        return value.ToString("0.##e0", CultureInfo.InvariantCulture);
    }

    private static double TruncateTowardZero(double value)
    {
        return value < 0 ? Math.Ceiling(value) : Math.Floor(value);
    }

    private static double RoundToSignificantDigits(double value, int significantDigits)
    {
        if (value == 0)
        {
            return 0;
        }

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(value))) + 1);
        return scale * Math.Round(value / scale, significantDigits);
    }
}
