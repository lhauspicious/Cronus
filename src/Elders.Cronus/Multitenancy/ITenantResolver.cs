﻿using System;
using System.Linq;
using Elders.Cronus.EventStore;
using Elders.Cronus.Projections;

namespace Elders.Cronus.Multitenancy
{
    public interface ITenantResolver
    {
        string Resolve(IAggregateRootId id);

        string Resolve(AggregateCommit aggregateCommit);

        string Resolve(ProjectionCommit projectionCommit);

        string Resolve(IBlobId id);

        string Resolve(IMessage message);
    }

    public class DefaultTenantResolver : ITenantResolver
    {
        public string Resolve(ProjectionCommit projectionCommit)
        {
            if (ReferenceEquals(null, projectionCommit) == true) throw new ArgumentNullException(nameof(projectionCommit));

            var tenant = string.Empty;
            if (TryResolve(projectionCommit.ProjectionId.RawId, out tenant))
                return tenant;

            throw new NotSupportedException($"Unable to resolve tenant for id {projectionCommit.ProjectionId}");
        }

        public string Resolve(IBlobId id)
        {
            if (ReferenceEquals(null, id) == true) throw new ArgumentNullException(nameof(id));

            var tenant = string.Empty;
            if (TryResolve(id.RawId, out tenant))
                return tenant;

            throw new NotSupportedException($"Unable to resolve tenant for id {id}");
        }

        public string Resolve(IAggregateRootId id)
        {
            if (ReferenceEquals(null, id) == true) throw new ArgumentNullException(nameof(id));

            if (id is StringTenantId)
                return ((StringTenantId)id).Tenant;

            throw new NotSupportedException($"Unable to resolve tenant for id {id}");
        }

        public string Resolve(AggregateCommit aggregateCommit)
        {
            if (ReferenceEquals(null, aggregateCommit) == true) throw new ArgumentNullException(nameof(aggregateCommit));

            var tenant = string.Empty;
            if (TryResolve(aggregateCommit.AggregateRootId, out tenant))
                return tenant;

            throw new NotSupportedException($"Unable to resolve tenant for id {aggregateCommit.AggregateRootId}");
        }

        public string Resolve(IMessage message)
        {
            var tenantPropertyMeta = message.GetType().GetProperty("Tenant", typeof(string));
            if (tenantPropertyMeta is null == false)
            {
                return (string)tenantPropertyMeta.GetValue(message);
            }

            var idMeta = message.GetType().GetProperties().Where(p => typeof(IBlobId).IsAssignableFrom(p.PropertyType)).FirstOrDefault();
            IBlobId id = idMeta?.GetValue(message) as IBlobId;
            if (id is null == false)
            {
                return Resolve(id);
            }

            throw new NotSupportedException($"Unable to resolve tenant from {message}");
        }

        bool TryResolve(byte[] id, out string tenant)
        {
            tenant = string.Empty;
            var urn = System.Text.Encoding.UTF8.GetString(id);
            StringTenantUrn stringTenantUrn;

            if (StringTenantUrn.TryParse(urn, out stringTenantUrn))
            {
                tenant = stringTenantUrn.Tenant;
                return true;
            }

            return false;
        }
    }
}
