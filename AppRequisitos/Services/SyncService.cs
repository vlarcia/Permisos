using AppRequisitos.DTOs;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AppRequisitos.Services
{
    public class SyncService
    {
        // Centralizo el connectionString aquí
        private readonly string _connectionString = "Server=192.168.1.8;Initial Catalog=db_cumplimiento;User Id=sa;Password=Laurean0";

        // Método para obtener las Fincas desde SQL Server
        public async Task<List<FincasDTO>> GetFincasFromSqlServerAsync()
        {
            var fincas = new List<FincasDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT Id_finca, Cod_finca, Descripcion, Area, Encargado, Proveedor FROM MaestroFinca";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var finca = new FincasDTO
                            {
                                idFinca = reader.GetInt32(0),
                                codFinca = reader.GetInt32(1),
                                descripcion = reader.GetString(2),
                                area = reader.GetDecimal(3),
                                encargado = reader.IsDBNull(4) ? null : reader.GetString(4), // Manejo de nulos
                                proveedor = reader.GetString(5)
                            };
                            fincas.Add(finca);
                        }
                    }
                }
            }

            return fincas;
        }

        // Método para obtener el Checklist desde SQL Server
        public async Task<List<ChecklistDTO>> GetChecklistFromSqlServerAsync()
        {
            var checklist = new List<ChecklistDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                // Modifico la consulta para solo traer los primeros 150 caracteres de Observaciones
                var query = "SELECT Id_requisito, Concepto, Ambito, Norma, Bonsucro, LEFT(Observaciones, 150) as Observaciones FROM Checklist";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new ChecklistDTO
                            {
                                idRequisito = reader.GetInt32(0),
                                descripcion = reader.GetString(1), // Concepto será el campo Descripcion
                                ambito = reader.GetString(2),
                                norma = reader.IsDBNull(3) ? null : reader.GetString(3), // Manejo de nulos
                                bonsucro = reader.IsDBNull(4) ? null : reader.GetString(4), // Manejo de nulos
                                observaciones = reader.IsDBNull(5) ? null : reader.GetString(5) // Manejo de nulos
                            };
                            checklist.Add(item);
                        }
                    }
                }
            }

            return checklist;
        }
    }
}
