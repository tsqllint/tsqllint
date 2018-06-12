nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.519 OpenCover
nuget install -Verbosity quiet -OutputDirectory packages -Version 3.0.2   ReportGenerator
nuget install -Verbosity quiet -OutputDirectory packages -Version 1.0.1   Codecov

SET CODECOV=.\packages\Codecov.1.0.1\tools\codecov.exe
SET OPENCOVER=.\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe
SET REPORTGENERATOR=.\packages\ReportGenerator.3.0.2\tools\ReportGenerator.exe

REM Remove any pre-existing artifacts
rmdir /Q /S coverage 2> nul
mkdir coverage 2> nul

%OPENCOVER% ^
    -excludebyattribute:*.ExcludeFromCodeCoverage* ^
    -register:user ^
    -output:".\coverage\coverage_results.xml" ^
    -target:"dotnet.exe" ^
    -targetargs:"test .\source\TSQLLint.Tests\TSQLLint.Tests.csproj" ^
    -filter:"+[*TSQLLint*]*" ^
    -oldStyle

%REPORTGENERATOR% ^
    -reports:.\coverage\coverage_results.xml ^
    -targetdir:.\coverage\report\

REM Only send coverage metrics on APPVEYOR builds
IF NOT "%APPVEYOR%" ==  "" (
    %CODECOV% ^
        -f ".\coverage\coverage_results.xml" ^
        -t %codecov_token%
)