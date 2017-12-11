dotnet publish ./TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.0 -r win-x86   -o ../lib/win-x86
dotnet publish ./TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.0 -r win-x64   -o ../lib/win-x64
dotnet publish ./TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.0 -r osx-x64   -o ../lib/osx-x64
dotnet publish ./TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.0 -r linux-x64 -o ../lib/linux-x64
