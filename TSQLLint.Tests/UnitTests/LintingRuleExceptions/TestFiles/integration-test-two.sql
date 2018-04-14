SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

/* tsqllint-disable select-star - approved by: Johnny Tables*/

SELECT * FROM dbo.FOO;

/* tsqllint-enable select-star */

SELECT * FROM dbo.FOO;