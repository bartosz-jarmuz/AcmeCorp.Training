using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace AcmeCorp.Training.Services
{
    using static LegacyObjectMetadataProviderHelpers;

    public abstract class LegacyObjectMetadataProvider
    {
       
        public abstract class ProviderBase
        {
            internal abstract string ProvideMetadata(out string properCode);
            public string ProvideMetadata()
            {
                return ProvideMetadata(out _);
            }

        }

        public class V1 : ProviderBase
        {
            internal override string ProvideMetadata(out string properCode)
            {
                properCode = Code();
                return $"{ItemNumber()}~{ItemType()}_{ItemGroup()}~{Market()}_{properCode}~{Guid()}.{Extension()}";
            }

            
        }

        public class V2 : ProviderBase
        {
            internal override string ProvideMetadata(out string properCode)
            {
                properCode = Code();
                return $"{ItemNumber()}~{ItemType()}_{ItemGroup()}~{Market()}_{MarketCode()}_{properCode}~{Guid()}.{Extension()}";
            }
        }

        public class V3 : ProviderBase
        {
            internal override string ProvideMetadata(out string properCode)
            {
                properCode = Code();
                return $"{ItemNumber()}~{ItemType()}_{ItemGroup()}~{Market()}_{MarketCode()}_{properCode}~{Guid()}_{SecondaryCode()}~{WarehouseId()}.{Extension()}";
            }
        }

        public class V4 : ProviderBase
        {
            internal override string ProvideMetadata(out string properCode)
            {
                properCode = Code();
                var version = Version();
                if (version < 6)
                {
                    return $"{ItemNumber()}~{ItemType()}_{ItemGroup()}~{Market()}_{MarketCode()}_v{version}~{properCode}~{Guid()}_{SecondaryCode()}~{WarehouseId()}.{Extension()}";
                }
                else
                {
                    return $"{ItemNumber()}~{ItemType()}_{ItemGroup()}~{Market()}_{MarketCode()}_v{version}~{Guid()}_{properCode}~{WarehouseId()}.{Extension()}";
                }
            }
        }

        public class V5 : ProviderBase
        {
            protected XElement GetElement(out string properCode)
            {
                properCode = Code();
                var xEle = new XElement("Object",
                    new XElement("ItemType", ItemType(),
                    new XElement("Metadata", $"{ItemGroup()}~{Market()}_{MarketCode()}_v{Version()}~{properCode}~{Guid()}_{SecondaryCode()}~{WarehouseId()}")));
                return xEle;
            }
            internal override string ProvideMetadata(out string properCode)
            {
                return GetElement(out properCode).ToString();
            }
        }

        public class V6 : ProviderBase
        {
           
            internal override string ProvideMetadata(out string properCode)
            {
                properCode = Code();

                JObject obj = JObject.FromObject(new
                {
                    @object = new
                    {
                        ItemType = ItemType(),
                        WarehouseId = WarehouseId(),
                        Metadata = $"{ItemGroup()}~{Market()}_{MarketCode()}_v{Version()}~{properCode}~{Guid()}",

                    }
                });
                return obj.ToString();
            }
        }

        public class V7 : ProviderBase
        {
            protected XElement GetElement(out string properCode)
            {
                properCode = Code();
                var market = Market();
                var codeWithAddedVariation = properCode;
                if (market == "PL" || market == "BG" || market == "EL")
                {
                    codeWithAddedVariation += market;
                }
                var version = Version();
                var xEle = new XElement("Object",
                    new XElement("ItemType", ItemType()),
                    new XElement("Code", codeWithAddedVariation),
                    new XElement("Market", market),
                    new XElement("Version", version),
                    new XElement("Metadata", $"{ItemGroup()}~_{MarketCode()}~{Guid()}_{SecondaryCode()}~{WarehouseId()}"));
                return xEle;
            }
            internal override string ProvideMetadata(out string properCode)
            {
                return GetElement(out properCode).ToString();
            }
        }

        public class LatestVersionProvider :ProviderBase
        {
            List<ProviderBase> allProviders = new List<ProviderBase>()
            {
                new V1(),
                new V2(),
                new V3(),
                new V4(),
                new V5(),
                new V6(),
                new V7(),
            };

            internal override string ProvideMetadata(out string properCode)
            {
                return allProviders[random.Next(allProviders.Count)].ProvideMetadata(out properCode);
            }
        }

    }
}
