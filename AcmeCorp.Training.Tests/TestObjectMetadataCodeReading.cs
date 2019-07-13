using AcmeCorp.Training.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
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

        [Test]
        public void TestLatestExplicitMax()
        {
            var provider = new LegacyObjectMetadataProvider.LatestVersionProvider();
            var metadata = provider.ProvideMetadata(LegacyObjectMetadataProvider.ApiVersion.V7);
        }

        [Test]
        public void TestError()
        {
            var provider = new LegacyObjectMetadataProvider.LatestVersionProvider();
            var validator = new ObjectCodeValidator();

            var metadata = provider.ProvideMetadata(out string properCode);
            Assert.That(() => validator.AssertCodeIsValid("AS", metadata),
                Throws.Exception.TypeOf<InvalidOperationException>().With.Message.Contains($"The code [AS] is not valid. Expected [{properCode}]!"));
        }

        delegate string GetMetadataDelegate (out string properCode);

        private void ValidateProvider(GetMetadataDelegate getMetadata)
        {
            var validator = new ObjectCodeValidator();
            for (int i = 0; i < 100000; i++)
            {
                var metadata = getMetadata(out string properCode);
                var recognizedCode = validator.GetCodeFromMetadata(metadata);
                Assert.AreEqual(properCode, recognizedCode, $"metadata was : {metadata}");
                validator.AssertCodeIsValid(recognizedCode, metadata);
            }
        }

       
    }
}