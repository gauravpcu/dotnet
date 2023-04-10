using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spbenchmark.data;

namespace spbenchmark.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
   
    private readonly ILogger<UserController> _logger;
    private readonly UserDbContext _context;
    public UserController(UserDbContext context, ILogger<UserController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("")]
    public IEnumerable<CoreUser> Get(int limit)
    {
        var users = _context.CoreUsers.Take(limit);
        return users.ToArray();
    }

    [HttpGet("manipulated")]
    public IEnumerable<CoreUser> GetManipulted(int limit)
    {
        var users = _context.CoreUsers.Take(limit).ToArray();
        for (int i = 0; i < users.Count(); i++)
        {
            users[i].Age += 1;
            users[i].FirstName = users[i].FirstName.ToUpper();
            users[i].LastName = "," + users[i].LastName.ToUpper();
        }
        return users.ToArray();
        
    }

    [HttpPut("")]
    public async Task<IActionResult> Put(CoreUser[] users)
    {
        var validUsers = users.Where(o => o.Id > 0).ToArray();

        if (!(validUsers.Count() > 0))
        {
            return NotFound();
        }
        

        try
        {
            _context.UpdateRange(validUsers);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw;
        }

        return NoContent();
    }

    [HttpPost("")]
    public async Task<ActionResult<CoreUser[]>> Post(CoreUser[] users)
    {
        if (users.Count() == 0)
        {
            return NotFound();
        }

        _context.CoreUsers.AddRange(users);

        await _context.SaveChangesAsync();
        
        return CreatedAtAction("Post", users.ToArray());
    }
}

