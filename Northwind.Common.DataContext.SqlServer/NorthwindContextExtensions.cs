﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Packt.Shared;

public static class NorthwindContextExtensions
{
    /// <summary>
    /// Adds NorthwindContext to the specified IServiceCollection. Use the SqlServer database provider
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString">Set to override teh default</param>
    /// <returns>An IServiceCollection that can be used to add more services</returns>
    /// 

    public static IServiceCollection AddNorthwindContext(
        this IServiceCollection services, string connectionString = "Data Source=.; Initial Catalog=Northwind;"+
        "Integrated Security=true;MultipleActiveResultsets=true;")
    {
        services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(connectionString));
        return services;
    }
}
