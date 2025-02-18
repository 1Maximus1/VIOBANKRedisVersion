using Microsoft.Extensions.Logging;
using VIOBANK.Domain.Models;
using VIOBANK.Domain.Stores;

namespace VIOBANK.Application.Services
{
    public class SettingsService
    {
        private readonly ILogger<SettingsService> _logger;
        private readonly ISettingsStore _settingsStore;

        public SettingsService(ILogger<SettingsService> logger, ISettingsStore settingsStore)
        {
            _logger = logger;
            _settingsStore = settingsStore;
        }

        public async Task<Settings> GetSettingsByUserId(int userId)
        {
            _logger.LogInformation($"Getting settings for a user {userId}");
            return await _settingsStore.GetByUserId(userId);
        }

        public async Task<bool> UpdateSettings(Settings settings)
        {
            try
            {
                _logger.LogInformation($"Updating user settings {settings.UserId}");
                await _settingsStore.Update(settings);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating settings: {ex.Message}");
                return false;
            }
        }
    }
}
