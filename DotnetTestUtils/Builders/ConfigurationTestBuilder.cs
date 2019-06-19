using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetTestUtils.Builders
{
    public class ConfigurationTestBuilder
    {
        private readonly Mock<IConfigurationRoot> _configurationStub;

        public ConfigurationTestBuilder()
        {
            _configurationStub = new Mock<IConfigurationRoot>();
        }

        public ConfigurationTestBuilder WithSetting(string settingName, string settingValue)
        {
            _configurationStub.Setup(x => x[settingName]).Returns(settingValue);
            return this;
        }

        public IConfigurationRoot Build()
        {
            return _configurationStub.Object;
        }
    }
}
