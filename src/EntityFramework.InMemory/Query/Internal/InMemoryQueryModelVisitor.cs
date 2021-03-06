// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Query.ExpressionVisitors;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Query.Internal
{
    public class InMemoryQueryModelVisitor : EntityQueryModelVisitor
    {
        private readonly IMaterializerFactory _materializerFactory;

        public InMemoryQueryModelVisitor(
            [NotNull] IQueryOptimizer queryOptimizer,
            [NotNull] INavigationRewritingExpressionVisitorFactory navigationRewritingExpressionVisitorFactory,
            [NotNull] ISubQueryMemberPushDownExpressionVisitor subQueryMemberPushDownExpressionVisitor,
            [NotNull] IQuerySourceTracingExpressionVisitorFactory querySourceTracingExpressionVisitorFactory,
            [NotNull] IEntityResultFindingExpressionVisitorFactory entityResultFindingExpressionVisitorFactory,
            [NotNull] ITaskBlockingExpressionVisitor taskBlockingExpressionVisitor,
            [NotNull] IMemberAccessBindingExpressionVisitorFactory memberAccessBindingExpressionVisitorFactory,
            [NotNull] IOrderingExpressionVisitorFactory orderingExpressionVisitorFactory,
            [NotNull] IProjectionExpressionVisitorFactory projectionExpressionVisitorFactory,
            [NotNull] IEntityQueryableExpressionVisitorFactory entityQueryableExpressionVisitorFactory,
            [NotNull] IQueryAnnotationExtractor queryAnnotationExtractor,
            [NotNull] IResultOperatorHandler resultOperatorHandler,
            [NotNull] IEntityMaterializerSource entityMaterializerSource,
            [NotNull] IExpressionPrinter expressionPrinter,
            [NotNull] IMaterializerFactory materializerFactory,
            [NotNull] QueryCompilationContext queryCompilationContext)
            : base(
                Check.NotNull(queryOptimizer, nameof(queryOptimizer)),
                Check.NotNull(navigationRewritingExpressionVisitorFactory, nameof(navigationRewritingExpressionVisitorFactory)),
                Check.NotNull(subQueryMemberPushDownExpressionVisitor, nameof(subQueryMemberPushDownExpressionVisitor)),
                Check.NotNull(querySourceTracingExpressionVisitorFactory, nameof(querySourceTracingExpressionVisitorFactory)),
                Check.NotNull(entityResultFindingExpressionVisitorFactory, nameof(entityResultFindingExpressionVisitorFactory)),
                Check.NotNull(taskBlockingExpressionVisitor, nameof(taskBlockingExpressionVisitor)),
                Check.NotNull(memberAccessBindingExpressionVisitorFactory, nameof(memberAccessBindingExpressionVisitorFactory)),
                Check.NotNull(orderingExpressionVisitorFactory, nameof(orderingExpressionVisitorFactory)),
                Check.NotNull(projectionExpressionVisitorFactory, nameof(projectionExpressionVisitorFactory)),
                Check.NotNull(entityQueryableExpressionVisitorFactory, nameof(entityQueryableExpressionVisitorFactory)),
                Check.NotNull(queryAnnotationExtractor, nameof(queryAnnotationExtractor)),
                Check.NotNull(resultOperatorHandler, nameof(resultOperatorHandler)),
                Check.NotNull(entityMaterializerSource, nameof(entityMaterializerSource)),
                Check.NotNull(expressionPrinter, nameof(expressionPrinter)),
                Check.NotNull(queryCompilationContext, nameof(queryCompilationContext)))
        {
            Check.NotNull(materializerFactory, nameof(materializerFactory));

            _materializerFactory = materializerFactory;
        }

        protected override void IncludeNavigations(
            IncludeSpecification includeSpecification,
            Type resultType,
            LambdaExpression accessorLambda,
            bool querySourceRequiresTracking)
        {
            Check.NotNull(includeSpecification, nameof(includeSpecification));
            Check.NotNull(resultType, nameof(resultType));
            Check.NotNull(accessorLambda, nameof(accessorLambda));

            var keyComparerParameter = Expression.Parameter(typeof(IIncludeKeyComparer), "keyComparer");
            var navigationPath = includeSpecification.NavigationPath;

            Expression
                = Expression.Call(
                    _includeMethodInfo.MakeGenericMethod(resultType),
                    QueryContextParameter,
                    Expression,
                    Expression.Constant(navigationPath),
                    accessorLambda,
                    Expression.NewArrayInit(
                        typeof(RelatedEntitiesLoader),
                        navigationPath.Select(
                            n =>
                                {
                                    var targetType = n.GetTargetType();

                                    var materializer
                                        = _materializerFactory
                                            .CreateMaterializer(targetType);

                                    return Expression.Lambda<RelatedEntitiesLoader>(
                                        Expression.Call(
                                            _getRelatedValueBuffersMethodInfo,
                                            QueryContextParameter,
                                            Expression.Constant(targetType),
                                            keyComparerParameter,
                                            materializer),
                                        keyComparerParameter);
                                })),
                    Expression.Constant(querySourceRequiresTracking));
        }

        private static readonly MethodInfo _includeMethodInfo
            = typeof(InMemoryQueryModelVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(Include));

        [UsedImplicitly]
        private static IEnumerable<TResult> Include<TResult>(
            QueryContext queryContext,
            IEnumerable<TResult> source,
            IReadOnlyList<INavigation> navigationPath,
            Func<TResult, object> accessorLambda,
            IReadOnlyList<RelatedEntitiesLoader> relatedEntitiesLoaders,
            bool querySourceRequiresTracking)
        {
            return
                source
                    .Select(result =>
                        {
                            queryContext.QueryBuffer
                                .Include(
                                    accessorLambda.Invoke(result),
                                    navigationPath,
                                    relatedEntitiesLoaders,
                                    querySourceRequiresTracking);

                            return result;
                        });
        }

        private static readonly MethodInfo _getRelatedValueBuffersMethodInfo
            = typeof(InMemoryQueryModelVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(GetRelatedValueBuffers));

        [UsedImplicitly]
        private static IEnumerable<EntityLoadInfo> GetRelatedValueBuffers(
            QueryContext queryContext,
            IEntityType targetType,
            IIncludeKeyComparer keyComparer,
            Func<IEntityType, ValueBuffer, object> materializer)
        {
            return ((InMemoryQueryContext)queryContext).Store
                .GetTables(targetType)
                .SelectMany(t =>
                    t.Rows.Select(vs => new EntityLoadInfo(
                        new ValueBuffer(vs), vb => materializer(t.EntityType, vb)))
                        .Where(eli => keyComparer.ShouldInclude(eli.ValueBuffer)));
        }

        public static readonly MethodInfo EntityQueryMethodInfo
            = typeof(InMemoryQueryModelVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(EntityQuery));

        [UsedImplicitly]
        private static IEnumerable<TEntity> EntityQuery<TEntity>(
            QueryContext queryContext,
            IEntityType entityType,
            IKey key,
            Func<IEntityType, ValueBuffer, object> materializer,
            bool queryStateManager)
            where TEntity : class
        {
            return ((InMemoryQueryContext)queryContext).Store
                .GetTables(entityType)
                .SelectMany(t =>
                    t.Rows.Select(vs =>
                        {
                            var valueBuffer = new ValueBuffer(vs);

                            return (TEntity)queryContext
                                .QueryBuffer
                                .GetEntity(
                                    key,
                                    new EntityLoadInfo(
                                        valueBuffer,
                                        vr => materializer(t.EntityType, vr)),
                                    queryStateManager, 
                                    throwOnNullKey: false);
                        }));
        }

        public static readonly MethodInfo ProjectionQueryMethodInfo
            = typeof(InMemoryQueryModelVisitor).GetTypeInfo()
                .GetDeclaredMethod(nameof(ProjectionQuery));

        [UsedImplicitly]
        private static IEnumerable<ValueBuffer> ProjectionQuery(
            QueryContext queryContext,
            IEntityType entityType)
        {
            return ((InMemoryQueryContext)queryContext).Store
                .GetTables(entityType)
                .SelectMany(t => t.Rows.Select(vs => new ValueBuffer(vs)));
        }
    }
}
