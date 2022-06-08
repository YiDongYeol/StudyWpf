﻿using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyDataTest
{
    public class SampleRepository
    {
        public IEnumerable<Customer> GetCustomers()
        {
            Randomizer.Seed = new Random(123456);
            var genCustomer = new Faker<Customer>()
                .RuleFor(r => r.Id, Guid.NewGuid)
                .RuleFor(r => r.Name, f => f.Company.CompanyName())
                .RuleFor(r => r.Address, f => f.Address.FullAddress())
                .RuleFor(r => r.Phone, f => f.Phone.PhoneNumber("010-####-####"))
                .RuleFor(r => r.ContactName, f => f.Name.FullName());

            return genCustomer.Generate(10);
        }
    }
}
