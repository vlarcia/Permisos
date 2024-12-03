using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AppRequisitos.Services
{
    public class DeviceAuthService
    {
        private readonly string _connectionString;

        public DeviceAuthService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool IsDeviceAuthorized(string androidId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT COUNT(*) FROM AndroidId WHERE androidid = @AndroidId";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AndroidId", androidId);

                        var count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Registra o muestra el error
                Console.WriteLine($"Error en conexión: {ex.Message}");
                return false;
            }
        }
    }


}
