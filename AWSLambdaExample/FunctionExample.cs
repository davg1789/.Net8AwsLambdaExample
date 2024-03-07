using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AWSLambdaExample.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambdaExample;

public class FunctionExample
{
    private static string ConnectionString;   

    private static void LoadConfig(string path)
    {
        try
        {            
            var configPath = Path.Combine(path, "appsettings.json");
            var configJson = File.ReadAllText(configPath);
            var config = JsonConvert.DeserializeObject<Config>(configJson);
            ConnectionString = config.ConnectionString;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error: {ex.Message}");
        }
    }


    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            LoadConfig(Directory.GetCurrentDirectory());

            var json = request?.Body;
            var data = JsonConvert.DeserializeObject<Location>(json);
            
            SaveToSQLServer(data);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "Data saved successfully",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };
        }
        catch (Exception ex)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = $"Error: {ex.Message}",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };
        }
    }

    private void SaveToSQLServer(Location data)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            
            var commandText = "INSERT INTO Location (name) VALUES (@name)";
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@name", data.Name);
                command.ExecuteNonQuery();
            }
        }
    }
}
