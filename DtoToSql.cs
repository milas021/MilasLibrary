using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MilasLibrary
{
    public class DtoToSql
    {
        List<PropertyInfo> _props;
        private IEnumerable<Object> _dto;       
        public DtoToSql(IEnumerable<Object> dto)
        {
            _dto = dto;
            Type type = dto.First().GetType();
            _props = type.GetProperties().ToList();
        }
        public void StoreDto(string tableName, string conecctionString)
        {
            string commandTxt = SqlCreateTableCommand(tableName);
            WriteToDB(conecctionString, commandTxt);
            commandTxt = SqlInsetCommand(tableName);
            WriteToDB(conecctionString, commandTxt);

        }

        private int WriteToDB(string connectionString, string commandText)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var command = new SqlCommand(commandText, connection);
                    var count = command.ExecuteNonQuery();

                    return count;
                }
                catch (Exception ex)
                {
                    throw ex;
                    
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        private string SqlCreateTableCommand(string TableName)
        {
            string result = "";
            string createTableCmd = $@"IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].[{TableName}]') AND type in (N'U'))

BEGIN
CREATE TABLE [dbo].[{TableName}](";
            StringBuilder sb = new StringBuilder(createTableCmd);
            foreach (var prop in _props)
            {
                sb.Append($"[{prop.Name.ToString()}] {GetSqlType(prop.PropertyType)} ,");
            }

            result = sb.ToString();
            result = result.Substring(0, result.LastIndexOf(","));
            result += ") END";

            return result;
        }

        private string SqlInsetCommand(string TableName)
        {
            List<Type> withQuotation = new List<Type>()
            {
                typeof(string),
                typeof(Guid),

            };
            List<Type> withoutQuotation = new List<Type>()
            {
                typeof(long) ,
                typeof(byte[]),
                typeof(bool),
                typeof(int),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(short),
                typeof(byte),
                

            };
            string result = $"Insert into {TableName} values  ";
            StringBuilder sb = new StringBuilder(result);
            foreach (var dto in _dto)
            {
                sb.Append("(");
                foreach (var p in _props)
                {
                    if (withQuotation.Contains(p.PropertyType ))
                    {
                        sb.Append($"N'{p.GetValue(dto)}',");
                    }
                    else if (withoutQuotation.Contains(p.PropertyType) )
                    {
                        sb.Append($"{p.GetValue(dto)},");

                    }
                    else
                    {
                        throw new Exception($"Data type {p.GetType().ToString()} not supported");
                    }

                }
                result = sb.ToString();
                result = result.Substring(0, result.LastIndexOf(","));
                result += ")";
                result += ",";
                sb = new StringBuilder(result);

            }
            result = sb.ToString();
            result = result.Substring(0, result.LastIndexOf(","));





            return result;
        }
        private string GetSqlType(Type type)
        {
            var sqlType = string.Empty;
            if (type == typeof(long))
                sqlType = "bigint";
            else if (type == typeof(byte[]))
                sqlType = "varbinary(max)";
            else if (type == typeof(bool))
                sqlType = "bit";
            else if (type == typeof(string))
                sqlType = "nvarchar(max)";
            else if (type == typeof(int))
                sqlType = "int";
            else if (type == typeof(DateTime))
                sqlType = "datetime2(7)";
            else if (type == typeof(DateTimeOffset))
                sqlType = "datetimeoffset(7)";
            else if (type == typeof(decimal))
                sqlType = "decimal(27,6)";
            else if (type == typeof(double))
                sqlType = "float";
            else if (type == typeof(float))
                sqlType = "real";
            else if (type == typeof(short))
                sqlType = "smallint";
            else if (type == typeof(TimeSpan))
                sqlType = "time(7)";
            else if (type == typeof(byte))
                sqlType = "tinyint";
            else if (type == typeof(Guid))
                sqlType = "uniqueidentifier";
            else if (type.IsEnum)
                sqlType = "int";
            if (string.IsNullOrEmpty(sqlType))
                throw new ArgumentException($"Type {type} cannot be converted to sql type");
            return sqlType;
        }
    }
}
