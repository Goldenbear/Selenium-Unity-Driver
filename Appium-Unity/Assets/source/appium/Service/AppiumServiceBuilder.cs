﻿//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//See the NOTICE file distributed with this work for additional
//information regarding copyright ownership.
//You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using OpenQA.Selenium.Appium.Service.Exceptions;
using OpenQA.Selenium.Appium.Service.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace OpenQA.Selenium.Appium.Service
{
    /// <summary>
    /// This thing accepts parameters and builds instances of AppiumLocalService
    /// </summary>
    public class AppiumServiceBuilder
    {
        private static readonly string ErrorNodeNotFound = "There is no installed nodes! Please install " +
                                " node via NPM (https://www.npmjs.com/package/appium#using-node-js) or download and " +
                                "install Appium app (http://appium.io/downloads.html)";

        private OptionCollector ServerOptions;
        private FileInfo AppiumJS;
        private string IpAddress = AppiumServiceConstants.DefaultLocalIPAddress;
        private int Port = AppiumServiceConstants.DefaultAppiumPort;
        private TimeSpan StartUpTimeout = new TimeSpan(0, 2, 0);
        private FileInfo NodeJS;
        private IDictionary<string, string> EnvironmentForAProcess;
        private string PathToLogFile;


        private static Process StartSearchingProcess(string file, string arguments)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = file;
            if (!String.IsNullOrEmpty(arguments))
            {
                proc.StartInfo.Arguments = arguments;
            }
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();
            return proc;
        }

        private static string GetTheLastStringFromsOutput(StreamReader processOutput)
        {
            string result = string.Empty;
            while (!processOutput.EndOfStream)
            {
                string current = processOutput.ReadLine();
                if (String.IsNullOrEmpty(current))
                {
                    continue;
                }
                result = current;
            }
            return result;
        }

        private static String ReadErrorStream(Process process)
        {
            string result = string.Empty;
            var errorStream = process.StandardError;

            while (!errorStream.EndOfStream)
            {
                string current = errorStream.ReadLine();
                if (String.IsNullOrEmpty(current))
                {
                    continue;
                }
                result = result + current + "\n";
            }
            return result;
        }

        private static FileInfo GetTempFile(string extension, byte[] bytes)
        {
            Guid guid = Guid.NewGuid();
            string guidStr = guid.ToString();

            string path = Path.ChangeExtension(Path.GetTempFileName(), guidStr + extension);

            File.WriteAllBytes(path, bytes);
            return new FileInfo(path);
        }

        private static void ValidateNodeStructure(FileInfo node)
        {
            string absoluteNodePath = node.FullName;

            if (!node.Exists)
            {
                throw new InvalidServerInstanceException("The invalid appium node " + absoluteNodePath + " has been defined",
                        new IOException("The node " + absoluteNodePath + "doesn't exist"));
            }
        }

        private static string FindAFileInPATH(string shortName)
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            string expandedPath = null;
            if (!string.IsNullOrEmpty(path))
            {
                expandedPath = Environment.ExpandEnvironmentVariables(path);
                if (!Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows) &&
                    !expandedPath.Contains(Path.DirectorySeparatorChar + "usr" + Path.DirectorySeparatorChar +
                    "local" + Path.DirectorySeparatorChar + "bin"))
                {
                    expandedPath = expandedPath + Path.PathSeparator + Path.DirectorySeparatorChar + "usr" + Path.DirectorySeparatorChar +
                    "local" + Path.DirectorySeparatorChar + "bin";
                }

                string[] dirs = expandedPath.Split(Path.PathSeparator);
                foreach (string dir in dirs)
                {
                    if (dir.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                    {
                        string fullPath = Path.Combine(dir, shortName);
                        if (File.Exists(fullPath))
                        {
                            return fullPath;
                        }
                    }
                    else
                    {
                        throw new IOException("Can not parse environmental variable PATH because the directory name \"" + dir + "\" contains invalid characters!");
                    }
                }
            }
            return null;
        }

        private FileInfo InstalledNodeInCurrentFileSystem
        {
            get
            {
                string instancePath;
                Process p = null;

                bool isWindows = Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows);
                byte[] bytes;
                string pathToScript = null;
                if (!isWindows)
                {
                    bytes = Properties.Resources.npm_script_unix;
                    pathToScript = GetTempFile(".sh", bytes).FullName;
                }

                try
                {
                    if (isWindows)
                    {
                        p = StartSearchingProcess(AppiumServiceConstants.CmdExe, "/C npm root -g");
                    }
                    else
                    {
                        p = StartSearchingProcess(AppiumServiceConstants.Bash, "-l " + pathToScript);
                    }
                }
                catch (Exception e)
                {
                    if (p != null)
                    {
                        p.Close();
                    }
                    if (pathToScript != null && File.Exists(pathToScript))
                    {
                        File.Delete(pathToScript);
                    }
                    throw e;
                }

                instancePath = GetTheLastStringFromsOutput(p.StandardOutput);

                try
                {
                    DirectoryInfo defaultAppiumNode;
                    if (String.IsNullOrEmpty(instancePath) || !(defaultAppiumNode = new DirectoryInfo(instancePath + Path.DirectorySeparatorChar +
                            AppiumServiceConstants.AppiumFolder)).Exists)
                    {
                        throw new InvalidServerInstanceException(ErrorNodeNotFound);
                    }

                    FileInfo oldResult;
                    //older appium server
                    if ((oldResult = new FileInfo(defaultAppiumNode.FullName + AppiumServiceConstants.AppiumNodeOldMask)).Exists)
                    {
                        return oldResult;
                    }
                    //appium servers v1.5.x and higher
                    FileInfo newResult;
                    if ((newResult = new FileInfo(defaultAppiumNode.FullName + AppiumServiceConstants.AppiumNodeMask)).Exists)
                    {
                        return newResult;
                    }

                    throw new InvalidServerInstanceException(ErrorNodeNotFound,
                                new IOException("Could not find file neither " + AppiumServiceConstants.AppiumNodeOldMask + " nor " +
                                AppiumServiceConstants.AppiumNodeMask + " in the " +
                                defaultAppiumNode + " directory"));
                }
                finally
                {
                    p.Close();
                    if (pathToScript != null && File.Exists(pathToScript))
                    {
                        File.Delete(pathToScript);
                    }
                }
            }
        }

        private FileInfo DefaultExecutable
        {
            get
            {
                string appiumJS = Environment.GetEnvironmentVariable(AppiumServiceConstants.NodeBinaryPath);
                if (!String.IsNullOrEmpty(appiumJS))
                {
                    FileInfo result = new FileInfo(appiumJS);
                    if (result.Exists)
                    {
                        return result;
                    }
                    else
                    {
                        throw new InvalidNodeJSInstanceException("The defined value " + result.FullName + " of the " +
                            AppiumServiceConstants.NodeBinaryPath + " refers to unexisting file!");
                    }
                }

                string filePath;
                try
                {
                    if (Platform.CurrentPlatform.IsPlatformType(PlatformType.Windows))
                    {
                        filePath = FindAFileInPATH(AppiumServiceConstants.Node + ".exe");
                    }
                    else
                    {
                        filePath = FindAFileInPATH(AppiumServiceConstants.Node);
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidNodeJSInstanceException("Node.js is not installed!", e);
                }

                if (String.IsNullOrEmpty(filePath))
                {

                    String errorMessage = "Couldn't find a path to the default Node.js instance from the PATH environmental variable. It seems Node.js is not " +
                        "installed on this computer. Please check the PATH environmental variable or define the " + AppiumServiceConstants.NodeBinaryPath + " " +
                        "environmental variable value.";
                    throw new InvalidNodeJSInstanceException(errorMessage);
                }
                return new FileInfo(filePath);
            }
        }

        /// <summary>
        /// This method specifies Appium server options
        /// </summary>
        /// <param name="serverOptions">A collection of Appium server options</param>
        /// <returns>Self reference</returns>
        public AppiumServiceBuilder WithArguments(OptionCollector serverOptions)
        {
            this.ServerOptions = serverOptions;
            return this;
        }

        /// <summary>
        /// This method defines the desired Appium server binary
        /// </summary>
        /// <param name="appiumJS">Is a file path to the desired appium.js file</param>
        /// <returns>Self-reference</returns>
        public AppiumServiceBuilder WithAppiumJS(FileInfo appiumJS)
        {
            this.AppiumJS = appiumJS;
            return this;
        }

        /// <summary>
        /// This method defines the IP Address to listen on
        /// </summary>
        /// <param name="ipAddress">the IP Address to listen on</param>
        /// <returns>Self-reference</returns>
        public AppiumServiceBuilder WithIPAddress(string ipAddress)
        {
            this.IpAddress = ipAddress;
            return this;
        }

        /// <summary>
        /// Sets time value for the service starting up
        /// </summary>
        /// <param name="startUpTimeout">a time value for the service starting up</param>
        /// <returns>self-reference</returns>
        public AppiumServiceBuilder WithStartUpTimeOut(TimeSpan startUpTimeout)
        {
            if (startUpTimeout == null)
            {
                throw new ArgumentNullException("A startup timeout should not be NULL");
            }
            this.StartUpTimeout = startUpTimeout;
            return this;
        }

        private void CheckAppiumJS()
        {
            if (AppiumJS != null)
            {
                ValidateNodeStructure(AppiumJS);
                return;
            }

            string appiumJS = Environment.GetEnvironmentVariable(AppiumServiceConstants.AppiumBinaryPath);
            if (!String.IsNullOrEmpty(appiumJS))
            {
                FileInfo node = new FileInfo(appiumJS);
                ValidateNodeStructure(node);
                this.AppiumJS = node;
                return;
            }

            this.AppiumJS = InstalledNodeInCurrentFileSystem;
        }

        /// <summary>
        /// Sets which Node.js the builder will use.
        /// </summary>
        /// <param name="nodeJSExecutable">The executable Node.js to use.</param>
        /// <returns>self-reference</returns>
        public AppiumServiceBuilder UsingDriverExecutable(FileInfo nodeJS)
        {
            if (nodeJS == null)
            {
                throw new ArgumentNullException("The nodeJS parameter should not be NULL");
            }

            if (!nodeJS.Exists)
            {
                throw new ArgumentException("The given nodeJS file doesn't exist. Given path " + nodeJS.FullName);
            }

            this.NodeJS = nodeJS;
            return this;
        }

        /// <summary>
        /// Sets which port the appium server should be started on. A value of 0 indicates that any
        /// free port may be used.
        /// </summary>
        /// <param name="port">The port to use; must be non-negative.</param>
        /// <returns>self-reference</returns>
        public AppiumServiceBuilder UsingPort(int port)
        {
            if (port < 0)
            {
                throw new ArgumentException("The port parameter should not be negative");
            }

            if (port == 0)
            {
                return UsingAnyFreePort();
            }

            this.Port = port;
            return this;
        }

        /// <summary>
        /// Configures the appium server to start on any available port.
        /// </summary>
        /// <returns>self-reference</returns>
        public AppiumServiceBuilder UsingAnyFreePort()
        {
            Socket sock = null;

            try
            {
                sock = new Socket(AddressFamily.InterNetwork,
                         SocketType.Stream, ProtocolType.Tcp);
                sock.Bind(new IPEndPoint(IPAddress.Any, 0));
                this.Port = ((IPEndPoint)sock.LocalEndPoint).Port;
                return this;
            }
            finally
            {
                if (sock != null)
                {
                    sock.Dispose();
                }
            }
        }

        /// <summary>
        /// Defines the environment for the launched appium server.
        /// </summary>
        /// <param name="environment">A dictionary of the environment variables to launch the
        ///     appium server with.</param>
        /// <returns>self-reference</returns>
        public AppiumServiceBuilder WithEnvironment(IDictionary<string, string> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("The environment parameter should not be NULL");
            }

            var keys = environment.Keys;
            foreach (var key in keys)
            {
                if (String.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException("The given environment parameter contains an empty or null key");
                }
            }

            var values = environment.Values;
            foreach (var value in values)
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("The given environment parameter contains an empty or null value");
                }
            }

            this.EnvironmentForAProcess = environment;
            return this;
        }

        /// <summary>
        /// Configures the appium server to write log to the given file.
        /// </summary>
        /// <param name="logFile">A file to write log to.</param>
        /// <returns>self-reference</returns>
        public AppiumServiceBuilder WithLogFile(FileInfo logFile)
        {
            if (logFile == null)
            {
                throw new ArgumentNullException("The logFile parameter should not be NULL");
            }
            this.PathToLogFile = logFile.FullName;
            return this;
        }

        private string Args
        {
            get
            {
                List<string> argList = new List<string>();
                CheckAppiumJS();
                argList.Add(string.Format("\"{0}\"", this.AppiumJS.FullName));
                argList.Add("--port");
                argList.Add(Convert.ToString(this.Port));

                argList.Add("--address");
                argList.Add(IpAddress);

                if (this.PathToLogFile != null)
                {
                    argList.Add("--log");
                    argList.Add(this.PathToLogFile);
                }

                if (this.ServerOptions != null)
                {
                    argList.AddRange(this.ServerOptions.Argiments);
                }

                string result = string.Empty;

                foreach (var value in argList)
                {
                    result = result + value + " ";
                }
                return result.Trim();
            }
        }

        /// <summary>
        /// This method builds an instance of AppiumLocalService using defined parameters
        /// </summary>
        /// <returns>an instance of AppiumLocalService built using defined parameters</returns>
        public AppiumLocalService Build()
        {
            if (NodeJS == null)
            {
                NodeJS = DefaultExecutable;
            }
            return new AppiumLocalService(NodeJS, Args, IPAddress.Parse(this.IpAddress), this.Port, StartUpTimeout);
        }
    }
}
