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
/// RFC 5424 data class. The class inherits <see cref="SdDictionary{T}"/> where T is
/// <see cref="SdElement"/>. Here, the dictionary key is the SD-ELEMENT SD-ID, and each
/// <see cref="SdElement"/> comprises a sequence of SD-PARAM key-values. Individual
/// values are access as: Data[SD-ID][SD-NAME].
/// </summary>
public sealed class StructuredData : SdDictionary<SdElement>
{
    /// <summary>
    /// Overrides.
    /// </summary>
    public override void AppendTo(StringBuilder buffer)
    {
        foreach (var item in this)
        {
            // Call the overloaded method with its Id
            item.Value.AppendTo(buffer, item.Key);
        }
    }
}