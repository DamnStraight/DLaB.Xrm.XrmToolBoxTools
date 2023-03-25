﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class CrmSvcUtilTests
    {
        [TestMethod]
        public void CreateTestEntityFile()
        {
            var factory = new ServiceFactory();
            var customizeDom = new CustomizeCodeDomService(null, new DLaBModelBuilderSettings());
            var codeGen =      new ModelBuilderExtensions.Entity.CustomCodeGenerationService(factory.GetService<ICodeGenerationService>(), new DLaBModelBuilderSettings());
            var filter =       new ModelBuilderExtensions.Entity.CodeWriterFilterService(factory.GetService<ICodeWriterFilterService>(), new DLaBModelBuilderSettings());

            TestFileCreation(factory, customizeDom, codeGen, filter);
        }

        [TestMethod]
        public void CreateTestOptionSetFile()
        {
            var factory = new ServiceFactory();
            var customizeDom = new OptionSet.CustomizeCodeDomService(new Dictionary<string, string>());
            var codeGen =      new OptionSet.CustomCodeGenerationService(factory.GetService<ICodeGenerationService>(), new DLaBModelBuilderSettings());
            var filter =       new OptionSet.CodeWriterFilterService(factory.GetService<ICodeWriterFilterService>());

            TestFileCreation(factory, customizeDom, codeGen, filter);
        }

        [TestMethod]
        public void CreateTestActionFile()
        {
            var factory = new ServiceFactory();
            var customizeDom = new CustomizeCodeDomService(null, new DLaBModelBuilderSettings());
            var codeGen =      new Message.CustomCodeGenerationService(factory.GetService<ICodeGenerationService>(), new DLaBModelBuilderSettings());
            var filter =       new Message.CodeWriterFilterService(factory.GetService<ICodeWriterFilterService>());

            TestFileCreation(factory, customizeDom, codeGen, filter);
        }

        private static void TestFileCreation(ServiceFactory factory, ICustomizeCodeDomService customizeDom, ICodeGenerationService codeGen, ICodeWriterFilterService filter)
        {
            if (!Debugger.IsAttached && !ConfigHelper.GetAppSettingOrDefault("TestFileCreation", false))
            {
                return;
            }

            using (var tmp = TempDir.Create())
            {
                var fileName = Path.Combine(tmp.Name, Guid.NewGuid() + ".txt");
                try
                {
                    //factory.Add<ICustomizeCodeDomService>(new CustomizeCodeDomService(new Dictionary<string, string>
                    //{
                    //    { "url", @"https://allegient.api.crm.dynamics.com/XRMServices/2011/Organization.svc"},
                    //    { "namespace", @"Test.Xrm.Entities"},
                    //    { "out", fileName },
                    //    {"servicecontextname", "CrmContext"},
                    //    {"codecustomization", "DLaB.ModelBuilderExtensions.Entity.CustomizeCodeDomService,DLaB.ModelBuilderExtensions"},
                    //    {"codegenerationservice", "DLaB.ModelBuilderExtensions.Entity.CustomCodeGenerationService,DLaB.ModelBuilderExtensions" },
                    //    {"codewriterfilter", "DLaB.ModelBuilderExtensions.Entity.CodeWriterFilterService,DLaB.ModelBuilderExtensions"},
                    //    {"metadataproviderservice:", "DLaB.ModelBuilderExtensions.Entity.MetadataProviderService,DLaB.ModelBuilderExtensions"},
                    //    {"namingservice", "DLaB.ModelBuilderExtensions.NamingService,DLaB.ModelBuilderExtensions"},
                    //    {"username", "dlabar@allegient.com"},
                    //    {"password", "*********"}
                    //}));

                    factory.Add(customizeDom);
                    factory.Add(codeGen);
                    factory.Add(filter);
                    factory.Add<INamingService>(new NamingService(factory.GetService<INamingService>(), new Dictionary<string, string>{{ "settingsTemplateFile", "Default" }}));

                    factory.GetService<ICodeGenerationService>().Write(factory.GetMetadata(), "CS", fileName, "DLaB.ModelBuilderExtensions.UnitTest", factory.ServiceProvider);
                }
                catch (Exception ex)
                {
                    // Line for adding a debug breakpoint
                    var message = ex.Message;
                    if (message != null)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
