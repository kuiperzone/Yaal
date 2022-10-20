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

using System.Text;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// Structured RFC 5424 SD-ELEMENT. The class inherits <see cref="SdDictionary{T}"/>
/// where T is string, so as provide a sequence of SD-PARAM key-values.
/// </summary>
public class SdElement : SdDictionary<string>
{
    /// <summary>
    /// Default constructor. The format is "name@enterprise_number", unless an IANA ID is used.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public SdElement()
	{
    }

    /// <summary>
    /// Constructor with initial key-value.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public SdElement(string key, string value)
        : base(key, value)
	{
    }

    /// <summary>
    /// Overloads with Id.
    /// </summary>
    public void AppendTo(StringBuilder buffer, string id)
    {
        buffer.Append('[');
        buffer.Append(id);
        base.AppendTo(buffer);
        buffer.Append(']');
    }
}

