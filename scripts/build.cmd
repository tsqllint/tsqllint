REM install nuget packages
nuget restore "TSQLLINT.sln"

REM build release
MsBuild.exe "TSQLLINT.sln" /t:Clean;Build /p:Configuration=Release /p:TargetFramework=v4.5.2