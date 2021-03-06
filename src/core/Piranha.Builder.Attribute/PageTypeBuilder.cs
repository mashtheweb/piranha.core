/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Piranha.Extend;

namespace Piranha.Builder.Attribute
{
    public class PageTypeBuilder : ContentTypeBuilder<PageTypeBuilder, PageType>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="logFactory">The optional log factory</param>
        public PageTypeBuilder(IApi api, ILoggerFactory logFactory = null) : base(api, logFactory) { }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public override void Build() {
            foreach (var type in types) {
                var pageType = GetContentType(type);

                if (pageType != null)
                    api.PageTypes.Save(pageType);
            }
            // Tell the app to reload the page types
            App.ReloadPageTypes(api);
        }

        #region Private methods
        /// <summary>
        /// Gets the possible page type for the given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The page type</returns>
        protected override PageType GetContentType(Type type) {
            var attr = type.GetTypeInfo().GetCustomAttribute<PageTypeAttribute>();

            if (attr != null) {
                logger?.LogInformation($"Importing PageType '{type.Name}'.");

                if (!string.IsNullOrEmpty(attr.Id) && !string.IsNullOrEmpty(attr.Title)) {
                    var pageType = new PageType() {
                        Id = attr.Id,
                        Title = attr.Title,
                        Route = attr.Route,
                        View = attr.View
                    };

                    foreach (var prop in type.GetTypeInfo().GetProperties(App.PropertyBindings)) {
                        var regionType = GetRegionType(prop);

                        if (regionType != null)
                            pageType.Regions.Add(regionType);
                    }
                    return pageType;
                } else {
                    logger?.LogError($"Id and/or Title is missing for PageType '{type.Name}'.");
                }
            } 
            return null;
        }
        #endregion
    }
}
