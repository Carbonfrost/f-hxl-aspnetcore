//
// Copyright 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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

    class ViewLocationCacheResult {
        public readonly Func<HxlPage> PageFactory;
        public readonly List<string> SearchedLocations = new List<string>();

        public bool Success {
            get {
                return PageFactory != null;
            }
        }

        private ViewLocationCacheResult(Func<HxlPage> pageFactory) {
            PageFactory = pageFactory;
        }

        public static ViewLocationCacheResult Found(Func<HxlPage> pageFactory) {
            return new ViewLocationCacheResult(pageFactory);
        }

        public static ViewLocationCacheResult NotFound(IEnumerable<string> searchedLocations) {
            var result = new ViewLocationCacheResult(null);
            result.SearchedLocations.AddRange(searchedLocations);
            return result;
        }
    }
}
