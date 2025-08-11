using AccountService.API.Features.Accounts.Create;
using AccountService.Application.Contracts.CQRS;
using AccountService.Application.UseCases.CreateAccount;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.API.Features.Accounts;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(ICommandDispatcher dispatcher, ILogger<AccountsController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        try
        {
            var result = await dispatcher.Send(command, cancellationToken);
            var response = new CreateAccountResponse { AccountId = result.AccountId };
            
            return CreatedAtAction(nameof(GetById), new { id = response.AccountId }, response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Email already in use"))
        {
            logger.LogInformation(ex, "Create account conflict for email {Email}", request.Email);
            return Conflict(new ProblemDetails
            {
                Title = "Email already in use",
                Detail = "An account already exists with this email.",
                Status = StatusCodes.Status409Conflict
            });
        }
    }
    
    // Stub pour CreatedAtAction (tu implémenteras plus tard la Query GetById)
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetById(Guid id) => StatusCode(StatusCodes.Status501NotImplemented);
}