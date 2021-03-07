using Grpc.Net.Client;
using System;

namespace GrpcServiceProvider
{
    public class GrpcServiceProvider : IDisposable
    {
        private static readonly string CurrencyConverterServerUrl = "https://localhost:5001";
        private Lazy<GrpcChannel> Channel { get; set; }
        private Converter.ConverterClient ConverterClient { get; set; }

        public GrpcServiceProvider()
        {
            this.Channel = new Lazy<GrpcChannel>(GrpcChannel.ForAddress(CurrencyConverterServerUrl));
        }

        public Converter.ConverterClient GetCurrencyConverterClient() =>
            this.ConverterClient ??= new Converter.ConverterClient(this.Channel.Value);

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.Channel.IsValueCreated)
                {
                    this.Channel.Value.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}