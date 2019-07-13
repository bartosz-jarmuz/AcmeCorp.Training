using AcmeCorp.Training.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Tests
{
   
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestV1()
        {
            var provider = new LegacyObjectMetadataProvider.V1();
            ValidateProvider(provider.ProvideMetadata);
        }

        [Test]
        public void TestV2()
        {
            var provider = new LegacyObjectMetadataProvider.V2();
            ValidateProvider(provider.ProvideMetadata);
        }

        [Test]
        public void TestV3()
        {
            var provider = new LegacyObjectMetadataProvider.V3();
            ValidateProvider(provider.ProvideMetadata);
        }

        [Test]
        public void TestV4()
        {
            var provider = new LegacyObjectMetadataProvider.V4();
            ValidateProvider(provider.ProvideMetadata);
        }

        [Test]
        public void TestV5()
        {
            var provider = new LegacyObjectMetadataProvider.V5();
            ValidateProvider(provider.ProvideMetadata);
        }
        [Test]
        public void TestV6()
        {
            var provider = new LegacyObjectMetadataProvider.V6();
            ValidateProvider(provider.ProvideMetadata);
        }
        [Test]
        public void TestV7()
        {
            var provider = new LegacyObjectMetadataProvider.V7();
            ValidateProvider(provider.ProvideMetadata);
        }
        [Test]
        public void TestLatest()
        {
            var provider = new LegacyObjectMetadataProvider.LatestVersionProvider();
            ValidateProvider(provider.ProvideMetadata);
        }

        delegate string GetMetadataDelegate (out string properCode);

        private void ValidateProvider(GetMetadataDelegate getMetadata)
        {
            for (int i = 0; i < 100000; i++)
            {
                var metadata = getMetadata(out string properCode);
                var recognizedCode = GetCodeFromMetadata(metadata);
                Assert.AreEqual(properCode, recognizedCode, $"metadata was : {metadata}");
            }
        }

        private string GetCodeFromMetadata(string metadata)
        {
            try
            {
                if (metadata.StartsWith("<"))
                {

                    XElement ele = XElement.Parse(metadata);
                    if (ele != null)
                    {
                        if (ele.Element("Code") != null)
                        {
                            var code = ele.Element("Code").Value;
                            if (ele.Element("Market") != null)
                            {
                                if (ele.Element("Market").Value == "PL" || ele.Element("Market").Value == "BG" || ele.Element("Market").Value == "EL")
                                {
                                    return code.Remove(code.Length - 2);
                                }
                                return code;
                            }
                        }
                    }
                }
            }
            catch
            {
                //ok, probably not XML after all
            }

            try
            {
                if (metadata.StartsWith("{"))
                {
                    dynamic jsonObject = JsonConvert.DeserializeObject(metadata);
                    string meta = jsonObject.Metadata;
                    var matches = Regex.Matches(metadata, @"~[\w]{3,5}~");
                    return matches[0].Value.Trim('~');
                }
            }
            catch
            {
                //ok, probably not XML after all
            }

            MatchCollection versionWithCodeMatches = Regex.Matches(metadata, @"_v[\d]~[\w]{3,5}~");
            if (versionWithCodeMatches.Count > 0)
            {
                var codeMatch = Regex.Match(versionWithCodeMatches[0].Value, @"~[\w]{3,5}~");
                return codeMatch.Value.Trim('~');
            }

            MatchCollection versionMatches = Regex.Matches(metadata, @"v[\d]~");
            if (versionMatches.Count > 0)
            {
                var version = int.Parse(versionMatches[0].Value.Replace("v", "").Replace("~", ""));
                if (version < 6)
                {
                    var matches = Regex.Matches(metadata, @"~[\w]{3,5}~");
                    return matches[matches.Count - 1].Value.Trim(new char[] { '_', '~' });
                }
                else
                {
                    var matches = Regex.Matches(metadata, @"_[\w]{3,5}~");
                    return matches[matches.Count - 1].Value.Trim(new char[] { '_', '~' });

                }
            }
            else
            {
                var matches = Regex.Matches(metadata, @"_[\w]{3,5}~");
                if (matches.Count > 2)
                {
                    return matches[matches.Count - 2].Value.Trim(new char[] { '_', '~' });
                }
                else
                {
                    return matches[matches.Count - 1].Value.Trim(new char[] { '_', '~' });
                }
            }
          
        }
    }
}