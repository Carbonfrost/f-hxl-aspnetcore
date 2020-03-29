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

    public struct HxlPageResult {

        public string Name { get; private set; }
        public HxlPage Page { get; private set; }
        public IEnumerable<string> SearchedLocations { get; private set; }

        private HxlPageResult(string name, HxlPage page) {
            Name = name;
            Page = page;
            SearchedLocations = null;
        }

        private HxlPageResult(string name, IEnumerable<string> searchedLocations) {
            Name = name;
            Page = null;
            SearchedLocations = searchedLocations;
        }

        public static HxlPageResult NotFound(string name, IEnumerable<string> searchedLocations) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (searchedLocations == null) {
                throw new ArgumentNullException(nameof(searchedLocations));
            }
            return new HxlPageResult(name, searchedLocations);
        }

        public static HxlPageResult Found(string name, HxlPage page) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }
            if (page == null) {
                throw new ArgumentNullException(nameof(page));
            }
            return new HxlPageResult(name, page);
        }
    }
}
