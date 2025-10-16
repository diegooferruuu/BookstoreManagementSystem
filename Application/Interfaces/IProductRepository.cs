using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Services;

namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface IProductRepository : IDataBase<Product>
    {
        // M�todos adicionales espec�ficos de Product si los necesitas
    }
}
