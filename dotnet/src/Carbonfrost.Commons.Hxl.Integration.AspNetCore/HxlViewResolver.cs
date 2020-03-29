//
// Copyright 2013-2014, 2020 Carbonfrost Systems, Inc. (https://carbonfrost.com)
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
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    abstract class HxlViewResolver : IHxlViewResolver {

        private readonly HxlViewEngineOptions _options;

        public HxlViewResolver(HxlViewEngineOptions options) {
            if (options == null) {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options;
        }

        protected abstract Func<HxlPage> TryCreateTemplate(string applicationRelativePath);

        public ViewLocationCacheResult FindPageUsingPath(string executingFilePath, string pagePath) {
            var applicationRelativePath = GetAbsolutePath(executingFilePath, pagePath);
            var result = TryCreateTemplate(applicationRelativePath);

            if (result == null) {
                return ViewLocationCacheResult.NotFound(new [] { applicationRelativePath });
            }

            return ViewLocationCacheResult.Found(result);
        }

        public ViewLocationCacheResult FindPageUsingViewLocations(
            ActionContext actionContext,
            string pageName
        ) {
            foreach (var l in FillLocations(actionContext)) {
                var result = TryCreateTemplate(l);
                if (result != null) {
                    return ViewLocationCacheResult.Found(result);
                }
            }

            return ViewLocationCacheResult.NotFound(FillLocations(actionContext));
        }

        private IEnumerable<string> FillLocations(ActionContext actionContext) {
            var controllerName = GetNormalizedRouteValue(actionContext, "controller");
            var areaName = GetNormalizedRouteValue(actionContext, "area");
            var actionName = GetNormalizedRouteValue(actionContext, "action");

            if (!string.IsNullOrEmpty(areaName)) {
                foreach (var area in _options.AreaViewLocationFormats) {
                    yield return string.Format(area, actionName, controllerName, areaName);
                }
            }

            foreach (var area in _options.ViewLocationFormats) {
                yield return string.Format(area, actionName, controllerName);
            }
        }

        public string GetAbsolutePath(string executingFilePath, string pagePath) {
            if (string.IsNullOrEmpty(pagePath)) {
                // Path is not valid; no change required.
                return pagePath;
            }

            if (IsApplicationRelativePath(pagePath)) {
                // An absolute path already; no change required.
                return pagePath;
            }

            if (!LooksLikeFileName(pagePath)) {
                // A page name; no change required.
                return pagePath;
            }

            // UNDONE Path navigation and munging won't work

            if (string.IsNullOrEmpty(executingFilePath)) {
                // Given a relative path i.e. not yet application-relative (starting with "~/" or "/"), interpret
                // path relative to currently-executing view, if any.
                // Not yet executing a view. Start in app root.
                var absolutePath = "/" + pagePath;
                return absolutePath;
            }

            return Path.Combine(executingFilePath, pagePath);
        }

        private static bool NameLooksLikePath(string pageName) {
            return IsApplicationRelativePath(pageName) || LooksLikeFileName(pageName);
        }

        private static bool IsApplicationRelativePath(string name) {
            return name[0] == '~' || name[0] == '/';
        }

        private static bool LooksLikeFileName(string name) {
            return name.EndsWith(HxlViewEngine.ViewFileExtension, StringComparison.OrdinalIgnoreCase);
        }

        static string GetNormalizedRouteValue(ActionContext context, string key) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            if (!context.RouteData.Values.TryGetValue(key, out var routeValue)) {
                return null;
            }

            var actionDescriptor = context.ActionDescriptor;
            string normalizedValue = null;

            if (actionDescriptor.RouteValues.TryGetValue(key, out var value) &&
                !string.IsNullOrEmpty(value)) {
                normalizedValue = value;
            }

            var stringRouteValue = Convert.ToString(routeValue, CultureInfo.InvariantCulture);
            if (string.Equals(normalizedValue, stringRouteValue, StringComparison.OrdinalIgnoreCase)) {
                return normalizedValue;
            }

            return stringRouteValue;
        }
    }
}
