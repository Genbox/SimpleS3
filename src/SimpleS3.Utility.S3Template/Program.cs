using System;
using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Utility.S3Template.Enums;

namespace Genbox.SimpleS3.Utility.S3Template
{
    internal static class Program
    {
        private static readonly IDictionary<DataType, (string, string)> Data = new Dictionary<DataType, (string, string)>
        {
            { DataType.Request, ("Network/Requests/%Type%s/", "Request.cs") },
            { DataType.RequestMarshal, ("Internals/Marshallers/Requests/%Type%s/", "RequestMarshal.cs") },
            { DataType.Response, ("Network/Responses/%Type%s/", "Response.cs") },
            { DataType.ResponseMarshal, ("Internals/Marshallers/Responses/%Type%s/", "ResponseMarshal.cs") },
        };

        private static readonly string RequestTemplate = File.ReadAllText("Templates/RequestTemplate.txt");
        private static readonly string RequestMarshalTemplate = File.ReadAllText("Templates/RequestMarshalTemplate.txt");
        private static readonly string ResponseTemplate = File.ReadAllText("Templates/ResponseTemplate.txt");
        private static readonly string ResponseMarshalTemplate = File.ReadAllText("Templates/ResponseMarshalTemplate.txt");

        private static void Main(string[] args)
        {
            //This is here so you can run the utility from Visual Studio
            if (args.Length == 0)
            {
                args = new string[2];
                args[0] = "Api.txt";
                args[1] = "C:/Temp/";
            }

            if (args.Length < 2)
            {
                Console.WriteLine(@"Usage: S3Template.exe api.txt c:\temp\");
                return;
            }

            string apiFile = args[0];
            string output = args[1];

            string[] apiDefinitions = File.ReadAllLines(apiFile);

            foreach (string apiDefinition in apiDefinitions)
            {
                string[] split = apiDefinition.Split(",");

                string apiName = split[0];
                ApiType apiType = Enum.Parse<ApiType>(split[1], true);

                DataType dataType = DataType.Request;
                string template = GetTemplate(dataType, apiType, apiName);
                WriteOutFile(dataType, apiType, apiName, template, output);

                dataType = DataType.RequestMarshal;
                template = GetTemplate(dataType, apiType, apiName);
                WriteOutFile(dataType, apiType, apiName, template, output);

                dataType = DataType.Response;
                template = GetTemplate(dataType, apiType, apiName);
                WriteOutFile(dataType, apiType, apiName, template, output);

                dataType = DataType.ResponseMarshal;
                template = GetTemplate(dataType, apiType, apiName);
                WriteOutFile(dataType, apiType, apiName, template, output);
            }
        }

        private static void WriteOutFile(DataType dataType, ApiType apiType, string apiName, string template, string output)
        {
            (string path, string append) = Data[dataType];
            string outPath = Path.Combine(output, path.Replace("%Type%", apiType.ToString(), StringComparison.Ordinal));

            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);

            File.WriteAllText(outPath + apiName + append, template);
        }

        private static string GetTemplate(DataType dataType, ApiType apiType, string apiName)
        {
            string template;

            switch (dataType)
            {
                case DataType.Request:
                    template = RequestTemplate;
                    break;
                case DataType.RequestMarshal:
                    template = RequestMarshalTemplate;
                    break;
                case DataType.Response:
                    template = ResponseTemplate;
                    break;
                case DataType.ResponseMarshal:
                    template = ResponseMarshalTemplate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
            }

            return template
                   .Replace("%ApiName%", apiName, StringComparison.Ordinal)
                   .Replace("%ApiType%", apiType.ToString(), StringComparison.Ordinal)
                   .Replace("%ApiTypeLower%", apiType.ToString().ToLowerInvariant(), StringComparison.Ordinal);
        }
    }
}