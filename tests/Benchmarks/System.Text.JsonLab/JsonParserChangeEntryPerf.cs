// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;
using Benchmarks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace System.Text.JsonLab.Benchmarks
{
    [MemoryDiagnoser]
    public class JsonParserChangeEntryPerf
    {
        // Keep the JsonStrings resource names in sync with TestCaseType enum values.
        public enum TestCaseType
        {
            DepsJsonSignalR,
            DepsJsonWeather,
            DepsJsonWebSockets
        }

        private byte[] _dataUtf8;
        private MemoryStream _stream;
        private StreamReader _reader;

        [ParamsSource(nameof(TestCaseValues))]
        public TestCaseType TestCase;

        public static IEnumerable<TestCaseType> TestCaseValues() => (IEnumerable<TestCaseType>)Enum.GetValues(typeof(TestCaseType));

        [GlobalSetup]
        public void Setup()
        {
            string jsonString = JsonStrings.ResourceManager.GetString(TestCase.ToString());
            _dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            _stream = new MemoryStream(_dataUtf8);
            _reader = new StreamReader(_stream, Encoding.UTF8, false, 1024, true);
        }

        [Benchmark(Baseline = true)]
        public void ChangeEntryPointLibraryNameNewtonsoft()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            TextReader reader = _reader;
            JToken deps;
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                deps = JToken.ReadFrom(jsonReader);
            }

            foreach (JProperty target in deps["targets"])
            {
                JProperty targetLibrary = target.Value.Children<JProperty>().FirstOrDefault();
                if (targetLibrary == null)
                {
                    continue;
                }
                targetLibrary.Remove();
            }

            JProperty library = deps["libraries"].Children<JProperty>().First();
            library.Remove();
        }

        [Benchmark]
        public void ChangeEntryPointLibraryNameJsonLab()
        {
            var parser = new JsonParser(_dataUtf8);
            JsonObject obj = parser.Parse();

            JsonObject targets = obj["targets"];
            if (targets.TryGetChild(out JsonObject child))
            {
                if (child.TryGetChild(out child))
                    obj.Remove(child);
            }

            JsonObject libraries = obj["libraries"];
            if (libraries.TryGetChild(out child))
                obj.Remove(child);
        }
    }
}
