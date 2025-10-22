using FluentValidation;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using SmartAgroPlan.BLL.DTO.Fields.Field;

namespace SmartAgroPlan.BLL.Validators.Fields.Field;

public class BaseFieldValidator : AbstractValidator<FieldCreateUpdateDto>
{
    public const int MaxNameLength = 100;
    public const int MaxLocationLength = 200;

    public BaseFieldValidator()
    {
        RuleFor(f => f.Name)
            .NotEmpty()
            .WithMessage("Field name is required.")
            .MaximumLength(MaxNameLength)
            .WithMessage($"Field name cannot exceed {MaxNameLength} characters.");

        RuleFor(f => f.Location)
            .NotEmpty()
            .WithMessage("Location is required.")
            .MaximumLength(MaxLocationLength)
            .WithMessage($"Location cannot exceed {MaxLocationLength} characters.");

        RuleFor(f => f.SowingDate)
            .LessThanOrEqualTo(DateTime.Now)
            .When(f => f.SowingDate.HasValue)
            .WithMessage("Sowing date cannot be in the future.");

        RuleFor(f => f.BoundaryGeoJson)
            .NotEmpty()
            .WithMessage("Boundary GeoJSON is required.")
            .Must(BeValidGeoJsonPolygon)
            .WithMessage("Boundary GeoJSON must be a valid GeoJSON Polygon.");

        RuleFor(f => f.FieldType)
            .NotNull()
            .WithMessage("Field type is required.")
            .IsInEnum()
            .WithMessage("Invalid field type.");
    }

    private bool BeValidGeoJsonPolygon(string? geoJson)
    {
        try
        {
            var reader = new GeoJsonReader();
            var geom = reader.Read<Polygon>(geoJson);
            return geom != null;
        }
        catch
        {
            return false;
        }
    }
}
