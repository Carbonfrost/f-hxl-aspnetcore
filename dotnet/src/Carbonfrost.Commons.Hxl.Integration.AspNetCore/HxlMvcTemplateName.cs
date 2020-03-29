//
// Copyright 2015, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Linq;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    internal class HxlMvcTemplateName {

        // type:area.controller.action

        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Type { get; set; }

        public static HxlMvcTemplateName Parse(string text) {
            text = (text ?? string.Empty).Trim();
            if (text.Length == 0) {
                return new HxlMvcTemplateName();
            }

            var result = new HxlMvcTemplateName();
            var parts = text.Split(':');
            if (parts.Length > 2) {
                throw new FormatException();
            }
            result.Type = parts[0].Trim();

            // Handle the controller
            parts = parts.Last().Split('.')
                .Select(t => t.Trim())
                .ToArray();
            if (parts.Length > 3 || parts.Any(p => p.Length == 0)) {
                throw new FormatException();
            }

            int index = parts.Length - 1;
            result.Action = parts[index--].Trim();

            if (index >= 0) {
                result.Controller = parts[index--].Trim();

                if (index >= 0) {
                    result.Area = parts[index].Trim();
                }
            }

            return result;
        }
    }
}
