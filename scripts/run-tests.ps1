$tsqllint = ".\TSQLLINT_CONSOLE\bin\Release\TSQLLLINT_CONSOLE.exe"
& tsqllint -i
& tsqllint @("-p", ".\TSQLLINT_TESTS\Integration Tests\HappyPath\Test Files\happy-path-one.sql")
& tsqllint @("-p", ".\TSQLLINT_TESTS\Integration Tests\HappyPath\Test Files\")
& tsqllint @("-p", ".\TSQLLINT_TESTS\Integration Tests\HappyPath\Test Files\happy-path-one.sql, .\TSQLLINT_TESTS\Integration Tests\HappyPath\Test Files\happy-path-two.sql")