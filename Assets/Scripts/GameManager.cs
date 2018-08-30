using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // test

        // HTTPClient.Instance.GET("https://azuretest0709.azurewebsites.net/api/values", delegate(WWW www){
        // 	Debug.Log(www.text);
        // });

        //HTTPClient.Instance.GET("https://azuretest0709.azurewebsites.net/api/values", delegate(WWW www){
        //	Debug.Log(www.text);
        //});

        JSONObject body = new JSONObject();
        body.Add("FacebookID", "1");
        body.Add("FacebookAccessToken", "1234");
        body.Add("FacebookName", "Chris");
        body.Add("FacebookPhotoURL", "http://abc");
        Debug.Log("Send To Server:" + body.ToString());

        HTTPClient.Instance.POST("https://azuretest0709.azurewebsites.net/Login/Facebook",
            body.ToString(),
            delegate (WWW www) {
            Debug.Log(www.text);
        });

    }

    // Update is called once per frame
    void Update () {
		
	}
}
