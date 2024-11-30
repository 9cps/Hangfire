using Hangfire.Database;
using Hangfire.Models.Dto;
using Hangfire.Models.Entities;
using Hangfire.Repositories.Interface;
using Hangfire.Shared.Helper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Hangfire.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private readonly ILogger<BaseRepository> _logger;
        private readonly ApplicationDbContext _context;

        public BaseRepository(ILogger<BaseRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void CallStoredProcedure(string spName, List<ParameterOfStoredProcedure>? listParams = null)
        {
            string query = $"EXEC {spName} ";
            // Add parameters to the query
            try
            {
                for (int i = 0; i < listParams?.Count; i++)
                {
                    query += $"@{listParams[i].Key} = '{listParams[i].Value}'";

                    if (i < listParams.Count - 1)
                    {
                        query += ", ";
                    }
                }

                _context.Database.ExecuteSqlRaw(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public List<T> CallStoredProcedure<T>(string spName, List<ParameterOfStoredProcedure>? listParams = null) where T : class, new()
        {
            var resultList = new List<T>();

            // สร้างคำสั่ง SQL
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using SqlCommand command = new SqlCommand(spName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;

                // เพิ่มพารามิเตอร์ถ้ามี
                if (listParams != null)
                {
                    foreach (var param in listParams)
                    {
                        command.Parameters.Add(new SqlParameter($"@{param.Key}", param.Value ?? (object)DBNull.Value));
                    }
                }

                // เปิดการเชื่อมต่อกับฐานข้อมูล
                connection.Open();

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        // Cache property info for the type T
                        var properties = typeof(T).GetProperties()
                            .Where(p => p.CanWrite)
                            .ToDictionary(p => p.Name, p => p);

                        // ดึงข้อมูลจาก DataReader
                        while (reader.Read())
                        {
                            var item = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var columnName = reader.GetName(i);

                                if (properties.TryGetValue(columnName, out var propertyInfo) && !reader.IsDBNull(i))
                                {
                                    var value = reader.GetValue(i);

                                    // Check if the property is nullable
                                    if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
                                    {
                                        // Convert to nullable type
                                        var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                                        if (nullableType != null)
                                        {
                                            propertyInfo.SetValue(item, Convert.ChangeType(value, nullableType));
                                        }
                                    }
                                    else
                                    {
                                        propertyInfo.SetValue(item, Convert.ChangeType(value, propertyInfo.PropertyType));
                                    }
                                }
                            }
                            resultList.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
                finally
                {
                    // ปิดการเชื่อมต่อ
                    _context.Database.CloseConnection();
                }
            }

            return resultList;
        }


        public async Task BatchExceptionLog(string stepJob, string remark, string? status = null)
        {
            try
            {
                // Add a new log transaction
                var tblLogTransection = new TblLogTransection
                {
                    TranDate = DateTime.Now,
                    StrepJob = stepJob,
                    Status = status,
                    Remark = remark
                };

                await _context.LogTransections.AddAsync(tblLogTransection);

                // Save changes
                await _context.SaveChangesAsync();
                _context.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
            }
        }
    }
}