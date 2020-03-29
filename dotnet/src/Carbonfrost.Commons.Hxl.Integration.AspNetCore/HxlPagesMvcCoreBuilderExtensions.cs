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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Carbonfrost.Commons.Hxl.Integration.AspNetCore {

    public static class HxlPagesMvcCoreBuilderExtensions {

        public static IMvcCoreBuilder AddHxlPages(this IMvcCoreBuilder builder) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddHxlViewEngine();
            AddHxlPagesServices(builder.Services);
            return builder;
        }

        public static IMvcCoreBuilder AddHxlViewEngine(this IMvcCoreBuilder builder) {
            if (builder == null) {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddViews();
            AddHxlViewEngineServices(builder.Services);
            return builder;
        }

        internal static void AddHxlPagesServices(IServiceCollection services) {
        }

        internal static void AddHxlViewEngineServices(IServiceCollection services) {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcViewOptions>, HxlMvcViewOptionsSetup>()
            );

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<HxlViewEngineOptions>, HxlViewEngineOptionsSetup>()
            );

            services.TryAddSingleton<IHxlViewEngine, HxlViewEngine>();
        }
    }
}
