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

public class FacilityIdExtensionTest
{
    [Fact]
    public void ToKeyword_CorrectMappings()
    {
        Assert.Equal("user", FacilityId.User.ToKeyword());
        Assert.Equal("mail", FacilityId.Mail.ToKeyword());
        Assert.Equal("daemon", FacilityId.Daemon.ToKeyword());
        Assert.Equal("auth", FacilityId.Auth.ToKeyword());
        Assert.Equal("syslog", FacilityId.Syslog.ToKeyword());
        Assert.Equal("lpr", FacilityId.Lpr.ToKeyword());
        Assert.Equal("news", FacilityId.News.ToKeyword());
        Assert.Equal("uucp", FacilityId.Uucp.ToKeyword());
        Assert.Equal("cron", FacilityId.Cron.ToKeyword());
        Assert.Equal("authpriv", FacilityId.AuthPriv.ToKeyword());
        Assert.Equal("ftp", FacilityId.Ftp.ToKeyword());
        Assert.Equal("ntp", FacilityId.Ntp.ToKeyword());
        Assert.Equal("auth", FacilityId.LogAudit.ToKeyword());
        Assert.Equal("authpriv", FacilityId.LogAlert.ToKeyword());
        Assert.Equal("cron", FacilityId.Clock.ToKeyword());
        Assert.Equal("local0", FacilityId.Local0.ToKeyword());
        Assert.Equal("local1", FacilityId.Local1.ToKeyword());
        Assert.Equal("local2", FacilityId.Local2.ToKeyword());
        Assert.Equal("local3", FacilityId.Local3.ToKeyword());
        Assert.Equal("local4", FacilityId.Local4.ToKeyword());
        Assert.Equal("local5", FacilityId.Local5.ToKeyword());
        Assert.Equal("local6", FacilityId.Local6.ToKeyword());
        Assert.Equal("local7", FacilityId.Local7.ToKeyword());
    }
}