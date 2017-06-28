/* read committed isolation level should still be flagged as a violation */
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

/* two statements, but should only produce one error*/
SELECT * FROM FOO;
SELECT * FROM FOO;