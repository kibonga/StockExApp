using Grpc.Core;
using StockExServer;

public class StockPriceService : Stock.StockBase {
    public override async Task GetStockPrice(StockPriceRequest request, IServerStreamWriter<StockPriceResponse> responseStream, ServerCallContext callContext) {
        string apiKey = request.ApiKey;
        string symbol = request.Symbol;

        using var client = new HttpClient();
        string baseUrl = "https://www.alphavantage.co/query";
        string requestUrl = $"{baseUrl}?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=1min&apikey={apiKey}";

        try {
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            if(!response.IsSuccessStatusCode) {
                throw new HttpRequestException("Failed to retreive response from stock api " + response.Content);
            }

            Stream stream = await response.Content.ReadAsStreamAsync();
            using StreamReader reader = new(stream);

            char[] buffer = new char[1024];
            int bytesRead;

            while((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                string data = new(buffer, 0, bytesRead);
                StockPriceResponse stockPriceResponse = new() {
                    Data = data
                };
                await responseStream.WriteAsync(stockPriceResponse);
            }
        } catch (Exception ex) {
            Console.WriteLine("Http request failed: " + ex.Message);
        }
    }
} 