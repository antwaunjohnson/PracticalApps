using Microsoft.EntityFrameworkCore.ChangeTracking;
using Packt.Shared;
using System.Collections.Concurrent;

namespace Northwind.WebApi.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private static ConcurrentDictionary<string, Customer>? customersCache;

    private NorthwindContext db;

    public CustomerRepository(NorthwindContext injectedContext)
    {
        db = injectedContext;
        if (customersCache is null)
        {
            customersCache = new ConcurrentDictionary<string,Customer>(db.Customers.ToDictionary(c => c.CustomerId));
        }
    }
    public async Task<Customer?> CreateAsync(Customer c)
    {
        // normalize CustomerId into uppercase
        c.CustomerId = c.CustomerId.ToUpper();
        // add to database using EF Core
        EntityEntry<Customer> added = await db.Customers.AddAsync(c);
        int affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            if (customersCache is null) return c;
            return customersCache.AddOrUpdate(c.CustomerId, c, UpdateCache);
        }
        else
        {
            return null;
        }
         
    }

    public Task<IEnumerable<Customer>> RetrieveAllAsync()
    {
        // for performance, get from cache
        return Task.FromResult(customersCache is null
            ? Enumerable.Empty<Customer>() : customersCache.Values);
    }



    public Task<Customer?> RetrieveAsync(string id)
    {
        id = id.ToUpper();
        if(customersCache is null) return null!;
        customersCache.TryGetValue(id, out Customer? c);
        return Task.FromResult(c);
    }

    private Customer UpdateCache(string id, Customer c)
    {
        Customer? old;
        if(customersCache is not null)
        {
            if (customersCache.TryGetValue(id, out old))
            {
                if(customersCache.TryUpdate(id, c, old))
                {
                    return c;
                }
            }
        }
        return null!;
    }
    public async Task<Customer?> UpdateAsync(string id, Customer c)
    {
        id = id.ToUpper();
        db.Customers.Update(c);
        int affected = await db.SaveChangesAsync();
        if(affected == 1)
        {
            return UpdateCache(id, c);
        }
        return null;
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        id = id.ToUpper();
        Customer? c = db.Customers.Find(id);
        if (c is null) return null;
        db.Customers.Remove(c);
        int affected = await db.SaveChangesAsync();
        if(affected == 1)
        {
            if (customersCache is null) return null;
            return customersCache.TryRemove(id, out c);
        }
        else
        {
            return null;
        }
    }
}

