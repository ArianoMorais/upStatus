using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace UpStatus.Infrastructure.Persistence.Mappings;

public static class MongoMappings
{
    private static bool _registered;
    private static readonly object Lock = new();

    public static void Register()
    {
        lock (Lock)
        {
            if (_registered)
            {
                return;
            }

            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("UpStatusConventions", pack, _ => true);

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            _registered = true;
        }
    }
}
