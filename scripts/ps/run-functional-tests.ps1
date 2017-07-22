$tsqllint_exe = '.\TSQLLINT_CONSOLE\bin\Release\TSQLLINT_CONSOLE.exe'
& "$tsqllint_exe" @("-i")
& $tsqllint_exe @("-f", ".\TSQLLINT_TESTS\IntegrationTests\TestFiles\integration-test-one.sql")
& $tsqllint_exe @("-f", ".\TSQLLINT_TESTS\IntegrationTests\TestFiles\")
& $tsqllint_exe @("-f", ".\TSQLLINT_TESTS\IntegrationTests\TestFiles\integration-test-one.sql, .\TSQLLINT_TESTS\IntegrationTests\TestFiles\TestFileSubDirectory\integration-test-two.sql")
& $tsqllint_exe @("-h")
& $tsqllint_exe @("-p")
& $tsqllint_exe @("-v")