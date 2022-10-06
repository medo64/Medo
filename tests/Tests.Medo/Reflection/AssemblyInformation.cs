using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Reflection;

namespace Tests;

[TestClass]
public class AssemblyInformation_Tests {

    [TestMethod]
    public void AssemblyInformation_Entry() {
        Assert.AreEqual("testhost, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", AssemblyInformation.Entry.FullName);
        Assert.AreEqual("testhost", AssemblyInformation.Entry.Name);
        Assert.AreEqual(new Version(15, 0, 0, 0), AssemblyInformation.Entry.Version);
        Assert.AreEqual("15.0.0", AssemblyInformation.Entry.SemanticVersionText);
        Assert.AreEqual("testhost", AssemblyInformation.Entry.Title);
        Assert.AreEqual("Microsoft.TestHost", AssemblyInformation.Entry.Product);
        Assert.AreEqual("", AssemblyInformation.Entry.Description);
        Assert.AreEqual("Microsoft Corporation", AssemblyInformation.Entry.Company);
        Assert.AreEqual("© Microsoft Corporation. All rights reserved.", AssemblyInformation.Entry.Copyright);
    }

    [TestMethod]
    public void AssemblyInformation_Calling() {
        Assert.AreEqual("System.Private.CoreLib", AssemblyInformation.Calling.Name);
#if NET6_0
        Assert.AreEqual("System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", AssemblyInformation.Calling.FullName);
        Assert.AreEqual(new Version(6, 0, 0, 0), AssemblyInformation.Calling.Version);
        Assert.AreEqual("6.0.0", AssemblyInformation.Calling.SemanticVersionText);
#elif NET7_0
        Assert.AreEqual("System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", AssemblyInformation.Calling.FullName);
        Assert.AreEqual(new Version(7, 0, 0, 0), AssemblyInformation.Calling.Version);
        Assert.AreEqual("7.0.0", AssemblyInformation.Calling.SemanticVersionText);
#endif
        Assert.AreEqual("System.Private.CoreLib", AssemblyInformation.Calling.Title);
        Assert.AreEqual("Microsoft® .NET", AssemblyInformation.Calling.Product);
        Assert.AreEqual("System.Private.CoreLib", AssemblyInformation.Calling.Description);
        Assert.AreEqual("Microsoft Corporation", AssemblyInformation.Calling.Company);
        Assert.AreEqual("© Microsoft Corporation. All rights reserved.", AssemblyInformation.Calling.Copyright);
    }

    [TestMethod]
    public void AssemblyInformation_Executing() {
        Assert.AreEqual("Medo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", AssemblyInformation.Executing.FullName);
        Assert.AreEqual("Medo", AssemblyInformation.Executing.Name);
        Assert.AreEqual(new Version(1, 0, 0, 0), AssemblyInformation.Executing.Version);
        Assert.AreEqual("1.0.0", AssemblyInformation.Executing.SemanticVersionText);
        Assert.AreEqual("Medo", AssemblyInformation.Executing.Title);
        Assert.AreEqual("Medo", AssemblyInformation.Executing.Product);
        Assert.AreEqual("Common classes", AssemblyInformation.Executing.Description);
        Assert.AreEqual("Josip Medved", AssemblyInformation.Executing.Company);
        Assert.AreEqual("Copyright (c) 2004 Josip Medved", AssemblyInformation.Executing.Copyright);
    }

}
