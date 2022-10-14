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

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// Construction options for the <see cref="FileSink"/> class.
/// </summary>
public class FileOptions
{
    private int _pageCounter;

    /// <summary>
    /// Placeholder for the application name.
    /// </summary>
    public const string AppTag = "{APP}";

    /// <summary>
    /// Placeholder for the application PID.
    /// </summary>
    public const string ProcIdTag = "{PID}";

    /// <summary>
    /// Placeholder for the thread-id.
    /// </summary>
    public const string ThreadIdTag = "{TID}";

    /// <summary>
    /// Placeholder for the thread name.
    /// </summary>
    public const string ThreadNameTag = "{TNM}";

    /// <summary>
    /// Placeholder for the page number. This is simply a counter value.
    /// </summary>
    public const string PageTag = "{PAG}";

    /// <summary>
    /// Placeholder for the build (Rel or Dbg).
    /// </summary>
    public const string BuildTag = "{BLD}";

    /// <summary>
    /// Placeholder for the user temp directory, excluding any trailing separator. If used,
    /// it should occur only at the start of the pattern.
    /// </summary>
    public const string TempDirTag = "{TMPDIR}";

    /// <summary>
    /// Gets or sets the directory pattern. The pattern may contain one or more of the following placeholder
    /// variables: <see cref="AppTag"/>, <see cref="BuildTag"/> and <see cref="TempDirTag"/>.
    /// If the value is empty, the working directory is used. The default is "{TMPDIR}/Logs".
    /// </summary>
    public string DirectoryPattern { get; set; } = TempDirTag + "/Logs/" + AppTag;

    /// <summary>
    /// Gets or sets the filename pattern, excluding any directory part. The pattern may contain one or more of
    /// the following placeholder variables: <see cref="AppTag"/>, <see cref="ProcIdTag"/>, <see cref="ThreadIdTag"/>,
    /// <see cref="ThreadNameTag"/>, <see cref="PageTag"/>, <see cref="BuildTag"/>, <see cref="UserTag"/>.
    /// Additionally, "{[DateFormat]}" may also be used, where the text between brace pair "{[" and "]}" will
    /// be substituted with a system time according to the DateTime format.
    /// </summary>
    public string FilePattern { get; set; } = ProcIdTag + "-" + ThreadNameTag + "-{[yyyyMMddTHHmmss]}." + PageTag + ".log";

    /// <summary>
    /// Gets or sets whether the directory defined by <see cref="DirectoryPattern"/> is created if it does not exist.
    /// </summary>
    public bool CreateDirectory { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum lines per file before it is closed and a new one opened.
    /// </summary>
    public long MaxLines { get; set; } = 100000;

    /// <summary>
    /// Gets the directory from to <see cref="FilePattern"/>.
    /// </summary>
    public string GetDirectoryName()
    {
        var name = DirectoryPattern.Trim();

        if (name.Length != 0)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                name = name.Replace('\\', '/');
            }

            name = name.Replace(AppTag, AppInfo.AssemblyName);
            name = name.Replace(BuildTag, AppInfo.IsDebug ? "Dbg" : "Rel");

            if (name.Contains(TempDirTag))
            {
                if (!name.StartsWith(TempDirTag))
                {
                    throw new ArgumentException($"{TempDirTag} must only be used at start of {nameof(DirectoryPattern)}");
                }

                name = name.Replace(TempDirTag, Path.GetTempPath().TrimEnd('/', '\\'));
            }
        }

        return name;
    }

    /// <summary>
    /// Gets the filename built from to <see cref="FilePattern"/>.
    /// </summary>
    public string GetFileName()
    {
        // Generate full file path. If pageName is -1, it
        // returns wildcard variant used for removal of stale files.
        const string Date1 = "{[";
        const string Date2 = "]}";

        string fname = FilePattern.Trim();

        // I.e. {APP}-{[yyyyMMddTHHmmssZ]}-ID{TID}-{PAG}.log
        int p1 = fname.IndexOf(Date1);

        if (p1 > 0)
        {
            // Date substitution.
            int p2 = fname.IndexOf(Date2);

            if (p2 <= p1)
            {
                throw new FormatException(nameof(FilePattern) + " contains '{[' without corresponding closure");
            }

            int len = p2 - p1 - Date1.Length;
            string tm = fname.Substring(p1 + Date1.Length, len).Trim();

            if (tm.EndsWith("Z"))
            {
                tm = DateTime.UtcNow.ToString(tm);
            }
            else
            {
                tm = DateTime.Now.ToString(tm);
            }

            fname = fname.Substring(0, p1) + tm + fname.Substring(p2 + Date2.Length);
        }

        const int ZeroPad = 3;
        fname = fname.Replace(AppTag, AppInfo.AssemblyName);
        fname = fname.Replace(ProcIdTag, AppInfo.ProcId.PadLeft(ZeroPad, '0'));
        fname = fname.Replace(ThreadIdTag, AppInfo.ThreadId.PadLeft(ZeroPad, '0'));
        fname = fname.Replace(ThreadNameTag, AppInfo.ThreadName);
        fname = fname.Replace(PageTag, (++_pageCounter).ToString(CultureInfo.InvariantCulture).PadLeft(ZeroPad, '0'));
        fname = fname.Replace(BuildTag, AppInfo.IsDebug ? "Dbg" : "Rel");

        return fname;
    }


}