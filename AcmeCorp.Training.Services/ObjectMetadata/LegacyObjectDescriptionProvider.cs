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
            public virtual string ProvideMetadata()
            {
                return ProvideMetadata(out _);
            }

        }

        public class V1 : ProviderBase
        {
            /// <summary>
            /// Metadata consists of ItemNumber~ItemType_ItemGroup~Market_ObjectCode~ObjectGuid.Extension
            /// </summary>
            /// <returns></returns>
            public override string ProvideMetadata() => base.ProvideMetadata();
         
            internal override string ProvideMetadata(out string properCode)
            {
                properCode = Code();
                return $"{ItemNumber()}~{ItemType()}_{ItemGroup()}~{Market()}_{properCode}~{Guid()}.{Extension()}";
            }
        }

        public class V2 : ProviderBase
        {
            /// <summary>
            ///  /// <summary>
            /// Like in V1, but added _MarketCode_ after Market
            /// </summary>
            /// <returns></returns>
            public override string ProvideMetadata() => base.ProvideMetadata();

            internal override string ProvideMetadata(out string properCode)
            {
                properCode = Code();
                return $"{ItemNumber()}~{ItemType()}_{ItemGroup()}~{Market()}_{MarketCode()}_{properCode}~{Guid()}.{Extension()}";
            }
        }

        public class V3 : ProviderBase
        {
            /// <summary>
            ///  /// <summary>
            /// Like in V2, but added _SecondaryCode~WarehouseId after Guid
            /// </summary>
            /// <returns></returns>
            public override string ProvideMetadata() => base.ProvideMetadata();
            internal override string ProvideMetadata(out string properCode)
            {
                properCode = Code();
                return $"{ItemNumber()}~{ItemType()}_{ItemGroup()}~{Market()}_{MarketCode()}_{properCode}~{Guid()}_{SecondaryCode()}~{WarehouseId()}.{Extension()}";
            }
        }


        public class V4 : ProviderBase
        {

            /// <summary>
            ///  /// <summary>
            /// Like in V3, but _version~ added after market code.
            /// If version equal or higher than 6, the Object code is moved after object Guid
            /// </summary>
            /// <returns></returns>
            public override string ProvideMetadata() => base.ProvideMetadata();

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

            /// <summary>
            ///  /// <summary>
            /// Returns as XML Element. Parts of metadata are moved to a metadata element.
            /// </summary>
            /// <returns></returns>
            public override string ProvideMetadata() => base.ProvideMetadata();

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
            /// <summary>
            ///  /// <summary>
            /// Returns as Json string. Parts of metadata are moved to a metadata node.
            /// </summary>
            /// <returns></returns>
            public override string ProvideMetadata() => base.ProvideMetadata();
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
            /// <summary>
            ///  /// <summary>
            /// Returns as Json string. Object code is placed in the 'Code' XML element, but if the Market is Polish, Bulgarian or Greek, a Market string is added to the code
            /// </summary>
            /// <returns></returns>
            public override string ProvideMetadata() => base.ProvideMetadata();
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

            /// <summary>
            ///  /// <summary>
            /// Returns a mixture of any previous version providers
            /// </summary>
            /// <returns></returns>
            public override string ProvideMetadata() => base.ProvideMetadata();

            internal override string ProvideMetadata(out string properCode)
            {
                return allProviders[random.Next(allProviders.Count)].ProvideMetadata(out properCode);
            }
        }

    }
}
