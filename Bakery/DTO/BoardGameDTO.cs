using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Bakery.Attributes;
using System.Text.Json.Serialization;
using Bakery.Models;

namespace Bakery.DTO
{
    public class OrderDTO //For Query #2
    {
        public string? DeliveryPlace { get; set; }
        //public DateTime DeliveryDate { get; set; }
        public string DeliveryDate { get; set; }
    }
    public class BakedGoodsDTO //For Query #2
    {
        public string? BakedGood { get; set; }
        public int Quantity { get; set; }
    }

    public class PacketDTO // for displaying query #5
    {
        public string? TrackId { get; set; }
        public string? DeliveryPlace { get; set; }
        public string GPSCoordinates { get; set; }
    }

    public class OrderBakingGoodDTO
    {
        //public int OrderId { get; set; }
        public int BakingGoodId { get; set; }
        public int Quantity { get; set; }
    }

    public class BakingGoodDTO
    {
        public int BakingGoodId { get; set; }
        public string? BgName { get; set; }
        public DateTime DateProduced { get; set; }
    }

    public class RestDTO<T>
    {
        public List<T> Data { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }

    public class LinkDTO
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }

        public LinkDTO(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }

    public class RequestDTO<T> : IValidatableObject
    {
        [DefaultValue(0)]
        public int PageIndex { get; set; } = 0;

        [DefaultValue(10)]
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        [DefaultValue("Name")]
        public string? SortColumn { get; set; } = "Name";

        [SortOrderValidator]
        [DefaultValue("ASC")]
        public string? SortOrder { get; set; } = "ASC";

        [DefaultValue(null)]
        public string? FilterQuery { get; set; } = null;

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            var validator = new SortColumnValidatorAttribute(typeof(T));
            var result = validator
                .GetValidationResult(SortColumn, validationContext);
            return (result != null)
                ? new[] { result }
                : new ValidationResult[0];
        }
    }
}
