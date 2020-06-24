﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XUnity.Common.Utilities;

namespace XUnity.AutoTranslator.Plugin.Core.Shims
{
   public static class CoroutineHelper
   {
      private static ICoroutineHelper _instance;

      public static ICoroutineHelper Instance
      {
         get
         {
            if( _instance == null )
            {
               _instance = ActivationHelper.Create<ICoroutineHelper>(
                  "XUnity.AutoTranslator.Plugin.Core.Shims.ManagedPathsHelper, XUnity.AutoTranslator.Plugin.Core.Managed.dll",
                  "XUnity.AutoTranslator.Plugin.Core.Shims.Il2CppPathsHelper, XUnity.AutoTranslator.Plugin.Core.IL2CPP.dll" );
            }
            return _instance;
         }
      }
   }

   public interface ICoroutineHelper
   {
      object Start( IEnumerator coroutine );

      void Stop( object coroutine );

      object CreateWaitForSeconds( float seconds );

      IEnumerator CreateWaitForSecondsRealtime( float seconds );

      bool SupportsCustomYieldInstruction();
   }
}
