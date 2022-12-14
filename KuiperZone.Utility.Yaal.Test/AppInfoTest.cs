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

using System;
using Xunit;

namespace KuiperZone.Utility.Yaal.Test;

public class AppInfoTest
{
    [Fact]
    public void Properties_NotEmpty()
    {
        // Output
        Console.WriteLine(AppInfo.ToString(true));

        Assert.NotEmpty(AppInfo.HostName);
        Assert.NotEmpty(AppInfo.AppName);
        Assert.NotEmpty(AppInfo.AssemblyName);
        Assert.NotEmpty(AppInfo.Version.ToString());
        Assert.NotEmpty(AppInfo.Pid);
    }

}
