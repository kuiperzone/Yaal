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
using System.IO;
using KuiperZone.Utility.Yaal.Internal;
using KuiperZone.Utility.Yaal.Sinks;
using Xunit;

namespace KuiperZone.Utility.Yaal.Test;

public class FileSinkOptionsTest
{
    [Fact]
    public void GetFileName_SubsAllTags()
    {
        var year = DateTime.UtcNow.Year.ToString();

        var opts = new FileSinkOptions();
        opts.FilePattern = "{ASM}-{APP}-{PID}-{THD}-{PAG}-{BLD}-{[yyyy]}";

        // BLD keys off entry assembly
        var expect = $"{AppInfo.AssemblyName}-{AppInfo.AppName}-{AppInfo.Pid}-{LogUtil.ThreadName}-668-Rel-{year}.log";
        Assert.Equal(expect, opts.GetFileName(668));
    }

    [Fact]
    public void GetFileName_AppendsLog()
    {
        var opts = new FileSinkOptions();
        opts.FilePattern = "Test";

        Assert.Equal("Test.log", opts.GetFileName());
    }

    [Fact]
    public void TMPDIR_Exists()
    {
        var opts = new FileSinkOptions();
        opts.DirectoryPattern = FileSinkOptions.TempTag;
        Assert.True(Directory.Exists(opts.GetDirectoryName()));
    }

    [Fact]
    public void DOCDIR_Exists()
    {
        var opts = new FileSinkOptions();
        opts.DirectoryPattern = FileSinkOptions.DocTag;
        Assert.True(Directory.Exists(opts.GetDirectoryName()));
    }

}