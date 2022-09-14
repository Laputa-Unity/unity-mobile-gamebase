#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabPreviewGenerator
    {
        private Color _backgroundColor = new Color(0.321568638f, 0.321568638f, 0.321568638f, 1f);
        private int _previewWidth = 128;
        private int _previewHeight = 128;
        private bool _isPreiewGenSessionActive;

        private Dictionary<Light, bool> _sceneLightToActiveState = new Dictionary<Light, bool>();

        [SerializeField]
        private Camera _renderCamera;
        [SerializeField]
        public Light[] _previewLights;

        public static PrefabPreviewGenerator Get() { return Octave3DWorldBuilder.ActiveInstance.PrefabPreviewGenerator; }

        public void BeginPreviewGenSession()
        {
            if (_isPreiewGenSessionActive) return;

            _sceneLightToActiveState.Clear();
            List<Light> sceneLights = Octave3DScene.Get().GetSceneLights();
            foreach (var light in sceneLights)
            {
                if (light == null) continue;
                _sceneLightToActiveState.Add(light, light.gameObject.activeSelf);
                light.gameObject.SetActive(false);
            }

            _isPreiewGenSessionActive = true;
        }

        public void EndPreviewGenSession()
        {
            if (!_isPreiewGenSessionActive) return;

            foreach (var pair in _sceneLightToActiveState)
            {
                if (pair.Key == null) continue;
                pair.Key.gameObject.SetActive(pair.Value);
            }
            _sceneLightToActiveState.Clear();

            _isPreiewGenSessionActive = false;
        }

        public Texture2D GeneratePreview(Prefab prefab)
        {
            if (prefab == null || prefab.UnityPrefab == null || !_isPreiewGenSessionActive) return null;

            RenderTexture renderTexture = GetRenderTexture();
            if (renderTexture == null) return null;

            Camera renderCam = GetRenderCamera();
            renderCam.backgroundColor = _backgroundColor;
            renderCam.orthographic = false;
            renderCam.fieldOfView = 65.0f;
            renderCam.targetTexture = renderTexture;
            renderCam.clearFlags = CameraClearFlags.Color;
            renderCam.nearClipPlane = 0.0001f;

            RenderTexture oldRenderTexture = UnityEngine.RenderTexture.active;
            RenderTexture.active = renderCam.targetTexture;
            GL.Clear(true, true, _backgroundColor);

            GameObject previewObjectRoot = GameObject.Instantiate(prefab.UnityPrefab);
            previewObjectRoot.hideFlags = HideFlags.HideAndDontSave;
            Transform previewObjectTransform = previewObjectRoot.transform;
            previewObjectTransform.position = Vector3.zero;
            previewObjectTransform.rotation = Quaternion.identity;
            previewObjectTransform.localScale = prefab.InitialWorldScale;

            Box worldBox = previewObjectRoot.GetHierarchyWorldBox();
            Sphere worldSphere = worldBox.GetEncpasulatingSphere();

            Sphere sceneSphere = Octave3DWorldBuilder.ActiveInstance.Octave3DScene.GetEncapuslatingSphere();
            Vector3 newPreviewSphereCenter = sceneSphere.Center - Vector3.right * (sceneSphere.Radius + worldSphere.Radius + 90.0f);
            previewObjectTransform.position += (newPreviewSphereCenter - worldSphere.Center);
            worldBox = previewObjectRoot.GetHierarchyWorldBox();
            worldSphere = worldBox.GetEncpasulatingSphere();

            Transform camTransform = renderCam.transform;
            camTransform.rotation = Quaternion.identity;
            camTransform.rotation = Quaternion.AngleAxis(-45.0f, Vector3.up) * Quaternion.AngleAxis(35.0f, camTransform.right);
            camTransform.position = worldSphere.Center - camTransform.forward * (worldSphere.Radius * 2.0f + renderCam.nearClipPlane);
        
            Mesh previewMesh = previewObjectRoot.GetMeshFromFilterOrSkinnedMeshRenderer();
            if (previewMesh != null)
            {
                Octave3DMesh octaveMesh = Octave3DMeshDatabase.Get().GetOctave3DMesh(previewMesh);
                if (octaveMesh != null && octaveMesh.NumberOfTriangles != 0 && octaveMesh.AllTrianglesFaceAway(renderCam, previewObjectRoot.transform, true))
                {
                    Vector3 normal = Vector3.zero;
                    for (int triIndex = 0; triIndex < octaveMesh.NumberOfTriangles; ++triIndex)
                    {
                        var triangle = octaveMesh.GetTriangle(triIndex);
                        normal += previewObjectRoot.transform.TransformDirection(triangle.Normal).normalized;
                    }

                    normal *= (1.0f / octaveMesh.NumberOfTriangles);
                    normal.Normalize();

                    Vector3 rotAxis = Vector3.Cross(normal, -camTransform.forward).normalized;
                    float rotationAmount = Vector3.Angle(normal, -camTransform.forward);
                    previewObjectTransform.rotation = Quaternion.AngleAxis(rotationAmount, rotAxis);
                }
            }

            SetPreviewLightsActive(true);
            SetupPreviewLights();
            renderCam.Render();
            SetPreviewLightsActive(false);

            GameObject.DestroyImmediate(previewObjectRoot);
            Texture2D previewTexture = new Texture2D(_previewWidth, _previewHeight, TextureFormat.ARGB32, true, PlayerSettings.colorSpace != ColorSpace.Linear);
            previewTexture.ReadPixels(new Rect(0, 0, _previewWidth, _previewHeight), 0, 0);
            previewTexture.Apply();
            UnityEngine.RenderTexture.active = oldRenderTexture;

            renderCam.targetTexture = null;
            RenderTexture.DestroyImmediate(renderTexture);

            return previewTexture;
        }

        public void DestroyData()
        {
            if (_renderCamera != null)
            {
                _renderCamera.targetTexture = null;
                GameObject.DestroyImmediate(_renderCamera.gameObject);
                _renderCamera = null;
            }
            /*if (_renderTexture != null)
            {
                RenderTexture.DestroyImmediate(_renderTexture);
                _renderTexture = null;
            }*/
            DestroyPreviewLights();
        }

        private void DestroyPreviewLights()
        {
            if (_previewLights != null)
            {
                for (int lightIndex = 0; lightIndex < _previewLights.Length; ++lightIndex)
                {
                    if (_previewLights[lightIndex] != null)
                    {
                        GameObject.DestroyImmediate(_previewLights[lightIndex].gameObject);
                    }
                    _previewLights[lightIndex] = null;
                }
            }

            _previewLights = null;
        }

        private Camera GetRenderCamera()
        {
            if(_renderCamera == null)
            {
                GameObject renderCamObject = EditorUtility.CreateGameObjectWithHideFlags("Preview Camera", HideFlags.HideAndDontSave);
                _renderCamera = renderCamObject.AddComponent<Camera>();
            }

            return _renderCamera;
        }

        private RenderTexture GetRenderTexture()
        {
            RenderTexture renderTexture = null;
            if (PlayerSettings.colorSpace == ColorSpace.Linear) renderTexture = new RenderTexture(_previewWidth, _previewHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            else renderTexture = new RenderTexture(_previewWidth, _previewHeight, 24);

            if (renderTexture == null) return null;
            renderTexture.Create();
            return renderTexture;

            /*if (_renderTexture == null)
            {
                if (QualitySettings.activeColorSpace == ColorSpace.Linear) _renderTexture = new RenderTexture(_previewWidth, _previewHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                else _renderTexture = new RenderTexture(_previewWidth, _previewHeight, 24);

                if (_renderTexture == null) return null;
                _renderTexture.Create();
            }

            return _renderTexture;*/
        }

        private Light[] GetPreviewLights()
        {
            if(_previewLights == null || _previewLights.Length == 0 || _previewLights[0] == null)
            {
                _previewLights = new Light[1];

                for(int lightIndex = 0; lightIndex < _previewLights.Length; ++lightIndex)
                {
                    GameObject lightObject = EditorUtility.CreateGameObjectWithHideFlags("Preview Dir Light", HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy);
                    _previewLights[lightIndex] = lightObject.AddComponent<Light>();
                    _previewLights[lightIndex].type = LightType.Directional;
                }

                SetPreviewLightsActive(false);
            }

            return _previewLights;
        }

        private void SetPreviewLightsActive(bool active)
        {
            Light[] lights = GetPreviewLights();
            foreach (var light in lights) { light.gameObject.SetActive(active); }
        }

        private void SetupPreviewLights()
        {
            Light[] lights = GetPreviewLights();
            lights[0].transform.forward = GetRenderCamera().transform.forward;
        
            float lightIntensity = 0.6f;
            foreach (var light in lights) light.intensity = lightIntensity;
        }
    }
}
#endif