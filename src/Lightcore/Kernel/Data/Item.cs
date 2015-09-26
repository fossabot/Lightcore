﻿using System;
using System.Collections.Generic;

namespace Lightcore.Kernel.Data
{
    public class Item
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public string Trace { get; set; }
        public Guid Id { get; set; }
        public string Layout { get; set; }
        public Language Language { get; set; }
        public IEnumerable<Rendering> Renderings { get; set; }
        public IEnumerable<Item> Children { get; set; }
        public IEnumerable<Field> Fields { get; set; }
    }
}