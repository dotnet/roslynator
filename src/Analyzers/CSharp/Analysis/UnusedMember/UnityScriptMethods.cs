// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;

namespace Roslynator.CSharp.Analysis.UnusedMember
{
    internal static class UnityScriptMethods
    {
        private static ImmutableHashSet<string> _methodNames;

        public static MetadataName MonoBehaviourClassName { get; } = MetadataName.Parse("UnityEngine.MonoBehaviour");

        public static ImmutableHashSet<string> MethodNames
        {
            get
            {
                if (_methodNames == null)
                    Interlocked.CompareExchange(ref _methodNames, LoadMethodNames(), null);

                return _methodNames;
            }
        }

        private static ImmutableHashSet<string> LoadMethodNames()
        {
            return ImmutableHashSet.CreateRange(new[] {
                "Awake",
                "FixedUpdate",
                "LateUpdate",
                "OnAnimatorIK",
                "OnAnimatorMove",
                "OnApplicationFocus",
                "OnApplicationPause",
                "OnApplicationQuit",
                "OnAudioFilterRead",
                "OnBecameInvisible",
                "OnBecameVisible",
                "OnCollisionEnter",
                "OnCollisionEnter2D",
                "OnCollisionExit",
                "OnCollisionExit2D",
                "OnCollisionStay",
                "OnCollisionStay2D",
                "OnConnectedToServer",
                "OnControllerColliderHit",
                "OnDestroy",
                "OnDisable",
                "OnDisconnectedFromServer",
                "OnDrawGizmos",
                "OnDrawGizmosSelected",
                "OnEnable",
                "OnFailedToConnect",
                "OnFailedToConnectToMasterServer",
                "OnGUI",
                "OnJointBreak",
                "OnJointBreak2D",
                "OnMasterServerEvent",
                "OnMouseDown",
                "OnMouseDrag",
                "OnMouseEnter",
                "OnMouseExit",
                "OnMouseOver",
                "OnMouseUp",
                "OnMouseUpAsButton",
                "OnNetworkInstantiate",
                "OnParticleCollision",
                "OnParticleSystemStopped",
                "OnParticleTrigger",
                "OnParticleUpdateJobScheduled",
                "OnPlayerConnected",
                "OnPlayerDisconnected",
                "OnPostRender",
                "OnPreCull",
                "OnPreRender",
                "OnRenderImage",
                "OnRenderObject",
                "OnSerializeNetworkView",
                "OnServerInitialized",
                "OnTransformChildrenChanged",
                "OnTransformParentChanged",
                "OnTriggerEnter",
                "OnTriggerEnter2D",
                "OnTriggerExit",
                "OnTriggerExit2D",
                "OnTriggerStay",
                "OnTriggerStay2D",
                "OnValidate",
                "OnWillRenderObject",
                "Reset",
                "Start",
                "Update",
            });
        }
    }
}
