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
/// options for the <see cref="ConsoleSink"/> class.
/// </summary>
public sealed class ConsoleSinkOptions : SinkOptions
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ConsoleSinkOptions()
    {
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public ConsoleSinkOptions(LogFormat format, SeverityLevel threshold = SeverityLevel.Lowest)
    {
        Format = format;
        Threshold = threshold;
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public ConsoleSinkOptions(bool useColor, LogFormat format = LogFormat.General, SeverityLevel threshold = SeverityLevel.Lowest)
    {
        Format = format;
        Threshold = threshold;
        UseColor = useColor;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public ConsoleSinkOptions(ConsoleSinkOptions other)
        : base(other)
    {
        UseColor = other.UseColor;
    }

    /// <summary>
    /// Gets or sets whether log messages are to be written in a foreground color which matches
    /// their <see cref="SeverityLevel"/> value. It has no effect where not supported.
    /// </summary>
    public bool UseColor { get; set; } = true;
}