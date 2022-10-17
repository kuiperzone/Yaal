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
/// Construction options for the <see cref="FileSink"/> class. Implements
/// <see cref="IReadOnlyFileSinkOptions"/> and provides setters.
/// </summary>
public sealed class FileSinkOptions : SinkOptions, IReadOnlyFileSinkOptions
{
    /// <summary>
    /// Default constructor with options.
    /// </summary>
    public FileSinkOptions(FormatKind format = FormatKind.Text, SeverityLevel threshold = SeverityLevel.Lowest)
        : base(format, threshold)
    {
    }

    /// <summary>
    /// Constructor with <see cref="IReadOnlyFileSinkOptions.DirectoryPattern"/> value.
    /// </summary>
    public FileSinkOptions(string directory, FormatKind format = FormatKind.Text, SeverityLevel threshold = SeverityLevel.Lowest)
        : base(format, threshold)
    {
        DirectoryPattern = directory;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public FileSinkOptions(IReadOnlyFileSinkOptions other)
        : base(other)
    {
        DirectoryPattern = other.DirectoryPattern;
        FilePattern = other.FilePattern;
        CreateDirectory = other.CreateDirectory;
        MaxLines = other.MaxLines;
        StaleLife = other.StaleLife;
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyFileSinkOptions.DirectoryPattern"/> and provides a setter.
    /// </summary>
    public string DirectoryPattern { get; set; } = "{DOCDIR}/Logs/{ASM}";

    /// <summary>
    /// Implements <see cref="IReadOnlyFileSinkOptions.FilePattern"/> and provides a setter.
    /// </summary>
    public string FilePattern { get; set; } = "{APP}-{PID}-{THD}-{[yyyyMMddTHHmmss]}.{PAG}.log";

    /// <summary>
    /// Implements <see cref="IReadOnlyFileSinkOptions.CreateDirectory"/> and provides a setter.
    /// </summary>
    public bool CreateDirectory { get; set; } = true;

    /// <summary>
    /// Implements <see cref="IReadOnlyFileSinkOptions.MaxLines"/> and provides a setter.
    /// </summary>
    public long MaxLines { get; set; } = 100000;

    /// <summary>
    /// Implements <see cref="IReadOnlyFileSinkOptions.StaleLife"/> and provides a setter.
    /// </summary>
    public TimeSpan StaleLife { get; set; }

}