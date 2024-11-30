using Hangfire.Database.Attributes;
using Hangfire.Database.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Hangfire.Database.Classes
{
    public class DbDescriptionUpdater<TContext>
           where TContext : DbContext
    {

        public DbDescriptionUpdater(TContext context)
        {
            this.context = context;
        }

        readonly TContext context;
        public void UpdateDatabaseDescriptions()
        {
            Type contextType = typeof(TContext);
            var props = contextType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            context.Database.OpenConnection();
            try
            {
                foreach (var prop in props)
                {
                    try
                    {
                        if (prop.PropertyType.InheritsOrImplements((typeof(DbSet<>))))
                        {
                            var tableType = prop.PropertyType.GetGenericArguments()[0];
                            SetTableDescriptions(tableType);
                        }
                    }
                    catch (Exception)
                    {
                        // silent exception
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            finally
            {
                if (context.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                    context.Database.CloseConnection();
            }
        }

        private void SetTableDescription(string tableName, string description)
        {
            string strGetDesc = "select [value] from fn_listextendedproperty('MS_Description','schema','dbo','table', null, null,null) where objname = N'" + tableName + "';";
            var prevDesc = RunSqlScalar(strGetDesc);
            RunSql(
                DbDescriptionUpdater<TContext>.ExtendedTablePropertyQuery(prevDesc != null),
                new SqlParameter("@table", tableName),
                new SqlParameter("@desc", description));
        }
        private static string ExtendedTablePropertyQuery(bool isUpdate = false)
        {
            string sproc = !isUpdate ? "sp_addextendedproperty" : "sp_updateextendedproperty";
            string query = string.Format(@"EXEC {0}
                    @name = N'MS_Description', 
                    @value = @desc, 
                    @level0type = N'Schema', 
                    @level0name = 'dbo', 
                    @level1type = N'Table',  
                    @level1name = @table, 
                    @level2type = null, 
                    @level2name = null", sproc);
            return query;
        }
        private void SetTableDescriptions(Type tableType)
        {
            string fullTableName = context.Model?.FindEntityType(tableType)?.GetTableName() ?? string.Empty;
            string tableName = ExtractTableName(fullTableName);

            var tableAttrs = tableType.GetCustomAttributes(typeof(TableAttribute), false);
            if (tableAttrs.Length > 0)
                tableName = ((TableAttribute)tableAttrs[0]).Name;

            SetTableMetaAttributes(tableType, tableName);

            foreach (var prop in tableType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
                    continue;

                try
                {
                    var field_desc = "";
                    // first try getting from FieldDescription
                    var fieldDescAttrs = prop.GetCustomAttributes(typeof(FieldDescriptionAttribute), false);
                    if (fieldDescAttrs.Length > 0)
                    {
                        // add meta
                        field_desc = ((FieldDescriptionAttribute)fieldDescAttrs[0]).Description;
                    }

                    // else, get from DisplayAttribute
                    if (field_desc == "")
                    {
                        var displayDescAttrs = prop.GetCustomAttributes(typeof(DisplayAttribute), false);
                        if (displayDescAttrs.Length > 0)
                        {
                            field_desc = ((DisplayAttribute)displayDescAttrs[0]).Name;
                        }
                    }

                    SetColumnDescription(tableName, prop.Name, field_desc ?? string.Empty);
                }
                catch
                {
                    // silent exception
                }
            }
        }
        private static string ExtractTableName(string fullTableName)
        {
            string pattern = @"(\[\w+\]\.)?\[(?<table>.*)\]";
            Match match = Regex.Match(fullTableName, pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
            return match.Success ? match.Groups["table"].Value : fullTableName;
        }

        private void SetTableMetaAttributes(Type tableType, string tableName)
        {
            var tableMetaAttrs = tableType.GetCustomAttributes(typeof(TableDescriptionAttribute), false);
            if (tableMetaAttrs.Length > 0)
            {
                var description = ((TableDescriptionAttribute)tableMetaAttrs[0]).Description;
                SetTableDescription(tableName, description);
            }
        }

        private void SetColumnDescription(string tableName, string columnName, string description)
        {
            if (String.IsNullOrEmpty(description)) return;

            string strGetDesc = "select [value] from fn_listextendedproperty('MS_Description','schema','dbo','table',N'" + tableName + "','column',null) where objname = N'" + columnName + "';";
            var prevDesc = RunSqlScalar(strGetDesc);
            if (prevDesc == null)
            {
                RunSql(@"
                        EXEC sp_addextendedproperty 
                        @name = N'MS_Description', @value = @desc,
                        @level0type = N'Schema', @level0name = 'dbo',
                        @level1type = N'Table',  @level1name = @table,
                        @level2type = N'Column', @level2name = @column;",
                        new SqlParameter("@table", tableName),
                        new SqlParameter("@column", columnName),
                        new SqlParameter("@desc", description));
            }
            else
            {
                RunSql(@"
                        EXEC sp_updateextendedproperty 
                        @name = N'MS_Description', @value = @desc,
                        @level0type = N'Schema', @level0name = 'dbo',
                        @level1type = N'Table',  @level1name = @table,
                        @level2type = N'Column', @level2name = @column;",
                        new SqlParameter("@table", tableName),
                        new SqlParameter("@column", columnName),
                        new SqlParameter("@desc", description));
            }
        }

        DbCommand CreateCommand(string cmdText, params SqlParameter[] parameters)
        {
            var cmd = context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = cmdText;
            foreach (var p in parameters)
                cmd.Parameters.Add(p);
            return cmd;
        }
        void RunSql(string cmdText, params SqlParameter[] parameters)
        {
            var cmd = CreateCommand(cmdText, parameters);
            cmd.ExecuteNonQuery();
        }
        object RunSqlScalar(string cmdText, params SqlParameter[] parameters)
        {
            var cmd = CreateCommand(cmdText, parameters);
            return cmd.ExecuteScalar() ?? new object();
        }


    }
}
