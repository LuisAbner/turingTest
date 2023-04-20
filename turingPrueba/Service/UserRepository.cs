using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using turingPrueba.Models;

namespace turingPrueba.Service
{
    public interface IUserRepository
    {
        Task<int> LoginUser(UserLogin user);
        Task<int> Register(User user);
        Task<User?> UserByUserName(String username);
        Task<DataSession?> GetDataSession(long id);
    }
    public class UserRepository : IUserRepository
    {
        private readonly string? _connectionString;
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<DataSession?> GetDataSession(long id)
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = $"call sp_user_select_session(?id);";
                cmd.Parameters.Add("?id", MySqlDbType.Int32).Value = id;
                DataSession dataSession = new DataSession();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    { 
                        dataSession.IdUser=Convert.ToInt32(reader["id_user"].ToString());
                        dataSession.Name=reader["name"].ToString();
                        var email=reader["email"].ToString();
                        if (email != null)
                        {
                            dataSession.Email=email;
                        }                        
                        dataSession.Status=(bool)reader["status"];
                        dataSession.TypeRole=reader["authority"].ToString();
                        dataSession.IdRole=Convert.ToInt32(reader["id_role"].ToString());
                    }
                    reader.Close();
                    return dataSession;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error:" + ex);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        public async Task<int> LoginUser(UserLogin user)
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                conn.Open();
                using var command = new MySqlCommand("call sp_login(?p_email, ?p_password);", conn);
                command.Parameters.AddWithValue("?p_email", user.Email);
                command.Parameters.AddWithValue("?p_password", user.Password);
                return Convert.ToInt32(await command.ExecuteScalarAsync());
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

        public async Task<int> Register(User user)
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                conn.Open();
                using var command = new MySqlCommand("sp_user_insert", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@sp_name", user.Name);
                command.Parameters.AddWithValue("@sp_email", user.Email);
                command.Parameters.AddWithValue("@sp_password", user.Password);
                command.Parameters.AddWithValue("@sp_id_role", user.Role);

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

        public async Task<User?> UserByUserName(string username)
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"SELECT * FROM user WHERE email = ?username;";
                cmd.Parameters.Add("?username", MySqlDbType.VarChar).Value = username;
                User user = new User();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        user.IdUser = Convert.ToInt64(reader["id_user"].ToString());
                        user.Email = reader["emial"].ToString();
                    }
                }
                return user;
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