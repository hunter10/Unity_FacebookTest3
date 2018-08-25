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
		 if(UserSingleton.Instance.UserID != 0 &&
		 UserSingleton.Instance.AccessToken != ""){
			LoginFacebook();	 	
		 } else {
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
		 
		 //FB.Init(OnInitComplete, OnHideUnity);
	 }

	 public IEnumerator LoadDataFromFacebook()
	 {
		 while(!finished[0] || !finished[1]){
			 yield return new WaitForSeconds(0.1f);
		 }
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
