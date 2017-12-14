using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using VirtualTourApi.Constants;
using VirtualTourApi.Models;

public class SceneStartup
    : MonoBehaviour
{
    public GameObject CameraGameObject;
    //public string SceneName;

    private Dictionary<string, GameObject> colliderWithText;
    private GameObject textGameObject;

    // Use this for initialization
    async void Start()
    {
        colliderWithText = new Dictionary<string, GameObject>();
        var sceneData = await FetchSceneData();
        if (sceneData != null)
        {
            var loader = GetComponent<VideoLoader>();
            loader.LoadVideo(sceneData.StorageContainerName, sceneData.StorageVideoName);

            foreach (var unityPrimitive in sceneData.UnityPrimatives)
            {
                // var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                // var point = sceneItem.Transform.Position;
                // sphere.transform.position = new Vector3(point.X, point.Y, point.Z);

                switch (unityPrimitive.PrimativeType)
                {
                    case UnityTypes.CapsuleCollider:
                        BuildCollider(unityPrimitive);
                        break;

                    default:
                        BuildUnityItem(unityPrimitive);
                        break;
                }
            }
        }
    }

    private void BuildUnityItem(UnityPrimitive unityPrimitive)
    {
        var unityResource = Resources.Load(unityPrimitive.PrimativeType, typeof(GameObject));
        var unityInstance = Instantiate(unityResource) as GameObject;

        var position = unityPrimitive.Transform.Position;
        var rotation = unityPrimitive.Transform.Rotation;
        var scale = unityPrimitive.Transform.Scale;

        unityInstance.transform.position = new Vector3(position.X, position.Y, position.Z);
        unityInstance.transform.eulerAngles = new Vector3(rotation.X, rotation.Y, rotation.Z);
        unityInstance.transform.localScale = new Vector3(scale.X, scale.Y, scale.Z);
    }

    private void BuildCollider(UnityPrimitive unityPrimitive)
    {
        var text3dPrefab = Resources.Load("3DTextPrefabResource", typeof(GameObject));
        textGameObject = Instantiate(text3dPrefab) as GameObject;

        var collider = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        var capsuleCollider = collider.AddComponent<CapsuleCollider>();

        var position = unityPrimitive.Transform.Position;
        var rotation = unityPrimitive.Transform.Rotation;
        var scale = unityPrimitive.Transform.Scale;

        collider.transform.position = new Vector3(position.X, position.Y, position.Z);
        collider.transform.eulerAngles = new Vector3(rotation.X, rotation.Y, rotation.Z);
        collider.transform.localScale = new Vector3(scale.X, scale.Y, scale.Z);

        var colliderRenderer = collider.GetComponent<MeshRenderer>();
        colliderRenderer.enabled = false;

        var camera = CameraGameObject.GetComponent<Camera>();
        var cameraPosition = camera.transform.position;

        textGameObject.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z + 1);
        var textRenderer = textGameObject.GetComponent<MeshRenderer>();
        textRenderer.enabled = false;

        var textMesh = textGameObject.GetComponent<TextMesh>();
        textMesh.text = unityPrimitive.Description;
        textMesh.fontSize = 256;

        var eventTrigger = collider.AddComponent<EventTrigger>();
        var enterTrigger = new EventTrigger.TriggerEvent();
        enterTrigger.AddListener((eventData) => OnEnter(unityPrimitive));

        var exitTrigger = new EventTrigger.TriggerEvent();
        exitTrigger.AddListener((eventData) => OnExit(unityPrimitive));

        var entryTriggerEntry = new EventTrigger.Entry() { callback = enterTrigger, eventID = EventTriggerType.PointerEnter };
        eventTrigger.triggers.Add(entryTriggerEntry);

        var exitTriggerEntry = new EventTrigger.Entry() { callback = exitTrigger, eventID = EventTriggerType.PointerExit };
        eventTrigger.triggers.Add(exitTriggerEntry);

        colliderWithText.Add(unityPrimitive.Name, textGameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (textGameObject != null)
        {
            var camera = CameraGameObject.GetComponent<Camera>();
            var cameraPosition = camera.transform.position;
            textGameObject.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, cameraPosition.z + 1);
        }
    }

    private void OnEnter(UnityPrimitive unityPrimitive)
    {
        var gameObject = colliderWithText[unityPrimitive.Name];
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
    }

    private void OnExit(UnityPrimitive unityPrimitive)
    {
        var gameObject = colliderWithText[unityPrimitive.Name];
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    public async Task<SceneData> FetchSceneData()
    {
        // TODO: provide scene name instead of hard coded Cafeteria.
        var uri = new Uri($"http://mr-virtual-tour.azurewebsites.net/api/scene/{Statics.sceneName}");
        var webClient = new WebClient();

        var json = await webClient.DownloadStringTaskAsync(uri);
        return JsonConvert.DeserializeObject<SceneData>(json);
    }
}
