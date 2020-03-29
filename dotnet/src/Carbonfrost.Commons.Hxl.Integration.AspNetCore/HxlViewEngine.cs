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
using System.Linq;

using Carbonfrost.Commons.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    public class HxlViewEngine : IHxlViewEngine {

        internal const string ViewFileExtension = ".hxl";
        private readonly HxlViewEngineOptions _options;
        private readonly IHxlViewResolver _resolver;

        public HxlViewEngine(IOptions<HxlViewEngineOptions> optionsAccessor) {
            if (optionsAccessor == null) {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            _options = optionsAccessor.Value;
            _resolver = new ProviderHxlViewResolver(_options);
        }

        public HxlPageResult FindPage(ActionContext context, string pageName) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }
            if (pageName == null) {
                throw Failure.Null(nameof(pageName));
            }
            if (string.IsNullOrEmpty(pageName)) {
                throw Failure.EmptyString(nameof(pageName));
            }
            // Find only works with names, not paths
            if (NameLooksLikePath(pageName)) {
                return HxlPageResult.NotFound(pageName, Enumerable.Empty<string>());
            }

            var cacheResult = _resolver.FindPageUsingViewLocations(context, pageName);
            if (cacheResult.Success) {
                var page = cacheResult.PageFactory();
                return HxlPageResult.Found(pageName, page);
            }

            return HxlPageResult.NotFound(pageName, cacheResult.SearchedLocations);
        }

        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }
            if (viewName == null) {
                throw Failure.Null(nameof(viewName));
            }
            if (string.IsNullOrEmpty(viewName)) {
                throw Failure.EmptyString(nameof(viewName));
            }

            // Find only works with names, not paths
            if (NameLooksLikePath(viewName)) {
                return ViewEngineResult.NotFound(viewName, Enumerable.Empty<string>());
            }

            var cacheResult = _resolver.FindPageUsingViewLocations(context, viewName);
            return CreateViewEngineResult(cacheResult, viewName);
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage) {
            if (viewPath == null) {
                throw Failure.Null(nameof(viewPath));
            }
            if (string.IsNullOrEmpty(viewPath)) {
                throw Failure.EmptyString(nameof(viewPath));
            }
            if (NameLooksLikePath(viewPath)) {
                var cacheResult = _resolver.FindPageUsingPath(executingFilePath, viewPath);
                return CreateViewEngineResult(cacheResult, viewPath);
            }

            return ViewEngineResult.NotFound(viewPath, Enumerable.Empty<string>());
        }

        private static bool NameLooksLikePath(string pageName) {
            return IsApplicationRelativePath(pageName) || LooksLikeFileName(pageName);
        }

        private static bool IsApplicationRelativePath(string name) {
            return name[0] == '~' || name[0] == '/';
        }

        private static bool LooksLikeFileName(string name) {
            return name.EndsWith(ViewFileExtension, StringComparison.OrdinalIgnoreCase);
        }

        private ViewEngineResult CreateViewEngineResult(ViewLocationCacheResult result, string viewName) {
            if (!result.Success) {
                return ViewEngineResult.NotFound(viewName, result.SearchedLocations);
            }

            var page = result.PageFactory();
            var view = new HxlView(this, page);
            return ViewEngineResult.Found(viewName, view);
        }
    }
}
