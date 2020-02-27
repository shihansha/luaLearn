using System;
using System.Text.RegularExpressions;

internal static partial class Number
{
    private static readonly Regex reInteger = new Regex(@"^[+-]?[0-9]+$|^-?0x[0-9a-f]+$");
    private static readonly Regex reHexFloat = new Regex(@"^([0-9a-f]+(\.[0-9a-f]*)?|([0-9a-f]*\.[0-9a-f]+))(p[+\-]?[0-9]+)?$");

    public static (Int64, bool) ParseInteger(string str)
    {
        str = str.Trim();
        str = str.ToLower();

        if (!reInteger.IsMatch(str))
        {
            return (0, false);
        }
        if (str[0] == '+')
        {
            str = str[1..];
        }
        if (str.IndexOf("0x") < 0)
        {
            bool err2 = long.TryParse(str, out long i2);
            return (i2, err2);
        }

        long sign = 1;
        if (str[0] == '-')
        {
            sign = -1;
            str = str[3..];
        }
        else
        {
            str = str[2..];
        }

        if (str.Length > 16)
        {
            str = str[(str.Length - 16)..];
        }

        bool err = ulong.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out ulong i);
        return (sign * (long)i, err);

    }

    public static (double, bool) ParseFloat(string str)
    {
        str = str.Trim();
        str = str.ToLower();
        if (str.Contains("nan") || str.Contains("inf"))
        {
            return (0, false);
        }
        if (str.StartsWith("0x") && str.Length > 2)
        {
            return ParseHexFloat(str[2..]);
        }
        if (str.StartsWith("+0x") && str.Length > 3)
        {
            return ParseHexFloat(str[3..]);
        }
        if (str.StartsWith("-0x") && str.Length > 3)
        {
            var res = ParseHexFloat(str[3..]);
            res.Item1 = -res.Item1;
            return res;
        }
        bool err = double.TryParse(str, out double f);
        return (f, err);
    }

    private static (double, bool) ParseHexFloat(string str)
    {
        double i16 = 0, f16 = 0, p10 = 0;

        if (!reHexFloat.IsMatch(str))
        {
            return (0, false);
        }

        int idxOfP = str.IndexOf("p");
        if (idxOfP > 0)
        {
            string digits = str[(idxOfP + 1)..];
            str = str[..idxOfP];

            double sign = 1;
            if (digits[0] == '-')
            {
                sign = -1;
            }
            if (digits[0] == '-' || digits[0] == '+')
            {
                digits = digits[1..];
            }

            if (str.Length == 0 || digits.Length == 0)
            {
                return (0, false);
            }

            for (int i = 0; i < digits.Length; i++)
            {
                if (ParseDigit(digits[i], 10, out double x))
                {
                    p10 = p10 * 10 + x;
                }
                else
                {
                    return (0, false);
                }
            }

            p10 = sign * p10;
        }

        int idxOfDot = str.IndexOf(".");
        if (idxOfDot >= 0)
        {
            string digits = str[(idxOfDot + 1)..];
            str = str[..idxOfDot];

            if (str.Length == 0 && digits.Length == 0)
            {
                return (0, false);
            }
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                if (ParseDigit(digits[i], 16, out double x))
                {
                    f16 = (f16 + x) / 16;
                }
                else
                {
                    return (0, false);
                }
            }
        }

        for (int i = 0; i < str.Length; i++)
        {
            if (ParseDigit(str[i], 16, out double x))
            {
                i16 = i16 * 16 + x;
            }
            else
            {
                return (0, false);
            }
        }

        double f = i16 + f16;
        if (p10 != 0)
        {
            f *= Math.Pow(2, p10);
        }
        return (f, true);
    }

    private static bool ParseDigit(char digit, int @base, out double result)
    {
        if (@base == 10 || @base == 16)
        {
            if (digit >= '0' && digit <= '9')
            {
                result = digit - '0';
                return true;
            }
        }
        if (@base == 16)
        {
            if (digit >= 'a' && digit <= 'f')
            {
                result = digit - 'a' + 10;
                return true;
            }
        }
        result = -1;
        return false;
    }
}

