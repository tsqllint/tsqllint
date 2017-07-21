$tsqllint_exe = '.\TSQLLINT_CONSOLE\bin\Release\TSQLLINT_CONSOLE.exe'
& "$tsqllint_exe" @("-i")
& $tsqllint_exe @("-p", ".\TSQLLINT_TESTS\Integration Tests\HappyPath\Test Files\happy-path-one.sql")
& $tsqllint_exe @("-p", ".\TSQLLINT_TESTS\Integration Tests\HappyPath\Test Files\")
& $tsqllint_exe @("-p", ".\TSQLLINT_TESTS\Integration Tests\HappyPath\Test Files\happy-path-one.sql, .\TSQLLINT_TESTS\Integration Tests\HappyPath\Test Files\happy-path-two.sql")
& $tsqllint_exe @("-h")