using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using turingPrueba.Models;

namespace turingPrueba.Service
{
    public interface IBookRepository
    {
        Task<int> Save(Book book);
        Task<IEnumerable<Book>?> GetByUSer(long idUser);
        Task<IEnumerable<Category>> GetByCategory();
        Task<int> DeleteById(long idBook);
        Task<Book?> GetById(long idBook);
        Task<int> Update(Book book);
    }
    public class BookRepository : IBookRepository
    {
        private readonly string? _connectionString;
        public BookRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Book?> GetById(long idBook)
        {
            using (MySqlConnection conexion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await conexion.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conexion;
                    cmd = new MySqlCommand("sp_book_select_by_id_book", conexion);
                    cmd.Parameters.AddWithValue("@sp_id_book", idBook);
                    cmd.CommandType = CommandType.StoredProcedure;

                    Book book = new Book();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            book.IdBook = Convert.ToInt32(reader["id_book"]);
                            book.Title = reader["title"].ToString();
                            book.Subtitle = reader["subtitle"] != DBNull.Value ? reader["subtitle"].ToString() : "";
                            book.Pages = Convert.ToInt32(reader["pages"].ToString());
                            book.Isbn = reader["isbn"].ToString();
                            book.CategoryId = reader["category_id"].ToString();
                            book.EditorialId = reader["editorial_id"].ToString();
                        }
                    }
                    return book;

                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error" + ex);
                    return null;
                }
                finally
                {
                    conexion.Close();
                }
            }
        }
        public async Task<IEnumerable<Category>> GetByCategory()
        {
            using (MySqlConnection conexion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await conexion.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conexion;

                    cmd = new MySqlCommand("call sp_book_select_by_category_count();", conexion);                    
                    int fieldCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                    cmd = new MySqlCommand("sp_book_select_by_category", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<Category> categories = new List<Category>();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        Category category= new Category();
                        int counter = 0;
                        int idindex = 0;
                        while (await reader.ReadAsync())
                        {
                            if (counter == 0)
                            {
                                idindex = Convert.ToInt32(reader["id_category"]);
                            }
                            if (idindex != Convert.ToInt32(reader["id_category"]))
                            {
                                categories.Add(category);
                                category = new Category();
                                idindex = Convert.ToInt32(reader["id_category"]);
                            }
                            category.Name=reader["name_category"].ToString();
                            Book book = new Book();
                            book.IdBook = Convert.ToInt32(reader["id_book"]);
                            book.Title = reader["title"].ToString();
                            book.Subtitle = reader["subtitle"] != DBNull.Value ? reader["subtitle"].ToString() : "Sin registro";
                            book.Pages = Convert.ToInt32(reader["pages"].ToString());
                            book.Isbn = reader["isbn"].ToString();                            
                            book.EditorialId = reader["name_editorial"].ToString();
                            book.UserId = reader["name_writer"].ToString();
                            category.books.Add(book);
                            counter++;
                            if (counter == fieldCount)
                            {
                                categories.Add(category);
                            }

                        }
                    }
                    return categories;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error" + ex);
                    return new List<Category>();
                }
                finally
                {
                    conexion.Close();
                }
            }

        }

        public async Task<IEnumerable<Book>?> GetByUSer(long idUser)
        {
            using (MySqlConnection conexion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await conexion.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conexion;
                    cmd = new MySqlCommand("sp_book_select_by_id_user", conexion);
                    cmd.Parameters.AddWithValue("@sp_id_user", idUser);
                    cmd.CommandType = CommandType.StoredProcedure;

                    List<Book> books = new List<Book>();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Book book = new Book();
                            book.IdBook = Convert.ToInt32(reader["id_book"]);
                            book.Title = reader["title"].ToString();
                            book.Subtitle = reader["subtitle"] != DBNull.Value ? reader["subtitle"].ToString() : "sin registro";
                            book.Pages = Convert.ToInt32(reader["pages"].ToString());
                            book.Isbn = reader["isbn"].ToString();
                            book.CategoryId = reader["name_book"].ToString();
                            book.EditorialId = reader["name_editorial"].ToString();
                            books.Add(book);
                        }
                    }
                    return books;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error" + ex);
                    return null;
                }
                finally
                {
                    conexion.Close();
                }
            }
        }
        public async Task<int> DeleteById(long idBook)
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                conn.Open();
                using var command = new MySqlCommand("sp_book_delete", conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@sp_delet_book", idBook);

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

        public async Task<int> Update(Book book)
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                conn.Open();
                using var command = new MySqlCommand("sp_book_update", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@sp_title", book.Title);
                command.Parameters.AddWithValue("@sp_subtitle", book.Subtitle);
                command.Parameters.AddWithValue("@sp_pages", book.Pages);
                command.Parameters.AddWithValue("@sp_isbn", book.Isbn);
                command.Parameters.AddWithValue("@sp_category_id", book.CategoryId);
                command.Parameters.AddWithValue("@sp_editorial_id", book.EditorialId);
                command.Parameters.AddWithValue("@sp_id_book", book.IdBook);

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

        public async Task<int> Save(Book book)
        {
            using var conn = new MySqlConnection(_connectionString);
            try
            {
                conn.Open();
                using var command = new MySqlCommand("sp_book_insert", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@sp_title", book.Title);
                command.Parameters.AddWithValue("@sp_subtitle", book.Subtitle);
                command.Parameters.AddWithValue("@sp_pages", book.Pages);
                command.Parameters.AddWithValue("@sp_isbn", book.Isbn);
                command.Parameters.AddWithValue("@sp_category_id", book.CategoryId);
                command.Parameters.AddWithValue("@sp_editorial_id", book.EditorialId);
                command.Parameters.AddWithValue("@sp_user_id", book.UserId);

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

    }
}