using Grpc.Core;
using GrpcServiceProvider;
using System.Windows;

namespace CurrencyConverterUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextBlockWelcome.Text = @"Convert a currency (dollars) from numbers into words.
            The maximum number of dollars is 999 999 999.
            The maximum number of cents is 99.
            The separator between dollars and cents is a ‘,’ (comma)";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using var serviceProvider = new GrpcServiceProvider.GrpcServiceProvider();
            var client = serviceProvider.GetCurrencyConverterClient();
            try
            {
                var reply = client.ToText(new ConvertRequest { Value = TextBoxValue.Text });
                TextBlockResult.Text = reply.Result;
            }
            catch (RpcException ex)
            {
                TextBlockResult.Text = ex.StatusCode == StatusCode.Unavailable
                    ?"Sorry, server is unavailable"
                    : ex.Status.Detail;
            }
        }
    }
}
