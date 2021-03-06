﻿using System;
using System.Linq;
using Elders.Cronus.Multitenancy;

namespace Elders.Cronus.MessageProcessing
{
    public class CronusContextFactory
    {
        private readonly CronusContext context;
        private readonly ITenantList tenants;
        private readonly ITenantResolver tenantResolver;

        public CronusContextFactory(CronusContext context, ITenantList tenants, ITenantResolver tenantResolver)
        {
            this.context = context;
            this.tenants = tenants;
            this.tenantResolver = tenantResolver;
        }

        public CronusContext GetContext(object obj, IServiceProvider serviceProvider)
        {
            if (context.IsNotInitialized)
            {
                string tenant = tenantResolver.Resolve(obj);
                EnsureValidTenant(tenant);
                context.Tenant = tenant;
                context.ServiceProvider = serviceProvider;
            }

            return context;
        }

        private void EnsureValidTenant(string tenant)
        {
            if (string.IsNullOrEmpty(tenant)) throw new ArgumentNullException(nameof(tenant));

            if (tenants.GetTenants().Where(t => t.Equals(tenant, StringComparison.OrdinalIgnoreCase)).Any() == false)
                throw new ArgumentException($"The tenant `{tenant}` is not registered. Make sure that the tenant `{tenant}` is properly configured using `cronus_tenants`. More info at https://github.com/Elders/Cronus/blob/master/doc/Configuration.md", nameof(tenant));
        }
    }
}
