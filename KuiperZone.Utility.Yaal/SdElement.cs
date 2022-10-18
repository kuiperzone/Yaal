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
using KuiperZone.Utility.Yaal.Internal;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// Structured RFC 5424 SD-ELEMENT. The class inherits <see cref="SdDictionary{T}"/>
/// where T is string, so as provide a sequence of SD-PARAM key-values.
/// </summary>
public class SdElement : SdDictionary<string>
{
    /// <summary>
    /// Constructor with value for <see cref="Id"/>. The id should not contain printable
    /// ASCII only and or space characters. The format is "name@enterprise_number", unless an IANA ID is used.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public SdElement(string id)
	{
        LogUtil.AssertId(id);
        Id = id;
    }

    /// <summary>
    /// Constructor with value for <see cref="Id"/>, and an initial key-value.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public SdElement(string id, string key, string value)
        : this(id)
	{
        Add(key, value);
    }

    /// <summary>
    /// Gets the SD-ID (unique per message). The format is "name@enterprise_number", unless
    /// an IANA ID is used. The value is immutable.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Overrides.
    /// </summary>
    public override void AppendTo(StringBuilder buffer, IReadOnlyLoggerOptions opts)
    {
        buffer.Append('[');
        buffer.Append(Id);
        base.AppendTo(buffer, opts);
        buffer.Append(']');
    }

    /// <summary>
    /// Overrides to give Id.GetHashCode().
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

