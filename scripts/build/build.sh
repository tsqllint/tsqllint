dotnet publish ./TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.0 -r win-x86   -o ../assemblies/win-x86
dotnet publish ./TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.0 -r win-x64   -o ../assemblies/win-x64
dotnet publish ./TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.0 -r osx-x64   -o ../assemblies/osx-x64
dotnet publish ./TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.0 -r linux-x64 -o ../assemblies/linux-x64
