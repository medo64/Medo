using System;
using Xunit;
using Medo.Reflection;

namespace Tests.Medo.Reflection {
    public class AssemblyInformationTests {

        [Fact(DisplayName = "AssemblyInformation: Entry")]
        public void Entry() {
            Assert.Equal("testhost, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", AssemblyInformation.Entry.FullName);
            Assert.Equal("testhost", AssemblyInformation.Entry.Name);
            Assert.Equal(new Version(15, 0, 0, 0), AssemblyInformation.Entry.Version);
            Assert.Equal("15.0.0", AssemblyInformation.Entry.SemanticVersionText);
            Assert.Equal("testhost", AssemblyInformation.Entry.Title);
            Assert.Equal("Microsoft.TestHost", AssemblyInformation.Entry.Product);
            Assert.Equal("", AssemblyInformation.Entry.Description);
            Assert.Equal("Microsoft Corporation", AssemblyInformation.Entry.Company);
            Assert.Equal("© Microsoft Corporation. All rights reserved.", AssemblyInformation.Entry.Copyright);
        }

        [Fact(DisplayName = "AssemblyInformation: Calling")]
        public void Calling() {
            Assert.Equal("System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", AssemblyInformation.Calling.FullName);
            Assert.Equal("System.Private.CoreLib", AssemblyInformation.Calling.Name);
            Assert.Equal(new Version(6, 0, 0, 0), AssemblyInformation.Calling.Version);
            Assert.Equal("6.0.0", AssemblyInformation.Calling.SemanticVersionText);
            Assert.Equal("System.Private.CoreLib", AssemblyInformation.Calling.Title);
            Assert.Equal("Microsoft® .NET", AssemblyInformation.Calling.Product);
            Assert.Equal("System.Private.CoreLib", AssemblyInformation.Calling.Description);
            Assert.Equal("Microsoft Corporation", AssemblyInformation.Calling.Company);
            Assert.Equal("© Microsoft Corporation. All rights reserved.", AssemblyInformation.Calling.Copyright);
        }

        [Fact(DisplayName = "AssemblyInformation: Executing")]
        public void Executing() {
            Assert.Equal("Medo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AssemblyInformation.Executing.FullName);
            Assert.Equal("Medo", AssemblyInformation.Executing.Name);
            Assert.Equal(new Version(1, 0, 0, 0), AssemblyInformation.Executing.Version);
            Assert.Equal("1.0.0", AssemblyInformation.Executing.SemanticVersionText);
            Assert.Equal("Medo", AssemblyInformation.Executing.Title);
            Assert.Equal("Medo", AssemblyInformation.Executing.Product);
            Assert.Equal("Common classes", AssemblyInformation.Executing.Description);
            Assert.Equal("Josip Medved", AssemblyInformation.Executing.Company);
            Assert.Equal("Copyright (c) 2004 Josip Medved", AssemblyInformation.Executing.Copyright);
        }

    }
}
