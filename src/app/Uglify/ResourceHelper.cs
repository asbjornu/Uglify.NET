using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Uglify
{
   /// <summary>
   /// Utility class to help retrieving embedded resources from the assembly.
   /// </summary>
   internal class ResourceHelper
   {
      private readonly Assembly assembly;
      private readonly string[] embeddedResources;


      /// <summary>
      /// Initializes a new instance of the <see cref="ResourceHelper"/> class.
      /// </summary>
      public ResourceHelper()
      {
         this.assembly = typeof(ResourceHelper).Assembly;
         this.embeddedResources = this.assembly.GetManifestResourceNames();
      }


      /// <summary>
      /// Gets the contents of the specified resource.
      /// </summary>
      /// <param name="resourceName">The name of the resource.</param>
      /// <returns>
      /// The contents of the specified resource.
      /// </returns>
      public string Get(string resourceName)
      {
         if (String.IsNullOrEmpty(resourceName))
            throw new ArgumentNullException("resourceName");

         string fullyQualifiedResourceName = String
            // Build the resource name.
            .Concat(GetType().Namespace, ".UglifyJS.", resourceName)
            // Replace all slashes with dots as that's how resource names are constructed.
            .Replace('/', '.')
            // Replace any backslashes with dots as that's how resource names are constructed.
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