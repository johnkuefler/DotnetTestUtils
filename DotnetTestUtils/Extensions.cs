using AutoFixture;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetTestUtils
{
    public static class Extensions
    { 
        public static void RegisterDbContext<T>(this Fixture fixture, T context)
        {
            fixture.Register(() => context);
        }
    }
}
