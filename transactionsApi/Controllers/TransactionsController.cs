using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using transactionsApi.Commands;
using transactionsApi.Contracts.Requests;
using transactionsApi.Contracts.Responses;
using transactionsApi.Queries.GetTransactionQuery;
using transactionsApi.Services.Kafka;

namespace transactionsApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController : ApiController
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly IKafkaProducer _producer;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(
        ISender sender,
        IMapper mapper,
        IKafkaProducer producer,
        ILogger<TransactionsController> logger)
    {
        _sender = sender;
        _mapper = mapper;
        _producer = producer;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTransaction(CreateTransactionRequest request)
    {
        _logger.LogInformation("Create transaction request received");
        var command = _mapper.Map<CreateTransactionCommand>(request);
        var response = await _sender.Send(command);
        _logger.LogInformation("Create transaction processed");
        
        if (!response.IsError)
        {
            var message = JsonConvert.SerializeObject(response.Value);
            var sendResult = await _producer.SendMessageAsync(response.Value.AccountId.ToString(), message);
            _logger.LogInformation("Create transaction result published");
        }
        
        return response.Match(
            res => Ok(_mapper.Map<CreateTransactionResponse>(res)),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetTransaction(GetTransactionRequest getTransactionRequest)
    {
        _logger.LogInformation("Get transaction request received");
        var query = _mapper.Map<GetTransactionQuery>(getTransactionRequest);
        var response = await _sender.Send(query);
        _logger.LogInformation("Get transaction processed");
        
        return response.Match(
            res => Ok(_mapper.Map<GetTransactionResponse>(res)),
            errors => Problem(errors)
        );
    }
}