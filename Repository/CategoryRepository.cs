using Npgsql;
using BookstoreManagementSystem.Models;
using BookstoreManagementSystem.Services;
using BookstoreManagementSystem.Repository;
using System.Collections.Generic;

public class CategoryRepository : IDataBase<Category>
{
    private readonly NpgsqlConnection _connection;

    public CategoryRepository()
    {
        _connection = DataBaseConnection.Instance.GetConnection();
    }

    public void Create(Category entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }


    public Category? Read(int id)
    {
        using var cmd = new NpgsqlCommand("SELECT * FROM categories WHERE id = @id", _connection);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Category
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
            };
        }

        return null;
    }

    public void Update(Category entity)
    {
        throw new NotImplementedException();
    }
    public List<Category> GetAll()
    {
        var categories = new List<Category>();

        using (var cmd = new NpgsqlCommand("SELECT id, name FROM categories", _connection))
        {
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

