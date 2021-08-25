dotnet pack ./source/TSQLLint.sln \
 --framework .NET5 \
 --output artifacts \
 --configuration Release

dotnet tool uninstall --global TSQLLint.Console

dotnet tool install --global --add-source ./artifacts TSQLLint.Console
