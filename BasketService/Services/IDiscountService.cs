using BasketService.Services.DiscountModel;
using DiscountService.Proto;
using Mapster;

namespace BasketService.Services;

public interface IDiscountService
{
    Task<OperationResult<DiscountInfoModel>> GetDiscountById(string id);
    Task<OperationResult<DiscountInfoModel>> GetDiscountByCode(string code);
}

public class DiscountService : IDiscountService
{
    private readonly DiscountServiceProto.DiscountServiceProtoClient _grpcClient;
    public DiscountService(DiscountServiceProto.DiscountServiceProtoClient grpcClient)
    {
        _grpcClient = grpcClient;
    }
    public async Task<OperationResult<DiscountInfoModel>> GetDiscountById(string id)
    {
        var data = await _grpcClient.GetDiscountByIdAsync(new RequestGetDiscountById() { Id = id });
        var response = new OperationResult<DiscountInfoModel>()
        {
            Data = data.Date.Adapt(new DiscountInfoModel())
        };

        return response;
    }

    public async Task<OperationResult<DiscountInfoModel>> GetDiscountByCode(string code)
    {
        var data = await _grpcClient.GetDiscountByAsync(new RequestGetDiscountBy() { Code = code });

        var response = new OperationResult<DiscountInfoModel>()
        {
            Data = data.Date.Adapt(new DiscountInfoModel())
        };

        return response;
    }
}