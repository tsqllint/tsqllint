dotnet publish ./source/TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.1 -r   win-x86 -o ../../assemblies/win-x86
dotnet publish ./source/TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.1 -r   win-x64 -o ../../assemblies/win-x64
dotnet publish ./source/TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.1 -r   osx-x64 -o ../../assemblies/osx-x64
dotnet publish ./source/TSQLLint.Console/TSQLLint.Console.csproj -c Release -f netcoreapp2.1 -r linux-x64 -o ../../assemblies/linux-x64
