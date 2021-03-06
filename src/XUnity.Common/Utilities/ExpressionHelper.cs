﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace XUnity.Common.Utilities
{
   /// <summary>
   /// WARNING: Pubternal API (internal). Do not use. May change during any update.
   /// </summary>
   public static class ExpressionHelper
   {
      /// <summary>
      /// WARNING: Pubternal API (internal). Do not use. May change during any update.
      /// </summary>
      /// <param name="method"></param>
      /// <returns></returns>
      public static Delegate CreateTypedFastInvoke( MethodInfo method )
      {
         if( method == null ) throw new ArgumentNullException( "method" );
         if( method.IsGenericMethod ) throw new ArgumentException( "The provided method must not be generic.", "method" );

         if( method.IsStatic )
         {
            var parameters = method.GetParameters()
               .Select( p => Expression.Parameter( p.ParameterType, p.Name ) )
               .ToArray();

            var call = Expression.Call( null, method, parameters );

            return Expression.Lambda( call, parameters ).Compile();
         }
         else
         {
            var parameters = method.GetParameters()
               .Select( p => Expression.Parameter( p.ParameterType, p.Name ) )
               .ToList();

            parameters.Insert( 0, Expression.Parameter( method.DeclaringType, "instance" ) );

            var call = Expression.Call( parameters[ 0 ], method, parameters.Skip( 1 ).ToArray() );

            return Expression.Lambda( call, parameters.ToArray() ).Compile();
         }
      }
   }
}
