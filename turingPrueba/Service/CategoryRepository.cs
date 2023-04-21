using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using turingPrueba.Models;

namespace turingPrueba.Service
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>?> getAll();
        Task<int> Add(Category category);
    }
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;
        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> Add(Category category)
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                conn.Open();
                using var command = new MySqlCommand("sp_category_insert", conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@sp_name", category.Name);


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

        public async Task<IEnumerable<Category>?> getAll()
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"SELECT * FROM category;";
                List<Category> categoryList = new List<Category>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Category category = new Category();
                        category.IdCategory = Convert.ToInt32(reader["id_category"]);
                        category.Name = reader["name"].ToString();
                        categoryList.Add(category);
                    }
                    reader.Close();
                }
                return categoryList;
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