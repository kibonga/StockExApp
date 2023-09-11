// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using StockExClient;

string api_key = "HBLX1OS0SXXL3MFV";

var grpc_channel_options = new GrpcChannelOptions {
    HttpClient = new HttpClient(new HttpClientHandler {
        ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
    })
};

using var channel = GrpcChannel.ForAddress("https://localhost:7017", grpc_channel_options);

var client = new Stock.StockClient(channel);

var req = new StockPriceRequest {
    ApiKey = api_key,
    Symbol = "ACXP"
};

using var call = client.GetStockPrice(req);

await foreach(var response in call.ResponseStream.ReadAllAsync()) {
    Console.WriteLine("Received stock price data: \n" + response.Data);
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
