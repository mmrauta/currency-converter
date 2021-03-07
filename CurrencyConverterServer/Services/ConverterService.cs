using CurrencyConverterServer.Converters;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CurrencyConverterServer
{
    public class ConverterService : Converter.ConverterBase
    {
        private readonly ILogger<ConverterService> _logger;
        private readonly ICurrencyConverter _converter;

        public ConverterService(ILogger<ConverterService> logger, ICurrencyConverter converter)
        {
            _logger = logger;
            _converter = converter;
        }

        public override Task<ConvertReply> ToText(ConvertRequest request, ServerCallContext context)
        {
            try
            {
                var result = _converter.Convert(request.Value);
                return Task.FromResult(new ConvertReply { Result = result });
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e.Message);
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid input provided."));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occured."));
            }
        }
    }
}
