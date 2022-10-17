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
/// Construction configuration for the <see cref="BufferSink"/> class. Implements
/// <see cref="IReadOnlyBufferConfig"/> and provides setters.
/// </summary>
public sealed class BufferConfig : SinkConfig, IReadOnlyBufferConfig
{
    /// <summary>
    /// Default constructor with options.
    /// </summary>
    public BufferConfig(FormatKind format = FormatKind.Text, SeverityLevel threshold = SeverityLevel.Lowest)
        : base(format, threshold)
    {
    }

    /// <summary>
    /// Default constructor with options.
    /// </summary>
    public BufferConfig(int capacity, FormatKind format = FormatKind.Text, SeverityLevel threshold = SeverityLevel.Lowest)
        : base(format, threshold)
    {
        Capacity = capacity;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public BufferConfig(IReadOnlyBufferConfig other)
        : base(other)
    {
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyBufferConfig.Capacity"/> and provides a setter.
    /// </summary>
    public int Capacity { get; set; } = 1000;

}