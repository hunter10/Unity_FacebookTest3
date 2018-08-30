using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

using UnityEngine.Networking;

public class HTTPClient : MonoBehaviour {
	static GameObject _container;
	static GameObject Container{
		get{
			return _container;
		}
	}

	static HTTPClient _instance;
	public static HTTPClient Instance{
		get{
			if(!_instance){
				_container = new GameObject();
				_container.name = "HTTPClient";
				_instance = _container.AddComponent(typeof(HTTPClient)) as HTTPClient;
			}
			return _instance;
		}
	}

	// GET 함수로 GET 형식의 HTTP 통신을 수행할 수 있습니다.
	public void GET(string url, Action<WWW> callback){
		WWW www = new WWW(url);
		StartCoroutine(WaitWWW(www, callback));
	}

	public void POST(string url, string input, Action<WWW> callback){

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("content-Type", "application/json");
        //input = input.Replace("'", "\"");
        byte[] body = Encoding.UTF8.GetBytes(input);
		WWW www = new WWW(url, body, headers);
        StartCoroutine(WaitWWW(www, callback));
    }

    public IEnumerator WaitWWW(WWW www, Action<WWW> callback)
	{
		yield return www;
		callback(www);
	}
}
