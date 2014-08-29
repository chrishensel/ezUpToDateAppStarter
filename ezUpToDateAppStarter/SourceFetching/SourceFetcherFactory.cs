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

using System.Linq;
using System.Collections.Generic;
using ezUpToDateAppStarter.Updater;

namespace ezUpToDateAppStarter.SourceFetching
{
    static class SourceFetcherFactory
    {
        #region Fields

        private static readonly ICollection<ISourceFetcher> _fetchers;

        #endregion

        #region Constructors

        static SourceFetcherFactory()
        {
            _fetchers = new List<ISourceFetcher>();
            _fetchers.Add(new DirectorySourceFetcher());
            _fetchers.Add(new ZipSourceFetcher());
        }

        #endregion

        internal static ISourceFetcher GetSourceFetcherFor(IConfiguration configuration)
        {
            ISourceFetcher match = _fetchers.First(_ => _.CanFetchFrom(configuration.Source));
            return match;
        }
    }
}
