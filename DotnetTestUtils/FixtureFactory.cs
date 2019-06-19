using AutoFixture;
using AutoFixture.AutoMoq;
using DotnetTestUtils.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace DotnetTestUtils
{
    public partial class FixtureFactory
    {
        private readonly Fixture _fixture;

        public FixtureFactory()
        {
            _fixture = new Fixture();

        }

        public virtual FixtureFactory WithDefaults()
        {
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });

            _fixture.RepeatCount = 0;

            return this;
        }
     
        public virtual FixtureFactory WithDbContext<ContextType>(ContextType customContext = null) where ContextType : DbContext
        {
            if (customContext == null)
            {
                _fixture.Register(() => new DbContextTestBuilder<ContextType>().Build());
            }
            else
            {
                _fixture.Register(() => customContext);
            }

            return this;
        }

        public virtual FixtureFactory WithUserManager<TUser>() where TUser : class
        {
            Mock<IUserStore<TUser>> userStoreMock = new Mock<IUserStore<TUser>>();
            Mock<UserManager<TUser>> userManagerMock = new Mock<UserManager<TUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<TUser>());

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<TUser>());

            userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_fixture.Create<TUser>());

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<TUser>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<TUser>()))
                .ReturnsAsync(IdentityResult.Success);


            _fixture.Register(() => userManagerMock.Object);

            Mock<SignInManager<TUser>> signinManagerMock = new Mock<SignInManager<TUser>>(userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object, null, null, null);

            signinManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<TUser>(), It.IsAny<string>(), false, true))
                .ReturnsAsync(SignInResult.Success);

            _fixture.Register(() => signinManagerMock.Object);

            return this;
        }

        public Fixture Create()
        {
            return _fixture;
        }
    }
}
