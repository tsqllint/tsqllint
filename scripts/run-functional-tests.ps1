$tsqllint_exe = '.\TSQLLINT_CONSOLE\bin\Release\TSQLLINT_CONSOLE.exe'
& "$tsqllint_exe" @("-i")
& $tsqllint_exe @("-p", ".\TSQLLINT_TESTS\Integration Tests\Test Files\integration-test-one.sql")
& $tsqllint_exe @("-p", ".\TSQLLINT_TESTS\Integration Tests\Test Files\")
& $tsqllint_exe @("-p", ".\TSQLLINT_TESTS\Integration Tests\Test Files\integration-test-one.sql, .\TSQLLINT_TESTS\Integration Tests\Test Files\integration-test-two.sql")
& $tsqllint_exe @("-h")