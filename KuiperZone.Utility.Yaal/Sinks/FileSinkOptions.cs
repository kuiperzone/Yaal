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

using System.Globalization;
using System.Runtime.InteropServices;
using KuiperZone.Utility.Yaal.Internal;

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// Options for the <see cref="FileLogSink"/> class.
/// </summary>
public sealed class FileSinkOptions : SinkOptions
{
    /// <summary>
    /// Placeholder for the application name.
    /// </summary>
    public const string AppTag = "{APP}";

    /// <summary>
    /// Placeholder for the application assembly name.
    /// </summary>
    public const string AsmTag = "{ASM}";

    /// <summary>
    /// Placeholder for the application PID.
    /// </summary>
    public const string ProcIdTag = "{PID}";

    /// <summary>
    /// Placeholder for the thread name or ID. IMPORTANT. If <see cref="FilePattern"/> contains
    /// <see cref="ThreadTag"/>, a separate file will be created for each thread.
    /// </summary>
    public const string ThreadTag = "{THD}";

    /// <summary>
    /// Placeholder for the page number. This is simply a counter value which incremented
    /// each time the file reaches <see cref="MaxLines"/> and is closed and re-opened.
    /// </summary>
    public const string PageTag = "{PAG}";

    /// <summary>
    /// Placeholder for the build (Rel or Dbg).
    /// </summary>
    public const string BuildTag = "{BLD}";

    /// <summary>
    /// Placeholder for the user temp directory, excluding any trailing separator.
    /// The placeholder should occur only at the start of the pattern only.
    /// Example: "{TMPDIR}/Logs"
    /// </summary>
    public const string TempTag = "{TMPDIR}";

    /// <summary>
    /// Placeholder for the user Document directory, excluding any trailing separator. This may be the
    /// user's home directory under Linux. If the directory does not exist, it falls back to the temporary
    /// directory. The placeholder should be specified only at the start of the pattern only.
    /// Example: "{DOCDIR}/Logs"
    /// </summary>
    public const string DocTag = "{DOCDIR}";

    /// <summary>
    /// Extension for all log files.
    /// </summary>
    public const string LogExt = ".log";

    /// <summary>
    /// Extension for old log files.
    /// </summary>
    public const string OldExt = ".old";


