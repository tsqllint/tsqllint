using System.IO;

namespace TSQLLint.Infrastructure.Interfaces
{
    public interface ISqlStreamReaderBuilder
    {
        StreamReader CreateReader(Stream sqlFileStream);
    }
}
