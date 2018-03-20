using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankingSystem.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        int CommitChanges();

        //IRepository<Categories> CategoriesRepository { get; }

        //IRepository<Restaurants> RestaurantsRepository { get; }

        //IRepository<Menu> MenuRepository { get; }

        //IRepository<Orders> OrdersRepository { get; }

        //IRepository<Users> UsersRepository { get; }

        //IRepository<UserTypes> UserTypesRepository { get; }

        //IRepository<MenuDetails> MenuDetailsRepository { get; }
    }
}
