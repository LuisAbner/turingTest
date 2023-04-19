using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using turingPrueba.Models;

namespace turingPrueba.Service
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>?> getAll();
    }
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;
        public RoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IEnumerable<Role>?> getAll()
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"SELECT * FROM role;";
                List<Role> roleList = new List<Role>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Role role = new Role();
                        role.IdRole = Convert.ToInt32(reader["id_role"]);
                        role.Authority = reader["authority"].ToString();
                        roleList.Add(role);
                    }
                    reader.Close();
                }
                return roleList;
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