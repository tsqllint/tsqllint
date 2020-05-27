using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration;
using TSQLLint.Infrastructure.Interfaces;

namespace TSQLLint.Infrastructure.Parser
{
    public class SqlStreamReaderBuilder : ISqlStreamReaderBuilder
    {
        private static readonly Regex PlaceholderRegex = new Regex(@"\$\((?<placeholder>[^)]+)\)", RegexOptions.Compiled);

        private readonly IEnvironmentWrapper environmentWrapper;

        public SqlStreamReaderBuilder()
            : this(new EnvironmentWrapper()) { }

        public SqlStreamReaderBuilder(IEnvironmentWrapper environmentWrapper)
        {
            this.environmentWrapper = environmentWrapper;
        }

        public StreamReader CreateReader(Stream sqlFileStream)
        {
            var sqlText = new StreamReader(sqlFileStream);
            sqlFileStream.Seek(0, SeekOrigin.Begin);
            var sql = ReplaceSqlPlaceholders(sqlText.ReadToEnd());
            return new StreamReader(new MemoryStream(sqlText.CurrentEncoding.GetBytes(sql)));
        }

        private string ReplaceSqlPlaceholders(string sql)
        {
            var matches = PlaceholderRegex.Matches(sql);

            if (matches.Count == 0)
            {
                return sql;
            }

            var newSql = new StringBuilder();
            var i = 0;

            foreach (Match match in matches)
            {
                var placeholder = match.Groups["placeholder"].Value;
                var replacement = environmentWrapper.GetEnvironmentVariable(placeholder) ?? match.Value;
                newSql.Append(sql.Substring(i, match.Index - i));
                newSql.Append(replacement);
                i = match.Index + match.Length;
            }

            newSql.Append(sql.Substring(i));

            return newSql.ToString();
        }
    }
}
