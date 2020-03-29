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

using System.Collections.Generic;
using Carbonfrost.Commons.Core.Runtime;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    public abstract class HxlPage<TModel> : HxlPage {

        public IHtmlHelper<TModel> Html {
            get;
            private set;
        }

        public TModel Model {
            get {
                if (ViewData == null) {
                    return default(TModel);
                }
                return ViewData.Model;
            }
        }

        public ViewDataDictionary<TModel> ViewData {
            get;
            set;
        }

        protected override void InitializeCore() {
            base.InitializeCore();

            Html = (IHtmlHelper<TModel>) HttpContext.RequestServices.GetService(typeof(IHtmlHelper<TModel>));
            TemplateContext.Data.AddMany(ViewData);

            if (Model != null) {
                TemplateContext.DataProviders.Add(PropertyProvider.FromValue(Model));
            }
        }
    }
}
