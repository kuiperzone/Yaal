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

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// Options for the <see cref="BufferSink"/> class.
/// </summary>
public sealed class BufferSinkOptions : SinkOptions
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public BufferSinkOptions()
    {
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public BufferSinkOptions(LogFormat format, SeverityLevel threshold = SeverityLevel.Lowest)
    {
        Format = format;
        Threshold = threshold;
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public BufferSinkOptions(int capacity, LogFormat format = LogFormat.General, SeverityLevel threshold = SeverityLevel.Lowest)
        : base(format, threshold)
    {
        Capacity = capacity;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public BufferSinkOptions(BufferSinkOptions other)
        : base(other)
    {
    }

    /// <summary>
    /// Gets or sets the maximum entry count. When reached, old entries are removed.
    /// A value of 0 or less is invalid.
    /// </summary>
    public int Capacity { get; set; } = 1000;

}