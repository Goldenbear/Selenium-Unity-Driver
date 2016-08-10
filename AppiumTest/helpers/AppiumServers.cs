//--------------------------------------------------------------------------
//  <copyright file="AppiumServers.cs">
//      Copyright (c) Andrea Tino. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace AppiumTests.Helpers
{
    using System;

    /// <summary>
    /// Here you should specify all servers you might use.
    /// </summary>
    /// <remarks>
    /// Machine aliases work fine.
    /// Only change the host name, leave port and the left part of the address as it is.
    /// </remarks>
    public static class TestServers
    {
        public static string Server1 { get { return "http://127.0.0.1:4723/wd/hub"; } }
        public static string Server2 { get { return "http://192.168.1.13:4723/wd/hub"; } }
        public static string Server3 { get { return "http://192.168.2.36:3432/wd/hub"; } }
        public static string Server4 { get { return "http://192.168.2.36:3436/wd/hub"; } }
        public static string Server5 { get { return "http://192.168.2.38:3445/wd/hub"; } }
        public static string Server6 { get { return "http://192.168.2.39:3445/wd/hub"; } }
    }
}
