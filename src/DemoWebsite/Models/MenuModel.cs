﻿using System.Collections.Generic;
using Lightcore.Kernel.Data;

namespace DemoWebsite.Models
{
    public class MenuModel
    {
        public IEnumerable<IItemDefinition> MainNavigation { get; set; }
        public IEnumerable<IItemDefinition> LanguageNavigation { get; set; }
    }
}