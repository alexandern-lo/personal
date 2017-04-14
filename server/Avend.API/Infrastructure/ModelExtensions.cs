using System;
using System.Linq.Expressions;
using Avend.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avend.API.Infrastructure
{
    public static class ModelExtensions
    {
        public static PropertyBuilder<object> CreatedAt<T>(this EntityTypeBuilder<T> builder, Expression<Func<T, object>> property,
            bool index = true) where T : class
        {
            if (index) builder.HasIndex(property);
            return builder
                .Property(property)
                .ForSqlServerHasDefaultValueSql("GETUTCDATE()");
            
        }


        public static PropertyBuilder<object> UpdatedAt<T>(this EntityTypeBuilder<T> builder, Expression<Func<T, object>> property,
            bool index = true) where T : class
        {
            if (index) builder.HasIndex(property);
            return builder
                .Property(property)
                .ValueGeneratedOnAddOrUpdate()
                .ForSqlServerHasDefaultValueSql("GETUTCDATE()");            
        }

        public static PropertyBuilder<bool> SoftDeletable<T>(this EntityTypeBuilder<T> builder) where T : class, IDeletable
        {
            return builder
                .Property(x => x.Deleted)
                .HasDefaultValue(false);
        }
    }
}