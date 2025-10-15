using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Services;

namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface IProductRepository : IDataBase<Product>
    {
        // Métodos adicionales específicos de Product si los necesitas
    }
}
