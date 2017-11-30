DECLARE @nv nvarchar(100),
		@vc varchar(100),
		@nc nchar(100),
		@c char(100),
		@sy sysname;

SELECT 'Not Unicode',
	   N'No special characters';

SELECT N'I am テスト in Japanese',
	   N'das Mädchen',
	   N'Los exámenes',
	   N'Le tréma';

SELECT @vc = 'Not Unicode',
	   @c = 'Not Unicode';

SELECT @nv = N'I am テスト in Japanese',
	   @nc = N'I am テスト in Japanese',
       @sy = N'I am テスト in Japanese';
