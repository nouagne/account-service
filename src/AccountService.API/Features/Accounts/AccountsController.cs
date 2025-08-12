using AccountService.API.Features.Accounts.Create;
using AccountService.API.Features.Accounts.Get;
using AccountService.Application.Contracts.CQRS;
using AccountService.Application.Exceptions;
using AccountService.Application.UseCases.CreateAccount;
using AccountService.Application.UseCases.GetAccount;
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
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await dispatcher.Send(new GetAccountCommand { AccountId = id }, cancellationToken);
            var response = new GetAccountResponse
            {
                AccountId = result.AccountId,
                FirstName = result.FirstName,
                LastName  = result.LastName,
                Email     = result.Email,
                Timezone  = result.Timezone,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
            
            return Ok(response);
        }
        catch (NotFoundException nf)
        {
            logger.LogInformation(nf, "Account not found for id {Id}", id);
            return NotFound(new ProblemDetails
            {
                Title  = "Account not found",
                Detail = nf.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }
}