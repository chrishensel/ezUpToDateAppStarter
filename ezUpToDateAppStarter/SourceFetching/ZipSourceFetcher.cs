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

using System.IO;
using Ionic.Zip;

namespace ezUpToDateAppStarter.SourceFetching
{
    class ZipSourceFetcher : ISourceFetcher
    {
        #region ISourceFetcher Members

        bool ISourceFetcher.CanFetchFrom(FileSystemInfo resolvedSource)
        {
            FileInfo file = resolvedSource as FileInfo;
            if (file != null)
            {
                return file.Extension == ".zip";
            }

            return false;
        }

        bool ISourceFetcher.ShouldFetch(DirectoryInfo destinationLocal, FileSystemInfo resolvedSource)
        {
            if (destinationLocal.Exists)
            {
                return destinationLocal.CreationTimeUtc != resolvedSource.LastWriteTimeUtc;
            }

            return true;
        }

        void ISourceFetcher.FetchInto(DirectoryInfo destinationLocal, FileSystemInfo resolvedSource)
        {
            /* - First, copy the ZIP file to the temporary directory.
             * - Second, unzip from there.
             */

            string localZipFileName = CopyZipFileToLocal(destinationLocal, resolvedSource);

            using (ZipFile zip = new ZipFile(localZipFileName))
            {
                foreach (ZipEntry entry in zip)
                {
                    entry.Extract(destinationLocal.FullName);
                }
            }

            destinationLocal.CreationTimeUtc = resolvedSource.LastWriteTimeUtc;

            File.Delete(localZipFileName);
        }

        private string CopyZipFileToLocal(DirectoryInfo destinationLocal, FileSystemInfo resolvedSource)
        {
            string destinationZipFileName = Path.Combine(destinationLocal.FullName, "_tmp.zip");

            File.Copy(resolvedSource.FullName, destinationZipFileName);

            return destinationZipFileName;
        }

        #endregion
    }
}
