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
using System.Reflection;

namespace ezUpToDateAppStarter
{
    static class Utilities
    {
        internal const string WorkingDirectoryAlias = "$\\";

        internal static string GetWorkingDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        internal static string GetAbsolutePath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }

            return Path.Combine(GetWorkingDirectory(), path);
        }

        internal static string SubstituteWorkingDirectoryAlias(string value)
        {
            return SubstituteWorkingDirectoryAlias(value, GetWorkingDirectory());
        }

        internal static string SubstituteWorkingDirectoryAlias(string value, string pathToSubstitute)
        {
            string tmp = value;
            if (tmp.StartsWith(WorkingDirectoryAlias))
            {
                tmp = Path.Combine(pathToSubstitute, tmp.Remove(0, WorkingDirectoryAlias.Length));
            }
            return tmp;
        }
    }
}
