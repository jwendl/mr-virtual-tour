using Microsoft.WindowsAzure.Storage;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class BaseStorage : MonoBehaviour
{
	public string ConnectionString = string.Empty;

	protected CloudStorageAccount StorageAccount;
	private Text _myText;


#if !UNITY_WSA || UNITY_EDITOR
    /// <summary>
    /// Unity's bug that doesn't handle ssl correctly, at least as of v2017.2
    /// </summary>
    private class CustomCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint sp,
            X509Certificate certificate, WebRequest request, int error)
        {
            return true;
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

    #endif

    // Use this for initialization
    void Awake()
    {

#if !UNITY_WSA || UNITY_EDITOR
        //This works, and one of these two options are required as Unity's TLS (SSL) support doesn't currently work like .NET
        //ServicePointManager.CertificatePolicy = new CustomCertificatePolicy();

        //This 'workaround' seems to work for the .NET Storage SDK, but not event hubs. 
        //If you need all of it (ex Storage, event hubs,and app insights) then consider using the above.
        //If you don't want to check an SSL certificate coming back, simply use the return true delegate below.
        //Also it may help to use non-ssl URIs if you have the ability to, until Unity fixes the issue (which may be fixed by the time you read this)
        ServicePointManager.ServerCertificateValidationCallback = CheckValidCertificateCallback; //delegate { return true; };
#endif
    }

	// Use this for initialization
	void Start ()
	{
		_myText = GameObject.Find("DebugText").GetComponent<Text>();
		StorageAccount = CloudStorageAccount.Parse(ConnectionString);
	}

	public void ClearOutput()
	{
		_myText.text = string.Empty;
	}

	public void WriteLine(string s)
	{
		if(_myText.text.Length > 20000)
			_myText.text = string.Empty + "-- TEXT OVERFLOW --";

		_myText.text += s + "\r\n";
	}
}