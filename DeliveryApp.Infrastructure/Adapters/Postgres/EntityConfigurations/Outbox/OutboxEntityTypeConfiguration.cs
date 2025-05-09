using DeliveryApp.Infrastructure.Adapters.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.Outbox;

internal class OutboxEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> goodConfiguration)
    {
        goodConfiguration
            .ToTable("outbox");

        goodConfiguration
            .Property(entity => entity.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        goodConfiguration
            .Property(entity => entity.Type)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("type")
            .IsRequired();

        goodConfiguration
            .Property(entity => entity.Content)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("content")
            .IsRequired();

        goodConfiguration
            .Property(entity => entity.OccurredOnUtc)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("occurred_on_utc")
            .IsRequired();

        goodConfiguration
            .Property(entity => entity.ProcessedOnUtc)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("processed_on_utc")
            .IsRequired(false);
    }
}
