using Mapster;
using Microsoft.AspNetCore.DataProtection;
using transactionsApi.Commands;
using transactionsApi.Contracts.Requests;
using transactionsApi.Contracts.Responses;
using transactionsApi.Contracts.Results;
using transactionsApi.Queries.GetTransactionQuery;

namespace transactionsApi.Common.Mappings;

public class TransactionMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateTransactionRequest, CreateTransactionCommand>()
            .Map(dest => dest.SourceAccountId, src => src.sourceAccountId)
            .Map(dest => dest.TargetAccountId, src => src.targetAccountId)
            .Map(dest => dest.Value, src => src.value)
            .Map(dest => dest.TranferTypeId, src => src.tranferTypeId);
            
        config.NewConfig<CreateTransactionResult, CreateTransactionResponse>()
            .Map(dest => dest.TransactionId, src => src.TransactionId)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"));

        config.NewConfig<GetTransactionRequest, GetTransactionQuery>()
            .Map(dest => dest.Id, src => src.transactionExternalId);

        config.NewConfig<GetTransactionResult, GetTransactionResponse>()
            .Map(dest => dest.TransactionExternalId, src => src.TransactionExternalId)
            .Map(dest => dest.SourceAccountId, src => src.SourceAccountId)
            .Map(dest => dest.TargetAccountId, src => src.TargetAccountId)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"));
    }
}