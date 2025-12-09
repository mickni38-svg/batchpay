using BatchPay.Api.Dtos;

namespace BatchPay.Api.Services
{
    public sealed class GroupOrderAppService : IGroupOrderAppService
    {
        // Her kan du injecte domain-repositories
        // private readonly IGroupOrderRepository _groupOrderRepository;
        // private readonly IUserRepository _userRepository;
        // private readonly IMerchantRepository _merchantRepository;

        public GroupOrderAppService(/* IGroupOrderRepository groupOrderRepository, ... */)
        {
            // _groupOrderRepository = groupOrderRepository;
            // ...
        }

        public async Task<AddOrderFromMerchantResponse?> AddOrderFromMerchantAsync( AddOrderFromMerchantRequest request )
        {
            // Her ligger al den logik vi snakkede om:
            // - Find merchant
            // - Map userExternalId -> User
            // - Find aktiv GroupOrder
            // - Tilføj ordre til deltager
            // - Gem PaymentTransactionId
            // - Tjek om alle har ordre
            await Task.CompletedTask;

            return new AddOrderFromMerchantResponse
            {
                GroupOrderId = 42,
                ParticipantId = 1337,
                Status = "OrderAdded",
                Message = "Order added to existing group order (dummy implementation).",
                AllParticipantsHaveOrders = false
            };
        }
    }
}
