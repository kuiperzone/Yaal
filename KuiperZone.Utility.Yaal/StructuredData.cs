// -----------------------------------------------------------------------------
// PROJECT   : Yaal
// COPYRIGHT : Andy Thomas (C) 2022
// LICENSE   : GPL-3.0-or-later
// HOMEPAGE  : https://github.com/kuiperzone/Yaal
//
// This file is part of Yaal.
//
// Yaal is free software: you can redistribute it and/or modify it under the terms of the GNU General
// Public License as published by the Free Software Foundation, either version 3 of the License, or at your option
// any later version.
//
// Yaal is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with Yaal.
// If not, see <https://www.gnu.org/licenses/>.
// -----------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// Structured RFC 5424 SD-ELEMENT. The class inherits <see cref="SdDictionary{T}"/>
/// where T is <see cref="SdElement"/>. In other words, this is a dictionary where the
/// key is an SD-ELEMENT SD-ID. Parameter values are access as: Data[SD-ID][SD-NAME].
/// </summary>
public sealed class StructuredData : SdDictionary<SdElement>
{
	/**
	 * Static routine which returns true if key is a valid SD-NAME or SD-ID. I.e. it is not empty, and
	 * all characters are within the printable ASCII range, and they do not contain: '=', SP, ']', '"'.
	 */
	public static bool IsValidKey([NotNullWhen(true)] string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        foreach(var c in key)
        {
            if (c <= ' ' || c > '~' || c == '=' || c == '"' || c == ']')
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Asserts <see cref="IsValidKey"/> is true.
    /// </summary>
    /// <exception cref="ArgumentNullException">name</exception>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public static void AssertKey([NotNull] string? name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (!IsValidKey(name))
        {
            throw new ArgumentException("Invalid RFC 5424 name value " + name);
        }
    }

    /// <summary>
	/// Escapes the ASCII string, replacing non-printing characters with escaped numerical codes.
    /// If "escapeExt" is true, characters >= 0x7F are also escaped. The "chars" value provides a
    /// list of additional characters to escape, i.e. "\\]\"".
    /// </summary>
    public static string Escape(string? obj, bool escapeExt, string chars = "")
    {
        // https://www.rfc-editor.org/rfc/rfc5424#section-6.3.5
        // https://www.rfc-editor.org/rfc/rfc5424#section-6.4
        string s = obj?.ToString() ?? "";
        var buffer = new StringBuilder(s.Length + 16);

        foreach (var c in s)
        {
            if (escapeExt && c >= '\xFF')
            {
                AppendHex(c, buffer);
            }
            else
            if (c >= 0 && c < 0x20)
            {
                switch (c)
                {
                    case '\x00': buffer.Append("\\0"); break;
                    case '\x07': buffer.Append("\\a"); break;
                    case '\x08': buffer.Append("\\b"); break;
                    case '\x09': buffer.Append("\\t"); break;
                    case '\x0A': buffer.Append("\\n"); break;
                    case '\x0B': buffer.Append("\\v"); break;
                    case '\x0C': buffer.Append("\\f"); break;
                    case '\x0D': buffer.Append("\\r"); break;
                    case '\x1B': buffer.Append("\\e"); break;
                    default: AppendHex(c, buffer); break;
                }
            }
            else
            if (chars.IndexOf(c) > -1)
            {
                buffer.Append('\\');
                buffer.Append(c);
            }
            else
            {
                buffer.Append(c);
            }
        }

        return buffer.ToString();
    }

    private static void AppendHex(UInt16 c, StringBuilder buffer)
    {
        if (c <= 0xFF)
        {
            buffer.Append("\\x");
            buffer.Append(c.ToString("X2"));
        }
        else
        {
            buffer.Append("\\u");
            buffer.Append(c.ToString("X4"));
        }
    }

}