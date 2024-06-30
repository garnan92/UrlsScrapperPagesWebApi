
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configuraciones adicionales
    }

    public async Task<List<T>> ExecuteStoredProcedureAsync<T>(string procedureName, SqlParameter[] parameters = null)
    {
        var resultList = new List<T>();

        using (var command = Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            if (command.Connection.State != ConnectionState.Open)
            {
                await command.Connection.OpenAsync();
            }

            // Ejecutar el comando y leer los resultados
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var resultObject = Activator.CreateInstance<T>();
                    var properties = typeof(T).GetProperties();

                    foreach (var property in properties)
                    {
                        var ordinal = reader.GetOrdinal(property.Name);
                        if (ordinal >= 0 && !reader.IsDBNull(ordinal))
                        {
                            var value = reader.GetValue(ordinal);
                            property.SetValue(resultObject, value);
                        }
                    }

                    resultList.Add(resultObject);
                }
            }
        }

        return resultList;
    }
}