REM install nuget packages
nuget restore "TSQLLINT.sln"

REM build release
MsBuild.exe "TSQLLINT.sln" /t:Clean;Build /p:Configuration=Release /p:TargetFramework=v4.5.2 

REM run tests againts release
.\packages\NUnit.ConsoleRunner.3.6.1\tools\nunit3-console.exe .\TSQLLINT_TESTS\bin\Release\TSQLLINT_LIB_TESTS.dll /stoponerror