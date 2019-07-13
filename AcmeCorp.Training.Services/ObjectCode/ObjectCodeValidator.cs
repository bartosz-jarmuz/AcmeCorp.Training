using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AcmeCorp.Training.Services
{
    public class ObjectCodeValidator
    {
        public void AssertCodesAreValid(IDictionary<string, string> codesAndMetadata)
        {
            foreach (var item in codesAndMetadata)
            {
                AssertCodeIsValid(item.Key, item.Value);
            }
        }

        public void AssertCodeIsValid(string code, string metadata)
        {
            var actualCode = GetCodeFromMetadata(metadata);
            if (code != actualCode)
            {
                throw new InvalidOperationException($"The code [{code}] is not valid. Expected [{actualCode}]!");
            }
        }
      
        internal string GetCodeFromMetadata(string metadata)
        {
            try
            {
                if (metadata.StartsWith("<"))
                {

                    XElement ele = XElement.Parse(metadata);
                    {
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
