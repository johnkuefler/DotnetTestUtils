using AutoFixture;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DotnetTestUtils.Builders
{
    public class DbContextTestBuilder<ContextType> where ContextType : DbContext
    {
        private readonly ContextType _context;

        public DbContextTestBuilder()
        {
            DbContextOptions<ContextType> options = new DbContextOptionsBuilder<ContextType>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = Activator.CreateInstance(typeof(ContextType),
                  new object[] { options }) as ContextType;
        }

        public DbContextTestBuilder<ContextType> WithRecords<T>(List<T> records) where T : class
        {
            foreach (var record in records)
            {
                _context.Add(record);
            }
            _context.SaveChanges();

            return this;
        }

        public DbContextTestBuilder<ContextType> WithRecord<T>(T record) where T : class
        {
            _context.Add(record);
            _context.SaveChanges();

            return this;
        }

        public DbContextTestBuilder<ContextType> WithRandomRecords<T>(int count = 5)
        {
            Fixture fixture = new FixtureFactory().WithDefaults().Create();
            fixture.RepeatCount = 0;
            
            for (int i=0; i<count; i++)
            {
                _context.Add(fixture.Create<T>());
            }
            _context.SaveChanges();

            return this;
        }

        public ContextType Build()
        {
            return _context;
        }
    }
}
