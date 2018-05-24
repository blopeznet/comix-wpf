using System;
using System.IO;
using System.Reflection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Responses;
using Nancy.Session;
using Nancy.TinyIoc;
using Nancy.ViewEngines;

namespace DirectoryBrowser.WebServer
{
	public class CustomBootstrapper : DefaultNancyBootstrapper
	{
        /// <summary>
        /// Replace with your App Assembly Name
        /// </summary>
        internal string InitAssembly = "DirectoryBrowser.WebServer.";

        #region favicon

        private byte[] favicon;

        protected override byte[] FavIcon
        {
            get { return this.favicon ?? (this.favicon = LoadFavIcon()); }
        }

        private byte[] LoadFavIcon()
        {
            //TODO: remember to replace 'AssemblyName' with the prefix of the resource
            using (var resourceStream = GetType().Assembly.GetManifestResourceStream(InitAssembly+ "Resources.Mattahan-Ultrabuuf-Comics-Ironman-Folder.ico"))
            {
                var memoryStream = new MemoryStream();
                resourceStream.CopyTo(memoryStream);
                return memoryStream.GetBuffer();
            }
        }

        #endregion

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            ResourceViewLocationProvider.RootNamespaces.Add(Assembly.GetAssembly(typeof(MainModule)), InitAssembly+"Views");

        }

		protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
		{
			base.ApplicationStartup(container, pipelines);
			CookieBasedSessions.Enable(pipelines);
		}

		protected override void ConfigureConventions(NancyConventions conventions)
		{
			base.ConfigureConventions(conventions);
			conventions.StaticContentsConventions.Add(AddStaticResourcePath("/content", Assembly.GetAssembly(typeof(MainModule)), InitAssembly + "Views.content"));
		}        

		void OnConfigurationBuilder(NancyInternalConfiguration x)
		{
			x.ViewLocationProvider = typeof(ResourceViewLocationProvider);
		}

		public static Func<NancyContext, string, Response> AddStaticResourcePath(string requestedPath, Assembly assembly, string namespacePrefix)
		{
			return (context, s) =>
			       	{
			       		var path = context.Request.Path;
						if (!path.StartsWith(requestedPath))
						{
							return null;
						}

						string resourcePath;
						string name;

						var adjustedPath = path.Substring(requestedPath.Length + 1);
						if (adjustedPath.IndexOf('/') >= 0)
						{
							name = Path.GetFileName(adjustedPath);
							resourcePath = namespacePrefix + "." + adjustedPath.Substring(0, adjustedPath.Length - name.Length - 1).Replace('/', '.');
						}
						else
						{
							name = adjustedPath;
							resourcePath = namespacePrefix;
						}
						return new EmbeddedFileResponse(assembly, resourcePath, name);
			       	};
		}
	}
}