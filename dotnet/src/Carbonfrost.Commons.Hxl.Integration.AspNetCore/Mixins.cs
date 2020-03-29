//
// Copyright 2013, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    static class Mixins {

        public static void AddMany<T>(this ICollection<T> items, IEnumerable<T> others) {
            if (others == null) {
                return;
            }

            foreach (var element in others) {
                items.Add(element);
            }
        }

        public static TValue GetValueOrCache<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> factory) {
            TValue value;
            if (source.TryGetValue(key, out value)) {
                return value;
            }

            source.Add(key, value = factory(key));
            return value;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key) {
            TValue value;
            if (source.TryGetValue(key, out value)) {
                return value;
            }

            return default(TValue);
        }
    }
}
