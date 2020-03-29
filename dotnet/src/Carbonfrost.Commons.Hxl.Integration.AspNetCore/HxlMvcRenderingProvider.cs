//
// Copyright 2014, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    class HxlMvcRenderingProvider : IHxlRenderingProvider {

        private readonly IHtmlHelper _html;

        public HxlMvcRenderingProvider(IHtmlHelper html) {
            _html = html;
        }

        public Task RenderPartialAsync(TextWriter writer,
                                       string partialViewName,
                                       HxlElementTemplateInfo templateInfo,
                                       object model,
                                       ViewDataDictionary viewData) {
            var viewData2 = new ViewDataDictionary(_html.MetadataProvider, null);
            viewData2.AddMany(viewData);

            _html.ViewContext.Writer = writer;
            _html.ViewContext.RouteData.Values.SetElementTemplateInfo(templateInfo);
            return _html.RenderPartialAsync(partialViewName, model, viewData2);
        }
    }
}
