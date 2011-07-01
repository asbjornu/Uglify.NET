using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Uglify
{
   /// <summary>
   /// Thrown when <see cref="M:Requirer.Require"/> experiences a problem.
   /// </summary>
   [Serializable]
   public class RequireException : Exception
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="RequireException"/> class.
      /// </summary>
      /// <param name="file">The file.</param>
      /// <param name="innerException">The inner exception.</param>
      public RequireException(string file, Exception innerException)
         : base(CreateMessage(file), TraverseException(innerException))
      {
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="RequireException"/> class.
      /// </summary>
      /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
      /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
      /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
      /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
      protected RequireException(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
      }


      private static string CreateMessage(string file)
      {
         return String.Format("An error occurred while requiring '{0}'.", file);
      }


      private static Exception TraverseException(Exception exception)
      {
         return (exception != null)
                && (exception is TargetInvocationException)
                && (exception.InnerException != null)
                   ? exception.InnerException
                   : exception;
      }
   }
}