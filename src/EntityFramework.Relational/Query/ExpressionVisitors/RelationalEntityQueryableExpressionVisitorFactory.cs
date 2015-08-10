// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query.Sql;
using Microsoft.Data.Entity.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.Data.Entity.Query.ExpressionVisitors
{
    public class RelationalEntityQueryableExpressionVisitorFactory : IEntityQueryableExpressionVisitorFactory
    {
        private readonly IModel _model;
        private readonly IEntityKeyFactorySource _entityKeyFactorySource;
        private readonly IMaterializerFactory _materializerFactory;
        private readonly ISqlQueryGeneratorFactory _sqlQueryGeneratorFactory;
        private readonly ICommandBuilderFactory _commandBuilderFactory;
        private readonly IRelationalMetadataExtensionProvider _relationalMetadataExtensionProvider;

        public RelationalEntityQueryableExpressionVisitorFactory(
            [NotNull] IModel model,
            [NotNull] IEntityKeyFactorySource entityKeyFactorySource,
            [NotNull] IMaterializerFactory materializerFactory,
            [NotNull] ISqlQueryGeneratorFactory sqlQueryGeneratorFactory,
            [NotNull] ICommandBuilderFactory commandBuilderFactory,
            [NotNull] IRelationalMetadataExtensionProvider relationalMetadataExtensionProvider)
        {
            Check.NotNull(model, nameof(model));
            Check.NotNull(entityKeyFactorySource, nameof(entityKeyFactorySource));
            Check.NotNull(materializerFactory, nameof(materializerFactory));
            Check.NotNull(sqlQueryGeneratorFactory, nameof(sqlQueryGeneratorFactory));
            Check.NotNull(commandBuilderFactory, nameof(commandBuilderFactory));
            Check.NotNull(relationalMetadataExtensionProvider, nameof(relationalMetadataExtensionProvider));

            _model = model;
            _entityKeyFactorySource = entityKeyFactorySource;
            _materializerFactory = materializerFactory;
            _sqlQueryGeneratorFactory = sqlQueryGeneratorFactory;
            _commandBuilderFactory = commandBuilderFactory;
            _relationalMetadataExtensionProvider = relationalMetadataExtensionProvider;
        }

        public virtual ExpressionVisitor Create(
            [NotNull] EntityQueryModelVisitor queryModelVisitor,
            [NotNull] IQuerySource querySource)
            => new RelationalEntityQueryableExpressionVisitor(
                _model,
                _entityKeyFactorySource,
                _materializerFactory,
                _sqlQueryGeneratorFactory,
                _commandBuilderFactory,
                _relationalMetadataExtensionProvider,
                (RelationalQueryModelVisitor)Check.NotNull(queryModelVisitor, nameof(queryModelVisitor)),
                Check.NotNull(querySource, nameof(querySource)));
    }
}
