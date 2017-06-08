using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jerrod.RabbitCommon.Messages
{
    public class GetItemByDescriptionResult
    {
        public int TotalItemsCount { get; set; }
        public List<Item> Items { get; set; }
        public List<Categoryrefiner> CategoryRefiners { get; set; }
    }

    public class Item
    {
        public Brand brand { get; set; }
        public Builderpricedetails builderPriceDetails { get; set; }
        public Configurationdetails configurationDetails { get; set; }
        public string legalText { get; set; }
        public Promotionaldetails promotionalDetails { get; set; }
        public Shippingdetails shippingDetails { get; set; }
        public string title { get; set; }
        public string variantCode { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string configurationType { get; set; }
        public string associatedConfigurationId { get; set; }
        public List<Associatedmedia> associatedMedia { get; set; }
        public string shortDescription { get; set; }
        public string longDescription { get; set; }
        public Leadtimeinformation leadTimeInformation { get; set; }
        public Stockinformation stockInformation { get; set; }
        public string manufacturer { get; set; }
        public Pricing pricing { get; set; }
        public List<Discount> discounts { get; set; }
        public bool? isDownloadable { get; set; }
        public bool? isProductAvailable { get; set; }
        public bool? isConfigurable { get; set; }
    }

    public class Brand
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Builderpricedetails
    {
        public string blurb { get; set; }
        public string text { get; set; }
    }

    public class Configurationdetails
    {
        public string blurb { get; set; }
        public string text { get; set; }
    }

    public class Promotionaldetails
    {
        public string campaignId { get; set; }
        public string legalDescription { get; set; }
        public string shortDescription { get; set; }
        public string longDescription { get; set; }
        public DateTime? expirationDate { get; set; }
    }

    public class Shippingdetails
    {
        public string _class { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public bool? isRequired { get; set; }
        public bool? isSuccessful { get; set; }
    }

    public class Leadtimeinformation
    {
        public int? days { get; set; }
        public string description { get; set; }
        public DateTime? estimatedShipDate { get; set; }
    }

    public class Stockinformation
    {
        public int? quantity { get; set; }
        public string status { get; set; }
        public bool? isInStock { get; set; }
    }

    public class Pricing
    {
        public decimal? sellingPrice { get; set; }
        public decimal? costPrice { get; set; }
        public Listprice listPrice { get; set; }
        public decimal? totalPrice { get; set; }
        public decimal? marginPercent { get; set; }
        public decimal? adjustedListPrice { get; set; }
        public decimal? discountValue { get; set; }
        public decimal? dncDiscountValue { get; set; }
        public decimal? dncDiscountPercent { get; set; }
        public decimal? additionalDiscountValue { get; set; }
        public decimal? additionalDiscountPercent { get; set; }
        public decimal? discountPercent { get; set; }
        public decimal? totalSavings { get; set; }
        public decimal? totalMarginValue { get; set; }
        public decimal? tax { get; set; }
        public Saleprice salePrice { get; set; }
        public Retailprice retailPrice { get; set; }
        public decimal? deltaPrice { get; set; }
        public string shortDescription { get; set; }
        public string longDescription { get; set; }
        public string contractCode { get; set; }
        public decimal? contractPrice { get; set; }
        public decimal? contractDiscountPercent { get; set; }
    }

    public class Listprice
    {
        public string countryCode { get; set; }
        public float? value { get; set; }
    }

    public class Saleprice
    {
        public string countryCode { get; set; }
        public float? value { get; set; }
    }

    public class Retailprice
    {
        public string countryCode { get; set; }
        public float? value { get; set; }
    }

    public class Associatedmedia
    {
        public string url { get; set; }
        public string type { get; set; }
        public bool? isPrimary { get; set; }
    }

    public class Discount
    {
        public Amount amount { get; set; }
        public decimal? percentage { get; set; }
        public decimal? discountAmount { get; set; }
        public bool? isPercentage { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string longDescription { get; set; }
        public string shortDescription { get; set; }
        public string expirationDescription { get; set; }
        public string offerType { get; set; }
        public string promotionType { get; set; }
        public decimal? tax { get; set; }
    }

    public class Amount
    {
        public string countryCode { get; set; }
        public float? value { get; set; }
    }

    public class Categoryrefiner
    {
        public string label { get; set; }
        public string path { get; set; }
        public int? resultCount { get; set; }
    }


}