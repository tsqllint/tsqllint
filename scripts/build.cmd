REM install nuget packages
nuget restore "TSQLLint.sln"

REM build release
MsBuild.exe "TSQLLint.sln" /t:Clean;Build /p:Configuration=Release /p:TargetFramework=v4.5.2