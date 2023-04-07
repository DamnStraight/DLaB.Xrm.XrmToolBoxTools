﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using System.Linq;
using XrmToolBox.Extensibility;

namespace DLaB.XrmToolBoxCommon
{
    /// <summary>
    /// This class adds the following three major features:
    /// Fully Implements IMsCrmToolsPluginUserControl
    /// Defines an Event for when the Connection is Updated, useful if needing to know when to refresh a connection specific cache
    /// Fully Implements the IWorkerHost which provides a much nicer api for requesting a connection then calling a method
    /// </summary>
    public abstract class PluginFactory : PluginBase
    {
        protected const string SmallImage32X32 =
            "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";

        protected const string LargeImage120X120 =
            "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";
    }
}
