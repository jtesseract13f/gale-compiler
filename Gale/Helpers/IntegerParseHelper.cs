using System.Globalization;
using Antlr4.Runtime.Tree;
using Gale.AST;

namespace Gale.Helpers;

public class IntegerParseHelper
{
    /// <summary>
    /// Парсит целочисленный литерал в значение типа long.
    /// </summary>
    /// <param name="integerLit">Объект, содержащий методы для получения литералов.</param>
    /// <returns>Распарсенное целое число.</returns>
    /// <exception cref="FormatException">Если ни один литерал не удалось распарсить.</exception>
    public static long ParseInteger(GoParser.IntegerContext integerLit)
    {
        if (integerLit == null)
            throw new ArgumentNullException(nameof(integerLit));

        // Проверяем литералы в порядке приоритета (кроме IMAGINARY_LIT, т.к. это не целое)
        ITerminalNode literal;
        string text;

        // 1. Рунный литерал (например, 'a')
        literal = integerLit.RUNE_LIT();
        if (literal != null)
        {
            text = literal.GetText();
            if (!string.IsNullOrEmpty(text))
                return ParseRuneLiteral(text);
        }

        // 2. Восьмеричный литерал
        literal = integerLit.OCTAL_LIT();
        if (literal != null)
        {
            text = literal.GetText();
            if (!string.IsNullOrEmpty(text))
                return ParseNumberWithBase(text, 8);
        }

        // 3. Шестнадцатеричный литерал
        literal = integerLit.HEX_LIT();
        if (literal != null)
        {
            text = literal.GetText();
            if (!string.IsNullOrEmpty(text))
                return ParseNumberWithBase(text, 16);
        }

        // 4. Десятичный литерал
        literal = integerLit.DECIMAL_LIT();
        if (literal != null)
        {
            text = literal.GetText();
            if (!string.IsNullOrEmpty(text))
                return ParseDecimal(text);
        }

        // 5. Двоичный литерал
        literal = integerLit.BINARY_LIT();
        if (literal != null)
        {
            text = literal.GetText();
            if (!string.IsNullOrEmpty(text))
                return ParseNumberWithBase(text, 2);
        }

        throw new FormatException("Не удалось распарсить целочисленный литерал.");
    }

    /// <summary>
    /// Парсит строку как число с заданным основанием, удаляя возможные префиксы.
    /// </summary>
    private static long ParseNumberWithBase(string text, int radix)
    {
        // Удаляем префиксы, если они есть (для универсальности)
        string normalized = text.Trim();
        if (radix == 16 && (normalized.StartsWith("0x", StringComparison.OrdinalIgnoreCase)))
            normalized = normalized.Substring(2);
        else if (radix == 2 && (normalized.StartsWith("0b", StringComparison.OrdinalIgnoreCase)))
            normalized = normalized.Substring(2);
        else if (radix == 8 && (normalized.StartsWith("0o", StringComparison.OrdinalIgnoreCase) ||
                                normalized.StartsWith("0O", StringComparison.Ordinal)))
            normalized = normalized.Substring(2);

        return Convert.ToInt64(normalized, radix);
    }

    /// <summary>
    /// Парсит десятичный литерал (без префикса).
    /// </summary>
    private static long ParseDecimal(string text)
    {
        return long.Parse(text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Парсит рунный литерал вида 'a' или '\n' в числовой код символа.
    /// </summary>
    private static long ParseRuneLiteral(string text)
    {
        if (string.IsNullOrEmpty(text) || text.Length < 3 || text[0] != '\'' || text[text.Length - 1] != '\'')
            throw new FormatException("Некорректный рунный литерал.");

        string inner = text.Substring(1, text.Length - 2); // убираем кавычки
        if (inner.Length == 1)
        {
            // Обычный символ
            return inner[0];
        }
        else if (inner.Length > 1 && inner[0] == '\\')
        {
            // Escape-последовательность — используем встроенный механизм
            string unescaped = System.Text.RegularExpressions.Regex.Unescape($"'{inner}'");
            if (unescaped.Length == 3 && unescaped[0] == '\'' && unescaped[2] == '\'')
                return unescaped[1];
            // Если это суррогатная пара или другой случай, можно вернуть первый char
            return unescaped.Length >= 2 ? unescaped[1] : unescaped[0];
        }
        else
        {
            throw new FormatException("Неподдерживаемый рунный литерал.");
        }
    }
}