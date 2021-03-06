// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Update
{
    public abstract class UpdateSqlGenerator : IUpdateSqlGenerator
    {
        protected UpdateSqlGenerator([NotNull] ISqlGenerationHelper sqlGenerationHelper)
        {
            Check.NotNull(sqlGenerationHelper, nameof(sqlGenerationHelper));

            SqlGenerationHelper = sqlGenerationHelper;
        }

        protected virtual ISqlGenerationHelper SqlGenerationHelper { get; }

        public virtual ResultSetMapping AppendInsertOperation(StringBuilder commandStringBuilder, ModificationCommand command, int commandPosition)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(command, nameof(command));

            var name = command.TableName;
            var schema = command.Schema;
            var operations = command.ColumnModifications;

            var writeOperations = operations.Where(o => o.IsWrite).ToArray();
            var readOperations = operations.Where(o => o.IsRead).ToArray();

            AppendInsertCommand(commandStringBuilder, name, schema, writeOperations);

            if (readOperations.Length > 0)
            {
                var keyOperations = operations.Where(o => o.IsKey).ToArray();

                return AppendSelectAffectedCommand(commandStringBuilder, name, schema, readOperations, keyOperations, commandPosition);
            }

            return ResultSetMapping.NoResultSet;
        }

        public virtual ResultSetMapping AppendUpdateOperation(StringBuilder commandStringBuilder, ModificationCommand command, int commandPosition)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(command, nameof(command));

            var name = command.TableName;
            var schema = command.Schema;
            var operations = command.ColumnModifications;

            var writeOperations = operations.Where(o => o.IsWrite).ToArray();
            var conditionOperations = operations.Where(o => o.IsCondition).ToArray();
            var readOperations = operations.Where(o => o.IsRead).ToArray();

            AppendUpdateCommand(commandStringBuilder, name, schema, writeOperations, conditionOperations);

            if (readOperations.Length > 0)
            {
                var keyOperations = operations.Where(o => o.IsKey).ToArray();

                return AppendSelectAffectedCommand(commandStringBuilder, name, schema, readOperations, keyOperations, commandPosition);
            }
            return AppendSelectAffectedCountCommand(commandStringBuilder, name, schema, commandPosition);
        }

        public virtual ResultSetMapping AppendDeleteOperation(StringBuilder commandStringBuilder, ModificationCommand command, int commandPosition)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(command, nameof(command));

            var name = command.TableName;
            var schema = command.Schema;
            var conditionOperations = command.ColumnModifications.Where(o => o.IsCondition).ToArray();

            AppendDeleteCommand(commandStringBuilder, name, schema, conditionOperations);

            return AppendSelectAffectedCountCommand(commandStringBuilder, name, schema, commandPosition);
        }

        protected virtual void AppendInsertCommand(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema,
            [NotNull] IReadOnlyList<ColumnModification> writeOperations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(writeOperations, nameof(writeOperations));

            AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
            AppendValuesHeader(commandStringBuilder, writeOperations);
            AppendValues(commandStringBuilder, writeOperations);
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
        }

        protected virtual void AppendUpdateCommand(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema,
            [NotNull] IReadOnlyList<ColumnModification> writeOperations,
            [NotNull] IReadOnlyList<ColumnModification> conditionOperations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(writeOperations, nameof(writeOperations));
            Check.NotNull(conditionOperations, nameof(conditionOperations));

            AppendUpdateCommandHeader(commandStringBuilder, name, schema, writeOperations);
            AppendWhereClause(commandStringBuilder, conditionOperations);
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
        }

        protected virtual void AppendDeleteCommand(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema,
            [NotNull] IReadOnlyList<ColumnModification> conditionOperations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(conditionOperations, nameof(conditionOperations));

            AppendDeleteCommandHeader(commandStringBuilder, name, schema);
            AppendWhereClause(commandStringBuilder, conditionOperations);
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
        }

        protected virtual ResultSetMapping AppendSelectAffectedCountCommand(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema,
            int commandPosition)
            => ResultSetMapping.NoResultSet;

        protected virtual ResultSetMapping AppendSelectAffectedCommand(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema,
            [NotNull] IReadOnlyList<ColumnModification> readOperations,
            [NotNull] IReadOnlyList<ColumnModification> conditionOperations,
            int commandPosition)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(readOperations, nameof(readOperations));
            Check.NotNull(conditionOperations, nameof(conditionOperations));

            AppendSelectCommandHeader(commandStringBuilder, readOperations);
            AppendFromClause(commandStringBuilder, name, schema);
            // TODO: there is no notion of operator - currently all the where conditions check equality
            AppendWhereAffectedClause(commandStringBuilder, conditionOperations);
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();

            return ResultSetMapping.LastInResultSet;
        }

        protected virtual void AppendInsertCommandHeader(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema,
            [NotNull] IReadOnlyList<ColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operations, nameof(operations));

            commandStringBuilder
                .Append("INSERT INTO ")
                .Append(SqlGenerationHelper.DelimitIdentifier(name, schema));

            if (operations.Count > 0)
            {
                commandStringBuilder
                    .Append(" (")
                    .AppendJoin(operations.Select(o => SqlGenerationHelper.DelimitIdentifier(o.ColumnName)))
                    .Append(")");
            }
        }

        protected virtual void AppendDeleteCommandHeader(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(name, nameof(name));

            commandStringBuilder
                .Append("DELETE FROM ")
                .Append(SqlGenerationHelper.DelimitIdentifier(name, schema));
        }

        protected virtual void AppendUpdateCommandHeader(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema,
            [NotNull] IReadOnlyList<ColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(operations, nameof(operations));

            commandStringBuilder
                .Append("UPDATE ")
                .Append(SqlGenerationHelper.DelimitIdentifier(name, schema))
                .Append(" SET ")
                .AppendJoin(
                    operations,
                    (sb, v) => sb
                        .Append(SqlGenerationHelper.DelimitIdentifier(v.ColumnName))
                        .Append(" = ")
                        .Append(SqlGenerationHelper.GenerateParameterName(v.ParameterName)),
                    ", ");
        }

        protected virtual void AppendSelectCommandHeader(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] IReadOnlyList<ColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(operations, nameof(operations));

            commandStringBuilder
                .Append("SELECT ")
                .AppendJoin(operations.Select(c => SqlGenerationHelper.DelimitIdentifier(c.ColumnName)));
        }

        protected virtual void AppendFromClause(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] string name,
            [CanBeNull] string schema)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(name, nameof(name));

            commandStringBuilder
                .AppendLine()
                .Append("FROM ")
                .Append(SqlGenerationHelper.DelimitIdentifier(name, schema));
        }

        protected virtual void AppendValuesHeader(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] IReadOnlyList<ColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(operations, nameof(operations));

            commandStringBuilder.AppendLine();
            commandStringBuilder.Append(operations.Count > 0 ? "VALUES " : "DEFAULT VALUES");
        }

        protected virtual void AppendValues(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] IReadOnlyList<ColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(operations, nameof(operations));

            if (operations.Count > 0)
            {
                commandStringBuilder
                    .Append("(")
                    .AppendJoin(operations.Select(
                        o => o.IsWrite ? SqlGenerationHelper.GenerateParameterName(o.ParameterName) : "DEFAULT"))
                    .Append(")");
            }
        }

        protected virtual void AppendWhereClause(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] IReadOnlyList<ColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(operations, nameof(operations));

            if (operations.Count > 0)
            {
                commandStringBuilder
                    .AppendLine()
                    .Append("WHERE ")
                    .AppendJoin(operations, (sb, v) => AppendWhereCondition(sb, v, useOriginalValue: true), " AND ");
            }
        }

        protected virtual void AppendWhereAffectedClause(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] IReadOnlyList<ColumnModification> operations)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(operations, nameof(operations));

            commandStringBuilder
                .AppendLine()
                .Append("WHERE ");

            AppendRowsAffectedWhereCondition(commandStringBuilder, 1);

            if (operations.Count > 0)
            {
                commandStringBuilder
                    .Append(" AND ")
                    .AppendJoin(operations, (sb, v) =>
                        {
                            if (v.IsKey)
                            {
                                if (v.IsRead)
                                {
                                    AppendIdentityWhereCondition(sb, v);
                                }
                                else
                                {
                                    AppendWhereCondition(sb, v, useOriginalValue: !v.IsWrite);
                                }
                            }
                        }, " AND ");
            }
        }

        protected abstract void AppendRowsAffectedWhereCondition([NotNull] StringBuilder commandStringBuilder, int expectedRowsAffected);

        protected virtual void AppendWhereCondition(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] ColumnModification columnModification,
            bool useOriginalValue)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(columnModification, nameof(columnModification));

            commandStringBuilder
                .Append(SqlGenerationHelper.DelimitIdentifier(columnModification.ColumnName))
                .Append(" = ")
                .Append(
                    SqlGenerationHelper.GenerateParameterName(
                        useOriginalValue
                            ? columnModification.OriginalParameterName
                            : columnModification.ParameterName));
        }

        protected abstract void AppendIdentityWhereCondition(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] ColumnModification columnModification);

        public virtual void AppendBatchHeader(StringBuilder commandStringBuilder)
        {
        }

        public virtual string GenerateNextSequenceValueOperation(string name, string schema)
            => "SELECT NEXT VALUE FOR " + SqlGenerationHelper.DelimitIdentifier(Check.NotNull(name, nameof(name)), schema);
    }
}
