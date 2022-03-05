using Microsoft.AspNetCore.Mvc;
using Packt.Shared;
using Northwind.WebApi.Repositories;


namespace Northwind.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository repo;

    public CustomersController(ICustomerRepository repo)
    {
        this.repo = repo;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
    public async Task<IEnumerable<Customer>> GetCustomers(string? country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return await repo.RetrieveAllAsync();
        }
        else
        {
            return (await repo.RetrieveAllAsync())
                .Where(customer => customer.Country == country);
        }
    }

    [HttpGet("{id}", Name = nameof(GetCustomer))]
    [ProducesResponseType(200, Type = typeof(Customer))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCustomer(string id)
    {
        Customer? c = await repo.RetrieveAsync(id);
        if(c == null)
        {
            return NotFound();
        }
        return Ok(c);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Customer))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Customer c)
    {
        if (c == null)
        {
            return BadRequest();
        }
        Customer? addedCustomer = await repo.CreateAsync(c);
        if (addedCustomer == null)
        {
            return BadRequest("Repository failed to create customer.");
        }
        else
        {
            return CreatedAtRoute( // 201 Created
              routeName: nameof(GetCustomer),
              routeValues: new { id = addedCustomer.CustomerId.ToLower() },
              value: addedCustomer);
        }
    }



    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(string id, [FromBody] Customer c)
    {
        id = id.ToUpper();
        c.CustomerId = c.CustomerId.ToUpper();
        if(c == null || c.CustomerId != id)
        {
            return BadRequest();
        }
        Customer? existing = await repo.RetrieveAsync(id);
        if(existing == null)
        {
            return NotFound();
        }
        await repo.UpdateAsync(id, c);
        return new NoContentResult();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType (404)]
    public async Task<IActionResult> Delete(string id)
    {
        if(id == "bad")
        {
            ProblemDetails problemDetails = new() {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://localhost:5001/customers/failed-to-delete",
                Title = $"Customer Id {id} found but failed to delete.",
                Detail = "More details like Company Name, Country and so on.",
                Instance = HttpContext.Request.Path
            };
            return BadRequest(problemDetails);
        }
        Customer? existing = await repo.RetrieveAsync(id);
        if( existing == null)
        {
            return NotFound();
        }
        bool? deleted = await repo.DeleteAsync(id);
        if(deleted.HasValue && deleted.Value)
        {
            return new NoContentResult();
        }
        else
        {
            return BadRequest($"Customer {id} was found but failed to delete");
        }
    }
}
