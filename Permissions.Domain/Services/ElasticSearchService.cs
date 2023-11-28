using Microsoft.Extensions.Logging;
using Nest;
using Permissions.Domain.Entities;

namespace Permissions.Domain.Services
{
    public interface IElasticSearchService
    {
        Task<bool> IndexPermission(Permission permission);
        Task<bool> UpdatePermission(Permission permission);
        Task<bool> DeletePermission(Permission permission);
        Task<bool> DocumentExistsAsync(Permission permission);
    }

    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticSearchService> _logger;

        public ElasticSearchService(IElasticClient elasticClient, ILogger<ElasticSearchService> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        // Persiste en ELK
        public async Task<bool> IndexPermission(Permission permission)
        {
            var response = await _elasticClient.IndexDocumentAsync(permission);

            if (!response.IsValid)
                _logger.LogError($"Error ELK: {response.OriginalException}");

            return response.IsValid;
        }

        // Actualiza en ELK y si no existe lo inserta
        public async Task<bool> UpdatePermission(Permission updatedPermission)
        {
            var documentExists = await DocumentExistsAsync(updatedPermission);

            if (!documentExists)
            {
                // Si el documento no existe, llamamos a IndexPermission para crearlo
                return await IndexPermission(updatedPermission);
            }
            else
            {
                // Si el documento existe, realizamos la actualización
                var response = await _elasticClient.UpdateAsync<Permission, object>(
                    new DocumentPath<Permission>(updatedPermission.Id),
                    u => u.Doc(updatedPermission)
                );

                return response.IsValid;
            }
        }

        // Elimina en ELK
        public async Task<bool> DeletePermission(Permission permissionToDelete)
        {
            var response = await _elasticClient.DeleteAsync<Permission>(
                new DocumentPath<Permission>(permissionToDelete.Id)
            );

            if (!response.IsValid)
                _logger.LogError($"Error ELK: {response.OriginalException}");

            return response.IsValid;
        }

        // Valida existencia de documento en ELK
        public async Task<bool> DocumentExistsAsync(Permission permission)
        {
            var documentExistsResponse = await _elasticClient.DocumentExistsAsync<Permission>(
                new DocumentPath<Permission>(permission.Id)
            );

            return documentExistsResponse.Exists;
        }
    }
}
