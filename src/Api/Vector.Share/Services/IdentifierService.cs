using System;
using Microsoft.Extensions.Options;
using Vector.Share.Configuration;
using Vector.Share.Providers.Random;

namespace Vector.Share.Services
{
    public interface IIdentifierService
    {
        string GenerateIdentifier();
    }

    public class IdentifierService : IIdentifierService
    {
        private readonly IOptions<IdentifierServiceConfiguration> _configuration;
        private readonly IRandomProvider _randomService;

        public IdentifierService(
            IOptions<IdentifierServiceConfiguration> configuration,
            IRandomProvider randomService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
        }

        public string GenerateIdentifier()
        {
            IRandom rng = _randomService.GetRandom();

            var result = new char[_configuration.Value.Length];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = _configuration.Value.Characters[rng.Next(_configuration.Value.Characters.Length)];
            }

            return string.Join("", result);
        }
    }
}
