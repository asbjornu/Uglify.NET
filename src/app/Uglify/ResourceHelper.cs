using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Uglify
{
   public class ResourceHelper
   {
      private readonly Assembly assembly;
      private readonly string[] embeddedResources;


      public ResourceHelper()
      {
         this.assembly = typeof(ResourceHelper).Assembly;
         this.embeddedResources = this.assembly.GetManifestResourceNames();
      }


      public string Get(string resourceName)
      {
         if (String.IsNullOrEmpty(resourceName))
            throw new ArgumentNullException("resourceName");

         string fullyQualifiedResourceName = String
            .Concat(GetType().Namespace, ".UglifyJS.", resourceName)
            .Replace("./", String.Empty)
            .Replace('/', '.')
            .Replace('\\', '.');

         if (Array.IndexOf(this.embeddedResources, fullyQualifiedResourceName) == -1)
            throw new MissingManifestResourceException(String.Format("Can't find '{0}'.", resourceName));

         using (var stream = this.assembly.GetManifestResourceStream(fullyQualifiedResourceName))
         {
            if (stream == null)
               return null;

            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
               return streamReader.ReadToEnd();
            }
         }
      }
   }
}