    /// <summary>
    /// Default constructor.
    /// </summary>
    public FileSinkOptions()
    {
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public FileSinkOptions(LogFormat format, SeverityLevel threshold = SeverityLevel.Lowest)
    {
        Format = format;
        Threshold = threshold;
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public FileSinkOptions(string directory, LogFormat format = LogFormat.General, SeverityLevel threshold = SeverityLevel.Lowest)
        : base(format, threshold)
    {
        DirectoryPattern = directory;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public FileSinkOptions(FileSinkOptions other)
        : base(other)
    {
        DirectoryPattern = other.DirectoryPattern;
        FilePattern = other.FilePattern;
        CreateDirectory = other.CreateDirectory;
        IndentCount = other.IndentCount;
        MaxLines = other.MaxLines;
        RemoveLogsOnStart = other.RemoveLogsOnStart;
        StaleLife = other.StaleLife;
    }

    /// <summary>
    /// Gets or sets the directory pattern. The pattern may contain one or more of the following placeholder
    /// variables: <see cref="AppTag"/>, <see cref="AsmTag"/>, <see cref="BuildTag"/>, <see cref="TempTag"/>
    /// and <see cref="DocTag"/>. The value cannot be empty. Example: "{DOCDIR}/Logs/{APP}"
    /// </summary>
    public string DirectoryPattern { get; set; } = "{DOCDIR}/Logs/{ASM}";

    /// <summary>
    /// Gets or sets the filename pattern, excluding any directory part. The pattern may contain one or more of
    /// the following placeholder variables: <see cref="AsmTag"/>, <see cref="AppTag"/>, <see cref="ProcIdTag"/>,
    /// <see cref="ThreadTag"/>, <see cref="PageTag"/>, <see cref="BuildTag"/>. Additionally, "{[DateFormat]}"
    /// may also be used, where the text between brace pair "{[" and "]}" will be substituted with a system time
    /// according to the DateTime format. IMPORTANT. If <see cref="FilePattern"/> contains <see cref="ThreadTag"/>,
    /// a separate file will be created for each calling thread. The value cannot be empty.
    /// </summary>
    public string FilePattern { get; set; } = "{APP}-{PID}-{THD}-{[yyyyMMddTHHmmss]}.{PAG}.log";

    /// <summary>
    /// Gets whether the directory defined by <see cref="DirectoryPattern"/> is created if it does not exist.
    /// </summary>
    public bool CreateDirectory { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of pad (indent) characters in the <see cref="LogFormat.General"/> and
    /// <see cref="LogFormat.TextOnly"/> formats (it does nothing in other formats). This setting is included
    /// specifically for use with the <see cref="LogFormat.General"/> format in conjunction with
    /// <see cref="Logger.Debug"/> statements. In this scenario, it serves as padding between leading stack trace
    /// information (method name and line) and the message content. This makes a log files easier to read, as the
    /// start of message contents are aligned. Typically, it should be set to a large value of 80 to 100 characters
    /// to leave sufficient room for stack information. In other modes, it simply serves as an indentation count and
    /// is not particularly useful. The default is 0 (disabled).
    /// </summary>
    public int IndentCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum lines per file before it is closed and a new one opened. The default is 10,000.
    /// A negative or zero value disables.
    /// </summary>
    public long MaxLines { get; set; } = 100000;

    /// <summary>
    /// Gets or sets whether to attempt to remove any pre-existing log files (with extension ".log") within the
    /// <see cref="DirectoryPattern"/> location immediately on start. This may be suitable for desktop applications,
    /// rather than web or server apps. See also <see cref="StaleLife"/>. Default is false.
    /// </summary>
    public bool RemoveLogsOnStart { get; set; }

    /// <summary>
    /// Gets or sets the life span of stale (old) files (with extension ".log") within the <see cref="DirectoryPattern"/>
    /// location. If <see cref="StaleLife"/> is a positive non-zero value, old log files are subject to automatic
    /// removal whenever a new file is opened.  The default is <see cref="TimeSpan.Zero"/> (disabled).
    /// </summary>
    public TimeSpan StaleLife { get; set; }

    /// <summary>
    /// Gets the directory formed by the expansion of <see cref="DirectoryPattern"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid DirectoryPattern</exception>
    public string GetDirectoryName()
    {
        var rslt = SubstituteCommon(DirectoryPattern);

        if (!string.IsNullOrEmpty(rslt))
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                rslt = rslt.Replace('\\', '/');
            }

            rslt = SubstituteDirectory(rslt, FileSinkOptions.TempTag);
            rslt = SubstituteDirectory(rslt, FileSinkOptions.DocTag);
            return rslt;
        }

        throw new ArgumentException($"{nameof(DirectoryPattern)} empty");
    }

    /// <summary>
    /// Gets the filename formed by the expansion of <see cref="FileSinkOptions.FilePattern"/>.
    /// Note that result may change on each call.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid FilePattern</exception>
    public string GetFileName(int page)
    {
        var rslt = SubstituteCommon(FilePattern);

        if (string.IsNullOrEmpty(rslt))
        {
            throw new ArgumentException($"{nameof(rslt)} is empty");
        }

        const int ZeroPad = 3;
        rslt = rslt.Replace(FileSinkOptions.ProcIdTag, AppInfo.Pid.PadLeft(ZeroPad, '0'), StringComparison.OrdinalIgnoreCase);
        rslt = rslt.Replace(FileSinkOptions.ThreadTag, LogUtil.ThreadName, StringComparison.OrdinalIgnoreCase);
        rslt = rslt.Replace(FileSinkOptions.PageTag, page.ToString(CultureInfo.InvariantCulture).PadLeft(ZeroPad, '0'), StringComparison.OrdinalIgnoreCase);

        rslt = SubstituteDateTime(rslt);

        if (!rslt.EndsWith(LogExt, StringComparison.OrdinalIgnoreCase))
        {
            rslt += LogExt;
        }

        return rslt;
    }

    private static string SubstituteCommon(string pattern)
    {
        pattern = pattern.Trim();
        pattern = pattern.Replace(FileSinkOptions.AppTag, AppInfo.AppName, StringComparison.OrdinalIgnoreCase);
        pattern = pattern.Replace(FileSinkOptions.AsmTag, AppInfo.AssemblyName, StringComparison.OrdinalIgnoreCase);
        pattern = pattern.Replace(FileSinkOptions.BuildTag, AppInfo.IsDebug ? "Dbg" : "Rel", StringComparison.OrdinalIgnoreCase);

        return pattern;
    }

    private static string SubstituteDateTime(string pattern)
    {
        // Data substitution
        // I.e. {APP}-{[yyyyMMddTHHmmssZ]}-{THD}-{PAG}.log
        const string Date1 = "{[";
        const string Date2 = "]}";

        while(true)
        {
            int p1 = pattern.IndexOf(Date1);

            if (p1 < 0)
            {
                return pattern;
            }

            // Date substitution.
            int p2 = pattern.IndexOf(Date2);

            if (p2 <= p1)
            {
                throw new ArgumentException("Pattern contains '{[' without corresponding closure");
            }

            int len = p2 - p1 - Date1.Length;
            string tm = pattern.Substring(p1 + Date1.Length, len).Trim();

            if (tm.EndsWith("Z"))
            {
                tm = DateTime.UtcNow.ToString(tm);
            }
            else
            {
                tm = DateTime.Now.ToString(tm);
            }

            pattern = pattern.Substring(0, p1) + tm + pattern.Substring(p2 + Date2.Length);
        }
    }

    private static string SubstituteDirectory(string pattern, string tag)
    {
        if (pattern.Contains(tag, StringComparison.OrdinalIgnoreCase))
        {
            if (!pattern.StartsWith(tag, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"{tag} must only be used at start of the {nameof(DirectoryPattern)}");
            }

            var value = Path.GetTempPath();

            if (tag == FileSinkOptions.DocTag)
            {
                try
                {
                    value = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                catch
                {
                    // Fall to temporary
                }
            }

            return pattern.Replace(tag, value.TrimEnd('/', '\\'), StringComparison.OrdinalIgnoreCase);
        }

        return pattern;
    }

}