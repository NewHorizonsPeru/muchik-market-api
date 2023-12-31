﻿using midis.muchik.market.domain.entities;
using midis.muchik.market.domain.interfaces;
using midis.muchik.market.infrastructure.context;

namespace midis.muchik.market.infrastructure.repositories
{
    public class CustomerRepository : GenericRepository<CommonContext, Customer>, ICustomerRepository
    {
        public CustomerRepository(CommonContext context) : base(context) { }
    }
}
