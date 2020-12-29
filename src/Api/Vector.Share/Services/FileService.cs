using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vector.Share.Configuration;
using Vector.Share.Data.Models;
using Vector.Share.DTO;
using Vector.Share.Repositories;

namespace Vector.Share.Services
{
    public interface IFileService
    {
        Task<UploadedFile> SaveFileAsync(string filename, byte[] data, FileLifetime lifetime, string contentType);
        Task<UploadedFile> GetFileAsync(string identifier);
        Task DeleteFileAsync(string identifier);
        Task DeleteMultipleFilesAsync(string[] identifiers);
    }

    public class FileService : IFileService
    {
        private readonly IOptions<FileServiceConfiguration> _configuration;
        private readonly IUploadedFileRepository _repository;
        private readonly IIdentifierService _identifierService;
        private readonly ILogger<FileService> _logger;

        public FileService(
            IOptions<FileServiceConfiguration>  configuration,
            IUploadedFileRepository repository,
            IIdentifierService identifierService,
            ILogger<FileService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _identifierService = identifierService ?? throw new ArgumentNullException(nameof(identifierService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UploadedFile> SaveFileAsync(string filename, byte[] data, FileLifetime lifetime, string contentType)
        {
            Directory.CreateDirectory(_configuration.Value.FileFolder);
            string identifier = _identifierService.GenerateIdentifier();
            var path = $"{_configuration.Value.FileFolder}/{identifier}-{filename}";
            _logger.LogInformation($"Saving file: {path}");
            await File.WriteAllBytesAsync(path, data);

            var model = new UploadedFile
            {
                Path = path,
                Identifier = identifier,
                Lifetime = lifetime,
                Uploaded = DateTime.UtcNow,
                ContentType = contentType,
                OriginalFilename = filename
            };

            await _repository.AddAsync(model);

            return model;
        }

        public async Task<UploadedFile> GetFileAsync(string identifier)
        {
            UploadedFile file = await _repository.FindAsync(identifier);
            return file ?? throw new FileNotFoundException();
        }

        public async Task DeleteFileAsync(string identifier)
        {
            UploadedFile file = await _repository.FindAsync(identifier);
            if (file == null)
            {
                throw new FileNotFoundException();
            }

            File.Delete(file.Path);
            await _repository.DeleteAsync(file);
        }

        public async Task DeleteMultipleFilesAsync(string[] identifiers)
        {
            UploadedFile[] files = await _repository.FindMultipleAsync(identifiers);
            foreach (UploadedFile file in files)
            {
                File.Delete(file.Path);
            }

            await _repository.DeleteMultipleAsync(files);
        }
    }
}
