# MilasLibrary
this is a personal library for Milas

DtoToSql
this class store any dto into sql server  table 
you shoud inject a list of dto to the DtoTosql instance then call StoreDto Method
if there is no  table with that name in database , creat a table with this name
input of StoreDto Method : 
  1.TableName
  2.ConnectionString
  
  Note : if you first create table in database and alter dto you can not recreate this dto with Same name 
