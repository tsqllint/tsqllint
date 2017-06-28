DECLARE some_cursor CURSOR  
    FOR SELECT * FROM FOO  
OPEN some_cursor
FETCH NEXT FROM some_cursor;