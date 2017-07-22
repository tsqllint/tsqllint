$tsqllint_exe = '.\TSQLLINT_CONSOLE\bin\Release\TSQLLINT_CONSOLE.exe'
& "$tsqllint_exe" @("-i")
& $tsqllint_exe @("-p", ".\TSQLLINT_TESTS\IntegrationTests\TestFiles\integration-test-one.sql")
& $tsqllint_exe @("-p", ".\TSQLLINT_TESTS\IntegrationTests\TestFiles\")
& $tsqllint_exe @("-p", ".\TSQLLINT_TESTS\IntegrationTests\TestFiles\integration-test-one.sql, .\TSQLLINT_TESTS\IntegrationTests\TestFiles\integration-test-two.sql")
& $tsqllint_exe @("-h")