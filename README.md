# EFCoreBug
Bug in Entity Framework Core when materializing navigation properties to an anonymous type.

There appears to be a bug in EF Core when materializing anonymous types that include a navigation property.

It looks like the entity is being "tracked" even when the query only contains anonymous types.
