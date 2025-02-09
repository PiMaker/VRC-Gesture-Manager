﻿using System.Collections.Generic;
using BlackStartX.GestureManager.Runtime.Extra;
using UnityEngine;

namespace BlackStartX.GestureManager
{
    public class GestureManager : MonoBehaviour
    {
        public static readonly Dictionary<GameObject, ModuleBase> ControlledAvatars = new Dictionary<GameObject, ModuleBase>();
        public static List<ModuleBase> LastCheckedActiveModules = new List<ModuleBase>();
        public static bool InWebClientRequest;

        private TransformData _managerTransform;
        private bool _drag;

        public ModuleBase Module;

        private void OnDisable() => UnlinkModule();

        private void OnDrawGizmos() => Module?.OnDrawGizmos();

        private void Update()
        {
            if (Module == null) return;
            if (Module.IsInvalid()) UnlinkModule();
            else ModuleUpdate();
        }

        private void ModuleUpdate()
        {
            if (_drag) _managerTransform.Difference(transform).AddTo(Module.Avatar.transform);
            _managerTransform = new TransformData(transform);
            Module.Update();
        }

        private void LateUpdate()
        {
            _managerTransform = new TransformData(transform);
            Module?.LateUpdate();
        }

        public void SetDrag(bool drag) => _drag = drag;

        public void UnlinkModule() => SetModule(null);

        public void SetModule(ModuleBase module)
        {
            if (Module != null)
            {
                Module.Unlink();
                Module.Active = false;
                ControlledAvatars.Remove(Module.Avatar);
            }

            Module = module;
            Module?.Avatar.transform.ApplyTo(transform);
            if (Module == null) return;

            Module.Active = true;
            Module.InitForAvatar();
            ControlledAvatars[module.Avatar] = module;
            _managerTransform = new TransformData(transform);
        }
    }
}