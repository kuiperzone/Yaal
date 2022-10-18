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

using KuiperZone.Utility.Yaal.Sinks;

namespace KuiperZone.Utility.Yaal.Internal;

/// <summary>
/// Used to provide options to <see cref="LogMessage.ToString"/>. It inherits from
/// <see cref="SinkConfig"/> because we need a way to get these values to the ToString() method.
/// </summary>
public sealed class MessageStringOptions : SinkConfig
{
    /// <summary>
    /// Default.
    /// </summary>
    public MessageStringOptions()
        : base(new SinkConfig())
    {
        LoggerConfig = new LoggerConfig();
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public MessageStringOptions(IReadOnlySinkConfig sink, IReadOnlyLoggerConfig log)
        : base(sink)
    {
        LoggerConfig = log;
    }

    /// <summary>
    /// Gets or sets the configuration of the host logger.
    /// </summary>
    public IReadOnlyLoggerConfig LoggerConfig { get; set; }

    /// <summary>
    /// Gets or sets whether to include the priority code for
    /// <see cref="LogFormat.Rfc5424"/> and <see cref="LogFormat.Bsd"/>.
    /// </summary>
    public bool IncludePriority { get; set; } = true;

    /// <summary>
    /// Gets or sets indent count for <see cref="LogFormat.Clean"/> format.
    /// </summary>
    public int IndentClean { get; set; }
}