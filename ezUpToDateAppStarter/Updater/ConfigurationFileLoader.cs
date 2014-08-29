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
using System.IO;
using System.Xml.Linq;

namespace ezUpToDateAppStarter.Updater
{
    class ConfigurationFileLoader : IConfigurationFileLoader
    {
        #region IConfigurationFileLoader Members

        IConfiguration IConfigurationFileLoader.LoadFrom(string path)
        {
            XDocument doc = XDocument.Load(path);

            Configuration configuration = new Configuration();
            configuration.Name = doc.Root.Element("name").Value;
            configuration.TargetDirectoryName = doc.Root.Element("targetDir").Value;
            configuration.PathToExecute = doc.Root.Element("pathToExecute").Value;
            try
            {
                configuration.Source = GetSourceInstance(doc.Root.Element("source").Value);
            }
            catch (Exception)
            {
                // Ignore source. Don't check for updates, but try to continue afterwards.
            }

            return configuration;
        }

        private FileSystemInfo GetSourceInstance(string sourceFromXml)
        {
            string tmp = Utilities.SubstituteWorkingDirectoryAlias(sourceFromXml);

            /* Try to figure out what kind of object that is by trial-and-error.
             */
            try
            {
                FileInfo file = new FileInfo(tmp);
                if (file.Exists)
                {
                    return file;
                }
            }
            catch { }

            try
            {
                DirectoryInfo directory = new DirectoryInfo(tmp);
                if (directory.Exists)
                {
                    return directory;
                }
            }
            catch { }

            throw new IOException("Invalid source!");
        }

        #endregion
    }
}
