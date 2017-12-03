DECLARE @nv nvarchar(100),
		@vc varchar(100),
		@nc nchar(100),
		@c char(100),
		@sy sysname;

SELECT 'I am テスト in Japanese',
       'das Mädchen',
       'Los exámenes',
       'Le tréma';

SELECT @vc = 'I am テスト in Japanese',
	   @c = 'I am テスト in Japanese';