using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using turingPrueba.Models;

namespace turingPrueba.Service
{
    public interface IEditorialRepository{
        Task<IEnumerable<Editorial>?> getAll();
        Task<int> Add(Editorial editorial);
    }
    public class EditorialRepository : IEditorialRepository
    {
        private readonly string _connectionString;
        public EditorialRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> Add(Editorial editorial)
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                conn.Open();
                using var command = new MySqlCommand("sp_editorial_insert", conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@sp_name", editorial.Name);


                command.Parameters.Add("@$response", MySqlDbType.Int32);
                command.Parameters["@$response"].Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                int response = (int)command.Parameters["@$response"].Value;

                return response;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error:" + ex);
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }

        public async Task<IEnumerable<Editorial>?> getAll()
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"SELECT * FROM editorial;";
                List<Editorial> editorialList = new List<Editorial>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Editorial editorial = new Editorial();
                        editorial.IdEditorial = Convert.ToInt32(reader["id_editorial"]);
                        editorial.Name = reader["name"].ToString();
                        editorialList.Add(editorial);
                    }
                    reader.Close();
                }
                return editorialList;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error" + ex);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}