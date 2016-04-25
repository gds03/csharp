using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using System.Xml;

namespace VirtualNote.MVC.Bootstrapper.SiteMap
{
    public class MvcSiteMapProvider : StaticSiteMapProvider
    {
        // Sitemap attributes which are not considered part of route,
        // you can add your custom attributes here.
        private readonly string[] _excludedAttributes = { "title", "description", "roles" };

        private RequestContext _requestContext;
        private SiteMapNode _rootNode;

        //
        // Default Values
        private String _siteMapPath = "~/Web.sitemap";


        // Initialize data structure by reading from xml file
        public override void Initialize(string name, NameValueCollection attributes)
        {
            // If location of sitemap is specified in provider attributes
            if (!string.IsNullOrEmpty(attributes["siteMapFile"]))
            {
                // Set specified sitemap location
                _siteMapPath = attributes["siteMapFile"];
            }

            // Load sitemap file
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(HostingEnvironment.MapPath(_siteMapPath));

            // Create new requestcontext, needed to resolve url's from route values. 
            // Current http context is used, route data is cleared.
            _requestContext = new RequestContext(
                new HttpContextWrapper(HttpContext.Current),
                new RouteData()
            );

            // Get nodes recursively
            XmlElement rootElement;

            if ((rootElement = xmlDocument.DocumentElement) == null)
                throw new InvalidOperationException();

            // First child is the sitemap element
            _rootNode = MakeFromNodeRecursively(rootElement, null);

            // Call base because
            // the default implementation initializes the SecurityTrimmingEnabled property for the site map
            // provider from the site navigation configuration. 
            base.Initialize(name, attributes);
        }

        public override SiteMapNode BuildSiteMap()
        {
            // Return rootnode which contains all childnodes
            return _rootNode;
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return _rootNode;
        }

        private SiteMapNode MakeFromNodeRecursively(XmlNode xmlNode, SiteMapNode parentNode)
        {
            // Create collection of custom sitemap attributes
            var attributes = new NameValueCollection();


            var routeData = new Dictionary<string, object>();

            foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
            {
                // Lookup for attributes candidates for routeData
                if (!_excludedAttributes.Contains(xmlAttribute.Name))
                {
                    routeData.Add(xmlAttribute.Name, xmlAttribute.Value);
                }

                // Add each attribute to attributes
                attributes[xmlAttribute.Name] = xmlAttribute.Value;
            }

            // Create route dictionary
            var routeDict = new RouteValueDictionary(routeData);

            // Resolve relative url from route values (from xml attributes)
            // Only resolve url when routevalues are specified (avoids creating "/" as url when no route values are specified)
            // Every url in a sitemap must be unique (except when empty), so if mulitple url's were to be set to '/' this would result in an exception
            string url = string.Empty;

            if (routeData.Count > 0)
            {
                url = RouteTable.Routes.GetVirtualPath(_requestContext, routeDict).VirtualPath;
            }

            // Store collection of route attribute keys in one custom sitemap attribute
            attributes.Add("RouteKeys", string.Join(",", routeData.Keys.ToArray()));

            // Get a list of roles 
            List<string> roles = attributes["roles"] == null ? null
                                                             : attributes["roles"].Replace(" ", "")
                                                                                  .Split(',')
                                                                                  .ToList();

            // Create new sitemap node
            SiteMapNode node = new SiteMapNode(this,
                                               Guid.NewGuid().ToString(),
                                               url,
                                               attributes["title"] ?? "",
                                               attributes["description"] ?? "",
                                               roles,
                                               attributes,
                                               null, null);

            // Add sitemapnode
            base.AddNode(node, parentNode); // For the level parentNode is null, for all subsequents parent is != null

            // Iterate through children
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                // Add children to sitemap
                MakeFromNodeRecursively(childNode, node);
            }

            return node;
        }

        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            if (!SecurityTrimmingEnabled)
                return true;

            // If node has no roles specified means it is accessible (unless parent is not accessible)
            if (node.Roles == null)
                return true;

            return node.Roles.Cast<string>().Any(role => context.User.IsInRole(role));
        }
    }
}