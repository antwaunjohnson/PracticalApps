using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Packt.Shared;

namespace Northwind.Web.Pages;
public class SuppliersModel : PageModel
{
    private NorthwindContext db;
    public SuppliersModel(NorthwindContext injectedContext)
    {
        db = injectedContext;
    }

    [BindProperty]
    public Supplier? Supplier { get; set; }
    public IActionResult OnPost()
    {
        if((Supplier is not null) && ModelState.IsValid)
        {
            db.Suppliers.Add(Supplier);
            db.SaveChanges();
            return RedirectToPage("/suppliers");
        }
        else
        {
            return Page();
        }
    }

    public IEnumerable<object>? Suppliers { get; set; }
   
    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Suppliers";
        Suppliers = db.Suppliers.OrderBy(c => c.Country).ThenBy(c => c.CompanyName);
    }
}
