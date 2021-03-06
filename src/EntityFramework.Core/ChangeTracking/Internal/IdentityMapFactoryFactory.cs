// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;

namespace Microsoft.Data.Entity.ChangeTracking.Internal
{
    public class IdentityMapFactoryFactory : IdentityMapFactoryFactoryBase
    {
        [CallsMakeGenericMethod(nameof(CreateFactory), typeof(TypeArgumentCategory.Keys))]
        public virtual Func<IIdentityMap> Create([NotNull] IKey key)
            => (Func<IIdentityMap>)typeof(IdentityMapFactoryFactory).GetTypeInfo()
            .GetDeclaredMethod(nameof(CreateFactory))
            .MakeGenericMethod(GetKeyType(key))
            .Invoke(null, new object[] { key });

        [UsedImplicitly]
        private static Func<IIdentityMap> CreateFactory<TKey>(IKey key)
        {
            var factory = key.GetPrincipalKeyValueFactory<TKey>();

            return typeof(TKey).IsNullableType()
                ? (Func<IIdentityMap>)(() => new NullableKeyIdentityMap<TKey>(key, factory))
                : () => new IdentityMap<TKey>(key, factory);
        }
    }
}
