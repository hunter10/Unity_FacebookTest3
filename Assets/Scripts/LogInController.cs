using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using System;
using Facebook;
using Facebook.Unity;
using UnityEngine.SceneManagement;

public class LogInController : MonoBehaviour {
	public GameObject BtnFacebook;
	/*
	Login Process

	1) LoginInit()
	2) LoginFacebook()
	3) LoadDataFromFacebook() - Coroutine
	4) LoginGameServer()
	5) LoadDataFromGameServer() - Coroutine
	6) LoadNextScene()
	 */

	 bool[] finished = new bool[10];

	 void Start()
	 {
		 LoginInit();

		 for(int i=0; i<finished.Length; i++){
			 finished[i] = false;
		 }
	 }

	 void LoginInit()
	 {
        // 이미 유저아이디가 있거나 액세스 토큰이 있으면 자동으로 로그인합니다.
		if(UserSingleton.Instance.UserID != 0 && 
           UserSingleton.Instance.AccessToken != "")
        {
            LoginFacebook();
        } else {
            // 저장된 유저아이디가 없으면 새로 로그인합니다.
            BtnFacebook.SetActive(true);
        }
	 }

	 public void LoginFacebook()
	 {
		 FB.Init(delegate{
			 // FB.ActivateApp()함수로 페이스북 SDK를 통해 유저가 얼마나 접속하는지 로깅합니다. 페이스북 관리자 페이지에서 유저의 접속 빈도를 확인할 수 있습니다.
			 FB.ActivateApp();

			 // 페이스북 SDK로 로그인을 수행합니다.
			 UserSingleton.Instance.FacebookLogin(delegate (bool isSuccess, string response){				
				if(isSuccess){
					StartCoroutine(LoadDataFromFacebook());	
				}else {
					Debug.Log("Facebook login fail!!");	
				}
			 });
		 }, OnHideUnity, "");
	 }

	 public IEnumerator LoadDataFromFacebook()
	 {
        UserSingleton.Instance.LoadFacebookMe(delegate (bool isSuccess, string response) {
            finished[0] = true;
        });

        UserSingleton.Instance.LoadFacebookFriend(delegate (bool isSuccess, string response){
            finished[1] = true;
        });

        while (!finished[0] || !finished[1])
        {
            yield return new WaitForSeconds(0.1f);
        }

        LoginGameServer();
	 }

    public void LoginGameServer()
    {
        // 페이스북 로그인 정보를 우리 게임 서버로 보내겠습니다.
        JSONObject body = new JSONObject();
        body.Add("FacebookID", UserSingleton.Instance.FacebookID);
        body.Add("FacebookAccessToken", UserSingleton.Instance.FacebookAccessToken);
        body.Add("FacebookName", UserSingleton.Instance.Name);
        body.Add("FacebookPhotoURL", UserSingleton.Instance.FacebookPhotoURL);
        Debug.Log("Send To Server:" + body.ToString());

        HTTPClient.Instance.POST(Singleton.Instance.HOST + "/Login/Facebook", 
                                 body.ToString(), 
                                 delegate (WWW www) 
        {
            Debug.Log("LoginGameServer ( www.text ) :" + www.text);
            JSONObject response = JSONObject.Parse(www.text);

            int ResultCode = (int)response["ResultCode"].Number;
            if(ResultCode == 1 || ResultCode == 2)
            {
                JSONObject Data = response.GetObject("Data");
                UserSingleton.Instance.UserID = (int)Data["UserID"].Number;
                UserSingleton.Instance.AccessToken = Data["AccessToken"].Str;

                StartCoroutine(LoadDataFromGameServer());
            }
            else
            {
                // 로그인 실패
            }
        });
    }

    public IEnumerator LoadDataFromGameServer()
    {
        UserSingleton.Instance.Refresh(delegate () {
            finished[2] = true;
        });

        RankSingleton.Instance.LoadTotalRank(delegate ()
        {
            finished[3] = true;
        });

        RankSingleton.Instance.LoadFriendRank(delegate ()
        {
            finished[4] = true;
        });

        while(!finished[2] || !finished[3] || !finished[4])
        {
            yield return new WaitForSeconds(0.1f);
        }

        LoadNextScene();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene("Lobby");
    }


    private void OnInitComplete()
    {
        //this.Status = "Success - Check log for details";
        Debug.Log("Success - Check log for details");
    }
    
    // 로그인 과정에서 시간을 멈추는 역할
    public void OnHideUnity(bool isUnityShown)
    {
		 /* 
		 if(!isUnityShown){
			Time.timeScale = 0;
		 } else {
			Time.timeScale = 1;
		 }
		 */
		 Debug.Log("UnityShow : " + isUnityShown);
    }

	 
}
