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

using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.CSharp.RuntimeBinder;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    public abstract class HxlPage : HxlTemplateExtension {

        public HttpRequest Request {
            get {
                return HttpContext?.Request;
            }
        }

        public HttpResponse Response {
            get {
                return HttpContext?.Response;
            }
        }

        public string Path {
            get;
            set;
        }

        public IUrlHelper Url {
            get;
            set;
        }

        public ViewContext ViewContext {
            get;
            set;
        }

        public HttpContext HttpContext {
            get {
                return ViewContext?.HttpContext;
            }
        }

        public ITempDataDictionary TempData {
            get {
                return ViewContext?.TempData;
            }
        }

        public dynamic ViewBag {
            get {
                return ViewContext?.ViewBag;
            }
        }

        public virtual ClaimsPrincipal User {
            get {
                return HttpContext?.User;
            }
        }

        public RouteData RouteData {
            get {
                return ViewContext.RouteData;
            }
        }

        public ModelStateDictionary ModelState {
            get {
                return ViewContext?.ModelState;
            }
        }

        protected HxlPage() {}

        protected override void InitializeCore() {
            base.InitializeCore();

            Url = (IUrlHelper) HttpContext.RequestServices.GetService(typeof(IUrlHelper));
            TemplateContext.TemplateFactory = new HxlMvcTemplateFactory();
        }
    }
}
