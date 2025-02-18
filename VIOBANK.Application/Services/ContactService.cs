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
    public class ContactService
    {
        private readonly ILogger<ContactService> _logger;
        private readonly IContactStore _contactStore;

        public ContactService(ILogger<ContactService> logger, IContactStore contactStore)
        {
            _logger = logger;
            _contactStore = contactStore;
        }

        public async Task<IReadOnlyList<Contact>> GetContactsByUserId(int userId)
        {
            _logger.LogInformation($"Getting user contacts with ID: {userId}");
            return await _contactStore.GetByUserId(userId);
        }

        public async Task<Contact> GetContactById(int contactId)
        {
            _logger.LogInformation($"Getting contact with ID: {contactId}");
            return await _contactStore.GetById(contactId);
        }

        public async Task<bool> AddContact(Contact contact)
        {
            try
            {
                _logger.LogInformation($"Adding a new contact: {contact.ContactName}");
                await _contactStore.Add(contact);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding contact: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateContact(Contact contact)
        {
            try
            {
                _logger.LogInformation($"Update contact: {contact.ContactId}");
                await _contactStore.Update(contact);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating contact: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteContact(int contactId)
        {
            try
            {
                _logger.LogInformation($"Deleting a contact with ID: {contactId}");
                await _contactStore.Delete(contactId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting contact: {ex.Message}");
                return false;
            }
        }
    }
}

