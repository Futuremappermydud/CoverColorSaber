using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRUIControls;

namespace CoverColorSaber
{
    //eh
    public class PointerTracker : MonoBehaviour
    {
        public bool shouldTrack;
        public VRPointer pointer;
        public Action<Vector2> HitTexCoord;
        public string objectGoal;
        public void Track(VRPointer pointer, string goal)
        {
            shouldTrack = true;
            this.pointer = pointer;
            objectGoal = goal;
        }

        public void StopTrack()
        {
            shouldTrack = false;
        }
        private void LateUpdate()
        {
            if (!shouldTrack) return;
            if (pointer.vrController.triggerValue < 0.1 && !Input.GetMouseButton(0)) return;
            RaycastHit hit;
            if (!Physics.Raycast(pointer.vrController.position, pointer.vrController.transform.TransformDirection(Vector3.forward), out hit))
                return;
            if (hit.transform.gameObject.name != objectGoal) return;
            Renderer rend = hit.transform.GetComponent<Renderer>();
            MeshCollider meshCollider = hit.collider as MeshCollider;

            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                return;
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
            HitTexCoord.Invoke(pixelUV);
        }
    }
}
