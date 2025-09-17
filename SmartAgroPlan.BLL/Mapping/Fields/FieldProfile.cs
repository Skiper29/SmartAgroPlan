using AutoMapper;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using SmartAgroPlan.BLL.DTO.Fields.Field;
using SmartAgroPlan.DAL.Entities.Fields;

namespace SmartAgroPlan.BLL.Mapping.Fields;

public class FieldProfile : Profile
{
    public FieldProfile()
    {
        var geoJsonWriter = new GeoJsonWriter();
        var geoJsonReader = new GeoJsonReader();

        CreateMap<Field, FieldDto>()
            .ForMember(dest => dest.BoundaryGeoJson,
                opt => opt.MapFrom(src =>
                    src.Boundary != null
                        ? geoJsonWriter.Write(src.Boundary)
                        : null))
            .ReverseMap()
            .ForMember(dest => dest.Boundary,
                opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.BoundaryGeoJson)
                        ? geoJsonReader.Read<Polygon>(src.BoundaryGeoJson)
                        : null));

        CreateMap<Field, FieldCreateUpdateDto>()
            .ForMember(dest => dest.BoundaryGeoJson,
                opt => opt.MapFrom(src =>
                    src.Boundary != null
                        ? geoJsonWriter.Write(src.Boundary)
                        : null))
            .ReverseMap()
            .ForMember(dest => dest.Boundary,
                opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.BoundaryGeoJson)
                        ? geoJsonReader.Read<Polygon>(src.BoundaryGeoJson)
                        : null));

        CreateMap<Field, FieldCreateDto>()
            .IncludeBase<Field, FieldCreateUpdateDto>()
            .ReverseMap()
            .IncludeBase<FieldCreateUpdateDto, Field>();

        CreateMap<Field, FieldUpdateDto>()
            .IncludeBase<Field, FieldCreateUpdateDto>()
            .ReverseMap()
            .IncludeBase<FieldCreateUpdateDto, Field>();
    }
}
