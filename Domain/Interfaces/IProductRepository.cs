using BookstoreManagementSystem.Domain.Models;
using System.Collections.Generic;
using BookstoreManagementSystem.Domain.Services;

namespace BookstoreManagementSystem.Domain.Interfaces
{
    public interface IProductRepository : IDataBase<Product>
    {
        // M�todos adicionales espec�ficos de Product
    }
}
