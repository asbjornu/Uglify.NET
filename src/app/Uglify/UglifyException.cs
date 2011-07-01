using System;
using System.Runtime.Serialization;

namespace Uglify
{
   /// <summary>
   /// Thrown when <see cref="M:Uglifier.Uglify"/> experiences a problem.
   /// </summary>
   [Serializable]
   public class UglifyException : ApplicationException
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="UglifyException"/> class.
      /// </summary>
      /// <param name="code">The code that couldn't be uglified.</param>
      /// <param name="innerException">The inner exception.</param>
      public UglifyException(string code, Exception innerException)
         : base(CreateMessage(code), innerException)
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="UglifyException"/> class.
      /// </summary>
      /// <param name="info">The object that holds the serialized object data.</param>
      /// <param name="context">The contextual information about the source or destination.</param>
      protected UglifyException(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
      }

      
      private static string CreateMessage(string code)
      {
         return String.Concat("The following code couldn't be uglified: \n", code);
      }
   }
}