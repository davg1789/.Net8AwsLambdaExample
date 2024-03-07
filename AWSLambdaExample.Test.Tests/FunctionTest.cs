using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Xunit;

namespace AWSLambdaExample.Test.Tests;

public class FunctionTest
{
    [Fact]
    public void TestFunctionHandler_WithValidRequest()
    {

        // Arrange
        var function = new FunctionExample();
        var request = new APIGatewayProxyRequest
        {
            Body = "{\"Name\": \"Test\"}" 
        };
        var context = new TestLambdaContext();

        // Act
        var response = function.FunctionHandler(request, context);

        // Assert
        Assert.Equal(200, response.StatusCode);
        Assert.Contains("Data saved successfully", response.Body);
    }

    [Fact]
    public void TestFunctionHandler_WithEmptyRequest()
    {
        // Arrange
        var function = new FunctionExample();
        var request = new APIGatewayProxyRequest();
        var context = new TestLambdaContext();

        // Act
        var response = function.FunctionHandler(request, context);

        // Assert
        Assert.Equal(500, response.StatusCode);        
    }
}

