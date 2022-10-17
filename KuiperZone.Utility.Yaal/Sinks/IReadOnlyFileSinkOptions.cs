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
/// Readonly interface for the <see cref="FileSink"/> class.
/// </summary>
public interface IReadOnlyFileSinkOptions : IReadOnlySinkOptions
{
    /// <summary>
    /// Placeholder for the application name.
    /// </summary>
    const string AppTag = "{APP}";

    /// <summary>
    /// Placeholder for the application assembly name.
    /// </summary>
    const string AsmTag = "{ASM}";

    /// <summary>
    /// Placeholder for the application PID.
    /// </summary>
    const string ProcIdTag = "{PID}";

    /// <summary>
    /// Placeholder for the thread name or ID. IMPORTANT. If <see cref="FilePattern"/> contains
    /// <see cref="ThreadTag"/>, a separate file will be created for each thread.
    /// </summary>
    const string ThreadTag = "{THD}";

    /// <summary>
    /// Placeholder for the page number. This is simply a counter value which incremented
    /// each time the file reaches <see cref="MaxLines"/> and is closed and re-opened.
    /// </summary>
    const string PageTag = "{PAG}";

    /// <summary>
    /// Placeholder for the build (Rel or Dbg).
    /// </summary>
    const string BuildTag = "{BLD}";

    /// <summary>
    /// Placeholder for the user temp directory, excluding any trailing separator.
    /// The placeholder should occur only at the start of the pattern only.
    /// Example: "{TMPDIR}/Logs"
    /// </summary>
    const string TempTag = "{TMPDIR}";

    /// <summary>
    /// Placeholder for the user Document directory, excluding any trailing separator. This may be the
    /// user's home directory under Linux. If the directory does not exist, it falls back to the temporary
    /// directory. The placeholder should be specified only at the start of the pattern only.
    /// Example: "{DOCDIR}/Logs"
    /// </summary>
    const string DocTag = "{DOCDIR}";

    /// <summary>
    /// Gets the directory pattern. The pattern may contain one or more of the following placeholder
    /// variables: <see cref="AppTag"/>, <see cref="AsmTag"/>, <see cref="BuildTag"/>, <see cref="TempTag"/>
    /// and <see cref="DocTag"/>. If the value is empty, the working directory is used. The default is empty.
    /// Example: "{DOCDIR}/Logs/{APP}"
    /// </summary>
    string DirectoryPattern { get; }

    /// <summary>
    /// Gets the filename pattern, excluding any directory part. The pattern may contain one or more of
    /// the following placeholder variables: <see cref="AsmTag"/>, <see cref="AppTag"/>, <see cref="ProcIdTag"/>,
    /// <see cref="ThreadTag"/>, <see cref="PageTag"/>, <see cref="BuildTag"/>. Additionally, "{[DateFormat]}"
    /// may also be used, where the text between brace pair "{[" and "]}" will be substituted with a system time
    /// according to the DateTime format. IMPORTANT. If <see cref="FilePattern"/> contains <see cref="ThreadTag"/>,
    /// a separate file will be created for each calling thread. Example:
    /// </summary>
    string FilePattern { get; }

    /// <summary>
    /// Gets whether the directory defined by <see cref="DirectoryPattern"/> is created if it does not exist.
    /// </summary>
    bool CreateDirectory { get; }

    /// <summary>
    /// Gets the maximum lines per file before it is closed and a new one opened. A negative or zero value disables.
    /// </summary>
    long MaxLines { get; }

    /// <summary>
    /// Gets the life span of stale (old) files within the <see cref="DirectoryPattern"/> location. After this time
    /// period, files become subject to house cleaning. If <see cref="StaleLife"/> is a positive non-zero value,
    /// old log files are subject to automatic removal whenever a new file is opened. Setting this to 0 disables
    /// house cleaning of stale files. The default is <see cref="TimeSpan.Zero"/> (disabled).
    /// </summary>
    TimeSpan StaleLife { get; }

}