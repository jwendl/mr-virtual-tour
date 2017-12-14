using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using UnityEngine.Video;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;

public class VideoLoader : MonoBehaviour
{

    public GameObject VideoObject;
    private Task downloadTask;
    private string path;
    private bool loaded = false;
    private bool getFromCache = false;

    // Use this for initialization
    public void LoadVideo(string containerName, string videoName)
    {
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidCertificateCallback);
        var ConnectionString = "DefaultEndpointsProtocol=https;AccountName=mrvirtualtour;AccountKey=QxwukxlyZ3V8K+Xq81bn81l2KDo2zDqqsdIdt5Hg4n6ofhbOyLP2860z8j9vhHDUVPlU9N1woYSdyWB7oc/Khg==;EndpointSuffix=core.windows.net";
        var ContainerName = containerName;
        var VideoName = videoName;
        var StorageAccount = CloudStorageAccount.Parse(ConnectionString);
        CloudBlobClient blobClient = StorageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);
        var cloudBlockBlob = container.GetBlockBlobReference(VideoName);
#if WINDOWS_UWP
		storageFolder = ApplicationData.Current.TemporaryFolder;
		sf = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
		path = sf.Path;
		await blockBlob.DownloadToFileAsync(sf);
#else
        path = Path.Combine(Application.temporaryCachePath, VideoName);
        path = path.Replace("/", "\\");
        Debug.Log("Path says: " + path);
        Debug.Log(VideoObject.ToString());

        if (File.Exists(path))
        {
            getFromCache = true;
        }
        else
        {
            downloadTask = cloudBlockBlob.DownloadToFileAsync(path, FileMode.Create);
        }

#endif

    }

    // Update is called once per frame
    void Update()
    {
        if (!loaded && downloadTask != null && downloadTask.IsCompleted)
        {
            Debug.Log("THIS THING SHOULD HAVE BEEN DOWNLOADED !!! YAAY OR NAY!?");
            var myVideo = VideoObject.GetComponent<VideoPlayer>();
            myVideo.url = "file:///" + path;
            loaded = true;
        }
        else if (!loaded && getFromCache)
        {
            Debug.Log("Was in cache");
            var myVideo = VideoObject.GetComponent<VideoPlayer>();
            myVideo.url = "file:///" + path;
            loaded = true;
        }
    }

    public bool CheckValidCertificateCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool valid = true;

        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        valid = false;
                    }
                }
            }
        }
        return valid;
    }

}
