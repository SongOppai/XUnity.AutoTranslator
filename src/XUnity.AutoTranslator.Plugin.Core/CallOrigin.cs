﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using XUnity.AutoTranslator.Plugin.Core.Extensions;
using XUnity.AutoTranslator.Plugin.Core.Utilities;
using XUnity.Common.Constants;
using XUnity.Common.Logging;

namespace XUnity.AutoTranslator.Plugin.Core
{
   internal static class CallOrigin
   {
      public static bool ExpectsTextToBeReturned = false;
      public static IReadOnlyTextTranslationCache TextCache = null;

      private static readonly HashSet<Assembly> BreakingAssemblies;

      static CallOrigin()
      {
         BreakingAssemblies = new HashSet<Assembly>();
         try
         {
            BreakingAssemblies.AddRange(
               AppDomain.CurrentDomain
                  .GetAssemblies()
                  .Where( x => x.GetName().Name.Equals( "Assembly-CSharp" ) || x.GetName().Equals( "Assembly-CSharp-firstpass" ) )
               );
         }
         catch( Exception e )
         {
            XuaLogger.AutoTranslator.Error( e, "An error occurred while scanning for game assemblies." );
         }
      }

      public static IReadOnlyTextTranslationCache GetTextCache( TextTranslationInfo info, TextTranslationCache generic )
      {
         if( info != null )
         {
            return info.TextCache ?? generic;
         }
         else
         {
            return TextCache ?? generic;
         }
      }

      public static void SetTextCacheForAllObjectsInHierachy( GameObject go, IReadOnlyTextTranslationCache cache )
      {
         try
         {
            foreach( var comp in go.GetAllTextComponentsInChildren() )
            {
               var info = comp.GetOrCreateTextTranslationInfo();
               info.TextCache = cache;
            }
         }
         catch( Exception e )
         {
            XuaLogger.AutoTranslator.Error( e, "An error occurred while scanning object hierarchy for text components." );
         }
      }

      public static IReadOnlyTextTranslationCache CalculateTextCacheFromStackTrace()
      {
         try
         {
            var trace = new StackTrace( 2 );
            var caches = AutoTranslationPlugin.Current.PluginTextCaches;
            var frames = trace.GetFrames();
            var len = frames.Length;
            for( int i = 0; i < len; i++ )
            {
               var frame = frames[ i ];
               var method = frame.GetMethod();
               if( method != null )
               {
                  var type = method.DeclaringType;
                  var assembly = type.Assembly;
                  if( BreakingAssemblies.Contains( assembly ) )
                     break;

                  var name = assembly.GetName().Name;
                  if( caches.TryGetValue( name, out var tc ) )
                  {
                     var translationCache = AutoTranslationPlugin.Current.TextCache.GetOrCreateCompositeCache( tc );
                     return translationCache;
                  }
               }
            }
         }
         catch( Exception e )
         {
            XuaLogger.AutoTranslator.Error( e, "An error occurred while calculating text translation cache from stack trace." );
         }

         return null;
      }
   }
}
