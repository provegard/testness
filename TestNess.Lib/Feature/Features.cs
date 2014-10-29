// Copyright (C) 2011-2014 Per Rovegård, http://rovegard.com
// This file is subject to the terms and conditions of the MIT license. See the file 'LICENSE',
// which is part of this source code package, or http://per.mit-license.org/2011.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TestNess.Lib.Feature
{
    public class Features
    {
        private static readonly IList<Type> FeatureTypes;
        private readonly Dictionary<Type, Lazy<IFeature>> _instancesByType;

        static Features()
        {
            var iFeature = typeof (IFeature);
            FeatureTypes =
                typeof (Features).Assembly.GetTypes().Where(t => iFeature.IsAssignableFrom(t) && !t.IsAbstract).ToList();
        }

        public Features(TestCase testCase)
        {
            _instancesByType =
                FeatureTypes
                    .Select(t => new { value = new Lazy<IFeature>(() => Activator.CreateInstance(t, testCase, this) as IFeature), type = t })
                    .ToDictionary(x => x.type, x => x.value);
        }

        public T Get<T>() where T : IFeature
        {
            Lazy<IFeature> m;
            if (!_instancesByType.TryGetValue(typeof (T), out m))
                throw new ArgumentException("Unknown feature: " + typeof (T).Name);
            return (T) m.Value;
        }
    }
}
