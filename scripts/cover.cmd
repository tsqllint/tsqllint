mkdir coverage
.\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user -output:.\coverage\coverage_results.xml "-target:.\packages\NUnit.ConsoleRunner.3.6.1\tools\nunit3-console.exe" "-targetargs:.\TSQLLINT_TESTS\bin\Debug\TSQLLINT_LIB_TESTS.dll"
.\packages\ReportGenerator.2.5.8\tools\ReportGenerator.exe "-reports:.\coverage\coverage_results.xml " "-targetdir:.\coverage\report\"
.\packages\Codecov.1.0.1\tools\codecov.exe -f ".\coverage\coverage_results.xml " -t %codecov_token%