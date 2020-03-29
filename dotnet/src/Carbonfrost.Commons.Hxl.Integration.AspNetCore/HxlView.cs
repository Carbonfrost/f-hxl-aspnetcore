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

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Carbonfrost.Commons.Core.Runtime;
using Carbonfrost.Commons.Instrumentation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.CSharp.RuntimeBinder;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    public class HxlView : IView {

        private readonly IHxlViewEngine _viewEngine;
        private readonly HxlPage _page;

        public HxlView(IHxlViewEngine viewEngine, HxlPage page) {
            if (viewEngine == null) {
                throw new ArgumentNullException(nameof(viewEngine));
            }
            if (page == null) {
                throw new ArgumentNullException(nameof(page));
            }

            _viewEngine = viewEngine;
            _page = page;
        }

        public string Path {
            get {
                return _page.Path;
            }
        }

        public async Task RenderAsync(ViewContext context) {
            await CoreRenderAsync(Path, _page, context, context.Writer);
        }

        private static void SetupForMvc(HxlPage page, string viewPath, ViewContext viewContext) {
            page.Path = viewPath;
            page.ViewContext = viewContext;
            page.Url = new UrlHelper(viewContext);
        }

        internal static async Task CoreRenderAsync(
            string viewPath,
            HxlPage page,
            ViewContext viewContext,
            TextWriter writer
        ) {
            Traceables.HxlViewRendering(viewPath, page);
            Traceables.HxlViewDebugRendering(viewContext);
            SetupForMvc(page, viewPath, viewContext);

            IProfilerScope ps = Metrics.StartViewRender(viewPath);
            bool success = false;

            try {
                var tc = new HxlTemplateContext(page);
                if (viewContext.ViewData?.Model != null) {
                    tc.Data.Add("model", viewContext.ViewData.Model);
                }
                tc.DataProviders.Add(new IgnoreNullValuesProvider(
                    new DynamicProvider(viewContext.ViewBag)
                ));
                tc.DataProviders.AddMany(HxlConfiguration.Current.GlobalDataProviders);

                // Evaluate the complete response before trying to write it.
                // This ensures that exceptions can be handled by middleware.
                var bufferWriter = new StringWriter();
                await page.TransformAsync(bufferWriter, tc);

                success = true;

                await writer.WriteAsync(bufferWriter.ToString());

            } catch (Exception ex) {
                Traceables.HxlViewFailedToRender(viewPath, ex);
                throw;

            } finally {
                ps.EndViewRender(success);
            }
        }

        class IgnoreNullValuesProvider : IPropertyProvider {

            private readonly IPropertyProvider _pp;

            public IgnoreNullValuesProvider(IPropertyProvider pp) {
                _pp = pp;
            }

            public Type GetPropertyType(string property) {
                return _pp.GetPropertyType(property);
            }

            public bool TryGetProperty(string property, Type propertyType, out object value) {
                // Sometimes a property provider returns true and sets value = null even when the
                // property does not exist.  If we compose this property provider with others,
                // then it might always win and cause null to be returned.  To stop this, return
                // false on null values.
                bool result = _pp.TryGetProperty(property, propertyType, out value);
                return result && value != null;
            }
        }

        class DynamicProvider : IPropertyProvider {
            // TODO Drop this implementation when it is available to f-core

            private readonly object _provider;

            public DynamicProvider(dynamic viewBag) {
                _provider = viewBag;
            }

            public Type GetPropertyType(string property) {
                return null;
            }

            public bool TryGetProperty(string property, Type propertyType, out object value) {
                var binder = Binder.GetMember(
                    CSharpBinderFlags.None,
                    property,
                    _provider.GetType(),
                    new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }
                );
                var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
                try {
                    value = callsite.Target(callsite, _provider);
                    return true;

                } catch (RuntimeBinderException) {
                    value = null;
                    return false;
                }
            }
        }
    }
}
