﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace c3IDE.Utilities.Helpers
{
    public class ResourceReader : Singleton<ResourceReader>
    {
        private readonly Assembly _currentAssmbley;
        private readonly Dictionary<string, string> _resourceCache;

        public ResourceReader()
        {
            _currentAssmbley = Assembly.GetExecutingAssembly();
            _resourceCache = new Dictionary<string, string>();
        }

        public string GetResourceText(string name)
        {
            if (_resourceCache.ContainsKey(name))
            {
                return _resourceCache[name];
            }

            using (var stream = _currentAssmbley.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                var resource = reader.ReadToEnd();
                _resourceCache.Add(name, resource);
                return resource;
            }
        }

        public string GetResourceAsBase64(string name)
        {
            if (_resourceCache.ContainsKey(name))
            {
                return _resourceCache[name];
            }

            using (var stream = _currentAssmbley.GetManifestResourceStream(name))
            {
                var img = Image.FromStream(stream ?? throw new InvalidOperationException());
                var base64 = ImageHelper.Insatnce.ImageToBase64(img);

                _resourceCache.Add(name, base64);
                return base64;
            }

            throw new InvalidOperationException("Failed to read base 64 icon");
        }

        public IEnumerable<string> LogResourceFiles()
        {
            var list = new List<string>();
            var resources = _currentAssmbley.GetManifestResourceNames();
            foreach (var resource in resources)
            {
                Console.WriteLine(resource);
                list.Add(resource);
            }

            return list;
        }
    }
}
