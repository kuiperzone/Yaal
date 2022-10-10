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

using System.Net;
using System.Runtime.InteropServices;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// Interface for readonly logging options.
/// </summary>
public class LogOptions : IReadOnlyLogOptions
{
    public LogOptions()
    {
        AppName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "";

        if (AppName.Length == 0)
        {
            AppName = "Unknown";
        }

        LocalHost = Dns.GetHostName();

        if (LocalHost.Length == 0)
        {
            LocalHost = "Unknown";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Format = FormatKind.Text;
        }
        else
        {
            Format = FormatKind.Rfc5424;
        }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.AppName"/> and provides a setter.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.LocalHost"/> and provides a setter.
    /// </summary>
    public string LocalHost { get; set; }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.Format"/> and provides a setter.
    /// </summary>
    public FormatKind Format { get; set; }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.Facility"/> and provides a setter.
    /// </summary>
	public FacilityId Facility { get; set; } = FacilityId.User;

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.MaxTextLength"/> and provides a setter.
    /// </summary>
    public int MaxTextLength { get; set; }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.DebugSdId"/> and provides a setter.
    /// </summary>
	public string DebugSdId { get; set; } = "DGB@00000000";
}