using Microsoft.AspNetCore.Mvc;
using PROJECT_BOOK_STORE_GROUP5_PRN222.Services;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _svc;
    public AdminController(IAdminService svc) => _svc = svc;

    [HttpGet("summary")] public async Task<IActionResult> Summary() => Ok(await _svc.GetSummaryAsync());

    [HttpGet("sales-report")]
    public async Task<IActionResult> SalesReport([FromQuery] string type = "day", [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        => Ok(await _svc.GetSalesReportAsync(type, from, to));

    [HttpGet("top-customers")]
    public async Task<IActionResult> TopCustomers([FromQuery] int top = 10)
        => Ok(await _svc.GetTopCustomersAsync(top));

    [HttpGet("top-books")]
    public async Task<IActionResult> TopBooks([FromQuery] int top = 10)
        => Ok(await _svc.GetTopBooksAsync(top));

    [HttpGet("deleted-items")]
    public async Task<IActionResult> DeletedItems()
        => Ok(await _svc.GetDeletedItemsAsync());

    [HttpPatch("restore/{table}/{id:long}")]
    public async Task<IActionResult> Restore(string table, long id)
        => await _svc.RestoreAsync(table, id) ? Ok(new { message = "Restored", table, id }) : BadRequest("Restore failed");

    [HttpPatch("restore-user/{userId}")]
    public async Task<IActionResult> RestoreUser(string userId)
        => await _svc.RestoreUserAsync(userId) ? Ok(new { message = "User restored", userId }) : NotFound();
}
