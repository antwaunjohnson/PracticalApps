using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Packt.Shared;

namespace Northwind.Web.Pages;

public class CustomersModel : PageModel
{
    private NorthwindContext db;

    public CustomersModel(NorthwindContext injectContext)
    {
        db = injectContext;
    }

    public Customer[] Customers { get; set; } = null!;

    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Customers";
        Customers = db.Customers.OrderBy(c => c.Country).ToArray();
    }
}
