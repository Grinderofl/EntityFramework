// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Metadata
{
    public class NavigationAccessorSource
    {
        private readonly ThreadSafeDictionaryCache<Tuple<Type, string>, NavigationAccessor> _cache
            = new ThreadSafeDictionaryCache<Tuple<Type, string>, NavigationAccessor>();

        private readonly ClrCollectionAccessorSource _collectionAccessorSource;
        private readonly ClrPropertySetterSource _setterSource;
        private readonly ClrPropertyGetterSource _getterSource;

        public NavigationAccessorSource(
            [NotNull] ClrPropertyGetterSource getterSource,
            [NotNull] ClrPropertySetterSource setterSource,
            [NotNull] ClrCollectionAccessorSource collectionAccessorSource)
        {
            Check.NotNull(getterSource, "getterSource");
            Check.NotNull(setterSource, "setterSource");
            Check.NotNull(collectionAccessorSource, "collectionAccessorSource");

            _getterSource = getterSource;
            _setterSource = setterSource;
            _collectionAccessorSource = collectionAccessorSource;
        }

        public virtual NavigationAccessor GetAccessor([NotNull] INavigation navigation)
        {
            Check.NotNull(navigation, "navigation");

            return _cache.GetOrAdd(
                Tuple.Create(navigation.EntityType.Type, navigation.Name),
                k => Create(navigation));
        }

        private NavigationAccessor Create(INavigation navigation)
        {
            var elementType = navigation.EntityType.Type.GetAnyProperty(navigation.Name).PropertyType.TryGetElementType(typeof(IEnumerable<>));

            var targetType = navigation.PointsToPrincipal
                ? navigation.ForeignKey.ReferencedEntityType
                : navigation.ForeignKey.EntityType;

            // TODO: Consider allowing an annotation to force treating a reference to an Entity which is an IEnumerable of
            // itself as a reference instead of a collection. Currently it will be considered a collection nav prop, which it isn't.

            return elementType != null && elementType.GetTypeInfo().IsAssignableFrom(targetType.Type.GetTypeInfo())
                ? new CollectionNavigationAccessor(
                    () => _getterSource.GetAccessor(navigation),
                    () => _setterSource.GetAccessor(navigation),
                    () => _collectionAccessorSource.GetAccessor(navigation))
                : new NavigationAccessor(
                    () => _getterSource.GetAccessor(navigation),
                    () => _setterSource.GetAccessor(navigation));
        }
    }
}