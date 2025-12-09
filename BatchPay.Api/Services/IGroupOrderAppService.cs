using BatchPay.Api.Dtos;

namespace BatchPay.Api.Services
{
    public interface IGroupOrderAppService
    {
        Task<AddOrderFromMerchantResponse?> AddOrderFromMerchantAsync( AddOrderFromMerchantRequest request );
    }
}
