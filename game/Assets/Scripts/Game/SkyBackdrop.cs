using UnityEngine;
using UnityEngine.Rendering;

namespace Saga.Game
{
    // The sky as a flat picture behind the battle, instead of a 3D skybox. The 2017 skies are flat 2:1
    // wallpapers (an Earth view, nebulae), not seamless 360-degree panoramas: wrapped round a sphere they
    // stretched badly under a strategy camera and showed a seam. Here each fills the screen at its own shape
    // (cropped, never stretched) and slides a little within its margins as the camera turns and pans.
    public class SkyBackdrop : MonoBehaviour
    {
        const float Zoom = 0.86f;            // share of the image shown along its tighter side (the rest is slide room)
        const float PanRange = 60000f;       // camera travel that slides the image from one margin to the other

        Camera cam;
        Material mat;
        Transform quad;

        public static SkyBackdrop Create(Camera cam, Material template, Texture2D tex, float exposure)
        {
            var go = new GameObject("Sky backdrop");
            var b = go.AddComponent<SkyBackdrop>();
            b.cam = cam;
            var q = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Object.DestroyImmediate(q.GetComponent<Collider>());
            q.name = "sky";
            q.transform.SetParent(cam.transform, false);
            b.quad = q.transform;
            b.mat = template != null ? new Material(template) : new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            b.mat.SetTexture("_BaseMap", tex);
            b.mat.SetColor("_BaseColor", new Color(exposure, exposure, exposure, 1f));
            b.mat.renderQueue = (int)RenderQueue.Background;
            var r = q.GetComponent<MeshRenderer>();
            r.sharedMaterial = b.mat;
            r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false;
            b.Refresh();
            return b;
        }

        void LateUpdate() => Refresh();

        public void Refresh()
        {
            if (cam == null || quad == null) return;
            // Fill the view just inside the far plane.
            float d = cam.farClipPlane * 0.97f;
            float h = 2f * d * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float w = h * cam.aspect;
            quad.localPosition = new Vector3(0, 0, d);
            quad.localRotation = Quaternion.identity;
            quad.localScale = new Vector3(w * 1.02f, h * 1.02f, 1f);

            // Crop to the screen's shape ("cover"), keeping the image's proportions.
            var tex = mat.GetTexture("_BaseMap");
            float imgAspect = tex != null ? (float)tex.width / tex.height : 2f;
            float uSpan, vSpan;
            if (cam.aspect > imgAspect) { uSpan = Zoom; vSpan = Zoom * imgAspect / cam.aspect; }
            else { vSpan = Zoom; uSpan = Zoom * cam.aspect / imgAspect; }

            // Slide within the margins: turning the camera and panning move it gently, never past an edge.
            var p = cam.transform.position;
            float yaw = cam.transform.eulerAngles.y * Mathf.Deg2Rad;
            float sx = Mathf.Clamp01(0.5f + 0.3f * Mathf.Sin(yaw) + p.x / PanRange);
            float sy = Mathf.Clamp01(0.5f + 0.15f * Mathf.Cos(yaw) + p.z / PanRange);
            mat.SetTextureScale("_BaseMap", new Vector2(uSpan, vSpan));
            mat.SetTextureOffset("_BaseMap", new Vector2((1f - uSpan) * sx, (1f - vSpan) * sy));
        }
    }
}
