using Azure;
using Azure.AI.TextAnalytics;

namespace PiiRedactionExample
{
    class Program
    {
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential("your-text-analytics-key");	
        private static readonly Uri endpoint = new Uri("https://your-resource.cognitiveservices.azure.com/");
        private static readonly double redactionScoreThreshold = 0.85;

        static async Task Main(string[] args)
        {
            var client = new TextAnalyticsClient(endpoint, credentials);
            string document = "My phone number is 555-123-4567 and my email is example@example.com.";
            var documents = new List<string> { document };
            var response = await client.RecognizePiiEntitiesBatchAsync(documents);

            foreach (var documentResult in response.Value)
            {
                Console.WriteLine($"Redacted Text: {RedactPiiEntities(document, documentResult.Entities, redactionScoreThreshold)}");
            }
        }

        static string RedactPiiEntities(string text, IReadOnlyCollection<PiiEntity> entities, double redactionThreshold)
        {
            foreach (var entity in entities)
            {
                Console.WriteLine($"Entity: {entity.Text}");
                Console.WriteLine($"Category: {entity.Category}");
                Console.WriteLine($"Score: {entity.ConfidenceScore}");
                Console.WriteLine("--------------------------");   
                if (entity.ConfidenceScore >= redactionThreshold)
                {
                    text = text.Replace(entity.Text, new string('*', entity.Text.Length));
                }             
            }
            return text;
        }
    }
}