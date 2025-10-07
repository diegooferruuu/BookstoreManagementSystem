using Npgsql;
using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Services;
using BookstoreManagementSystem.Repository;
using System.Collections.Generic;

public class CategoryRepository
{
    private readonly NpgsqlConnection _connection;

    public CategoryRepository()
    {
        _connection = DataBaseConnection.Instance.GetConnection();
    }

    public List<Category> GetAll()
    {
        var categories = new List<Category>();

        using (var cmd = new NpgsqlCommand("SELECT id, nombre FROM categories", _connection))
        {
            _connection.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }
        return categories;
    }
}

