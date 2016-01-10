// ezUpToDateAppStarter
// Copyright (C) 2014 Sascha-Christian Hensel
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System;
using System.Diagnostics;
using System.IO;
using ezUpToDateAppStarter.SourceFetching;
using ezUpToDateAppStarter.Updater;

namespace ezUpToDateAppStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteInfo("".PadRight(79, '*'));
            WriteInfo("ezUpToDateAppStarter, Copyright (C) 2016 Sascha-Christian Hensel");
            WriteInfo("ezUpToDateAppStarter comes with ABSOLUTELY NO WARRANTY.");
            WriteInfo("This is free software, and you are welcome to redistribute it");
            WriteInfo("under certain conditions; see LICENSE.");
            WriteInfo("".PadRight(79, '*'));
            WriteInfo("");

            bool shouldPromptUser = true;

            if (args.Length != 1)
            {
                WriteError("No configuration file given!");
            }
            else
            {
                string path = Utilities.GetAbsolutePath(args[0]);

                WriteInfo("Trying to load configuration file '{0}'...", path);

                IConfiguration configuration = LoadConfiguration(path);
                if (configuration != null)
                {
                    try
                    {
                        DirectoryInfo destinationLocal = new DirectoryInfo(GetDestinationLocalDirectoryPath(configuration));

                        if (configuration.Source == null)
                        {
                            WriteWarning("Failed in checking for updates. Possible causes: The location is unavailable or the configuration is invalid.");
                            WriteWarning("Trying to run application nevertheless. If that fails, contact the vendor.");
                        }
                        else
                        {
                            ISourceFetcher sourceFetcher = SourceFetcherFactory.GetSourceFetcherFor(configuration);

                            if (!Properties.Settings.Default.AlwaysFetch && !sourceFetcher.ShouldFetch(destinationLocal, configuration.Source))
                            {
                                WriteInfo("No need to fetch, all is up-to-date.");
                            }
                            else
                            {
                                WriteInfo("Prepare fetching of contents...");

                                DirectoryInfo tmpDir = new DirectoryInfo(GetDestinationLocalDirectoryPath(configuration) + "_tmp");
                                if (tmpDir.Exists)
                                {
                                    WriteInfo("Deleting previously existing temporary directory.");
                                    tmpDir.Delete(true);
                                }

                                WriteInfo("Creating new directory.");

                                tmpDir.Create();

                                WriteInfo("Fetching contents into temporary directory...");

                                sourceFetcher.FetchInto(tmpDir, configuration.Source);

                                if (destinationLocal.Exists)
                                {
                                    destinationLocal.Delete(true);
                                }

                                tmpDir.MoveTo(destinationLocal.FullName);

                                WriteInfo("Fetching of remote contents successful!");
                            }

                            shouldPromptUser = false;
                        }

                        RunPostActions(configuration, destinationLocal);
                    }
                    catch (Exception ex)
                    {
                        WriteError("Failed in executing the action. Error details: {0}", ex.Message);
                    }
                }
            }

            if (shouldPromptUser && Properties.Settings.Default.InteractiveMode)
            {
                WriteInfo("");
                WriteInfo("Press any key to quit . . .");
                Console.ReadKey();
            }
        }

        static IConfiguration LoadConfiguration(string path)
        {
            try
            {
                IConfigurationFileLoader parser = new ConfigurationFileLoader();
                IConfiguration configuration = parser.LoadFrom(path);

                WriteInfo("Loaded configuration: {0}, dir: {1}, source: {2}", configuration.Name, configuration.TargetDirectoryName, configuration.Source);

                return configuration;
            }
            catch (Exception ex)
            {
                WriteError("Failed loading configuration! Error details: {0}", ex.Message);
            }

            return null;
        }

        static void WriteInfo(string format, params object[] args)
        {
            Console.ResetColor();
            Console.WriteLine(format, args);
        }

        static void WriteWarning(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(format, args);
        }

        static void WriteError(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
        }

        static string GetDestinationLocalDirectoryPath(IConfiguration configuration)
        {
            string tmp = (string.IsNullOrWhiteSpace(configuration.TargetDirectoryName)) ? configuration.Name : configuration.TargetDirectoryName;

            return Path.Combine(Utilities.GetWorkingDirectory(), tmp);
        }

        static void RunPostActions(IConfiguration configuration, DirectoryInfo destination)
        {
            if (string.IsNullOrWhiteSpace(configuration.PathToExecute))
            {
                return;
            }

            string pathToExecute = Path.Combine(Utilities.SubstituteWorkingDirectoryAlias(configuration.PathToExecute, destination.FullName));

            Process.Start(pathToExecute);
        }
    }
}
