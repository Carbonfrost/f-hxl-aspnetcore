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

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    internal class HxlMvcViewOptionsSetup : IConfigureOptions<MvcViewOptions> {

        private readonly IHxlViewEngine _hxlViewEngine;

        public HxlMvcViewOptionsSetup(IHxlViewEngine hxlViewEngine) {
            if (hxlViewEngine == null) {
                throw new ArgumentNullException(nameof(hxlViewEngine));
            }

            _hxlViewEngine = hxlViewEngine;
        }

        public void Configure(MvcViewOptions options) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }

            options.ViewEngines.Add(_hxlViewEngine);
        }
    }
}
