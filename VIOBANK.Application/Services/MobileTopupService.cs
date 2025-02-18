using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.Application.Services
{
    public class MobileTopupService
    {
        private readonly ILogger<MobileTopupService> _logger;
        private readonly IMobileTopupStore _mobileTopupStore;

        public MobileTopupService(ILogger<MobileTopupService> logger, IMobileTopupStore mobileTopupStore)
        {
            _logger = logger;
            _mobileTopupStore = mobileTopupStore;
        }

        public async Task<IReadOnlyList<MobileTopup>> GetTopupsByUserId(int userId)
        {
            _logger.LogInformation($"Getting a list of top-ups for a user {userId}");
            return await _mobileTopupStore.GetByUserId(userId);
        }

        public async Task<MobileTopup> GetTopupById(int topupId)
        {
            _logger.LogInformation($"Receiving a top-up with ID: {topupId}");
            return await _mobileTopupStore.GetById(topupId);
        }

        public async Task<bool> AddTopup(MobileTopup topup)
        {
            try
            {
                _logger.LogInformation($"Adding a new top-up for a user {topup.UserId}");
                await _mobileTopupStore.Add(topup);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding top-up: {ex.Message}");
                return false;
            }
        }
    }
}
