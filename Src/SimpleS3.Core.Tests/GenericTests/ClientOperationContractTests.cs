using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ClientOperationContractTests
{
    [Theory]
    [InlineData(typeof(IBucketOperations), typeof(IBucketClient))]
    [InlineData(typeof(IObjectOperations), typeof(IObjectClient))]
    [InlineData(typeof(IMultipartOperations), typeof(IMultipartClient))]
    [SuppressMessage("Design", "CA1062", Justification = "The theory supplies non-null interface types.")]
    public void EveryOperationHasMatchingConvenienceMethod(Type operations, Type client)
    {
        MethodInfo[] operationMethods = operations.GetMethods();
        MethodInfo[] clientMethods = client.GetMethods();
        Assert.Equal(operationMethods.Select(method => method.Name).Order(StringComparer.Ordinal), clientMethods.Select(method => method.Name).Order(StringComparer.Ordinal));

        foreach (MethodInfo operation in operationMethods)
        {
            MethodInfo convenience = Assert.Single(clientMethods, method => method.Name == operation.Name);
            Assert.Equal(operation.ReturnType, convenience.ReturnType);

            ParameterInfo[] parameters = operation.GetParameters();
            Assert.Equal(2, parameters.Length);
            Assert.True(typeof(IRequest).IsAssignableFrom(parameters[0].ParameterType));
            Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
            Assert.Contains(convenience.GetParameters(), parameter => parameter.ParameterType == typeof(Action<>).MakeGenericType(parameters[0].ParameterType));
        }
    }
}