SELECT name, object_id, type_desc  
FROM sys.objects   
WHERE OBJECTPROPERTY(object_id, N'SchemaId') = SCHEMA_ID(N'Production')  
ORDER BY type_desc, name;

SELECT name, object_id, type_desc  
FROM sys.tables   
WHERE objectproperty(object_id, N'SchemaId') = SCHEMA_ID(N'Production')  
ORDER BY type_desc, name;