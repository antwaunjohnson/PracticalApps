using Microsoft.AspNetCore.Mvc.RazorPages;
using Packt.Shared;

namespace PacktFeatures.Pages;
public class EmployeesPageModel : PageModel
{
    private NorthwindContext db;
    public EmployeesPageModel(NorthwindContext injectContext)
    {
        db = injectContext;
    }

    public Employee[] Employees { get; set; } = null!;
    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Employees";
        Employees = db.Employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToArray();
    }
}
