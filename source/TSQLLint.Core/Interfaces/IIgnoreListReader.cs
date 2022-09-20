using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Core.Interfaces
{
    public interface IIgnoreListReader
    {
        public IEnumerable<string> IgnoreList { get; }

        public bool IsIgnoreListLoaded { get; }

        public void LoadIgnoreList(string path);
    }
}
